using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Repositories.Interfaces;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Utilities.Constants;
using System.Transactions;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Validators.Operations;

namespace MTAoarsGeneral.Services.Operations {
    public class UploadService : BaseService, IUploadService {

        IRepository<IbfimResult> ibfimRepository;
        IUploadRepository uploadRepository;
        INotificaitonService notificationService;
        IRegistrationService registrationService;
        IRegistrationBuilder registrationBuilder;
        ConfigManager config;

        public UploadService(IValidationProvider validationProvider, IRepository<IbfimResult> ibfimRepository,
            IUploadRepository uploadRepository, IRegistrationService registrationService, INotificaitonService notificationService, IRegistrationBuilder registrationBuilder, ConfigManager config)
            : base(validationProvider) {
                this.ibfimRepository = ibfimRepository;
                this.uploadRepository = uploadRepository;
                this.notificationService = notificationService;
                this.registrationService = registrationService;
                this.registrationBuilder = registrationBuilder;
                this.config = config;
        }

        public void UploadIbfim(ExcelList<IbfimExcelViewModel> items) {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan)) {
                foreach (var item in items) {
                    Validate(item);
                    if (item.IsValid() == false) continue;
                    var result = GetResult(item, "A", "1");
                    if (result != null) ibfimRepository.Save(result);
                    result = GetResult(item, "B", "2");
                    if (result != null) ibfimRepository.Save(result);
                    result = GetResult(item, "C", "3");
                    if (result != null) ibfimRepository.Save(result);
                }
                ibfimRepository.SaveChanges();

                var history = SaveFiles(items.Cast<ExcelViewModel>(), LookupConstants.Uploads.Ibfim, items.RawColumnsData,
                    items.FileName, items.CompanyID);
                
                scope.Complete();
            }
        }

        public void UploadRegistration(ExcelList<RegistrationExcelViewModel> items) {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan)) {
                foreach (var item in items) {
                    Validate(item);
                    if (item.IsValid() == false) continue;
                    if (AddRegistration(item) == false) {
                        foreach (var msg in registrationService.CurrentContext.ValidationMessages) {
                            item.AddError(msg.ErrorMessage);
                        }
                    } //if
                }// foreach

                var history = SaveFiles(items.Cast<ExcelViewModel>(), LookupConstants.Uploads.Registration, items.RawColumnsData,
                   items.FileName, items.CompanyID);

                foreach (var item in items) {
                    var uploadedItem = Mapper.Map<RegistrationUploadHistory>(item);
                    history.RegistrationUploadHistories.Add(uploadedItem);
                }
                uploadRepository.SaveChanges();
                scope.Complete();
            }//using
        }

        bool AddRegistration(RegistrationExcelViewModel input ) {
            registrationService.CurrentContext.ValidationMessages.Clear();
            var indexModel = Mapper.Map<RegistrationIndexViewModel>(input);         
            var options = registrationService.Check(indexModel);
            if (options == RegistrationCheckOptions.Error) return false;
            switch(options) {
                case RegistrationCheckOptions.New:
                    var addModel = GetNewRegistration(input);
                    addModel.CurrentAction = Actions.Add;
                    long id = registrationService.Add(addModel);
                    input.SetRemarks(uploadRepository.GetAgencyNumber(id));
                    break;
                case RegistrationCheckOptions.Include:
                    var inclusionModel = registrationBuilder.GetInclusion(indexModel);
                    inclusionModel.Guarantor = Mapper.Map<GuarantorViewModel>(input);
                    registrationService.Include(inclusionModel);
                    input.SetRemarks(uploadRepository.GetAgencyNumber(inclusionModel.AgencyID));
                    break;
                case RegistrationCheckOptions.Referred:
                    registrationService.CurrentContext.ValidationMessages.Add(new ValidationMessage("", "This agent is in referred agent list so it could not be added through upload"));
                    break;
                case RegistrationCheckOptions.Reinstate:
                    var reinstationModel = registrationBuilder.GetReinstation(indexModel);
                    registrationService.Reinstate(reinstationModel);
                    input.SetRemarks(uploadRepository.GetAgencyNumber(reinstationModel.AgencyID));
                    break;
            }
            return registrationService.CurrentContext.IsSuccess;
        }

        RegistrationViewModel GetNewRegistration(RegistrationExcelViewModel model) {
            switch (model.AgencyTypeCode) {
                case LookupConstants.AgencyType.Individual:
                    return Mapper.Map<IndividualRegistrationViewModel>(model);
                case LookupConstants.AgencyType.SoleProprietorship:
                    return Mapper.Map<SoleProprietorshipRegistrationViewModel>(model);
                case LookupConstants.AgencyType.Partnership:
                    return Mapper.Map<PartnershipRegistrationViewModel>(model);
                case LookupConstants.AgencyType.Cooperative:
                case LookupConstants.AgencyType.GovernmentAgency:
                case LookupConstants.AgencyType.PrivateLimitedCompany:
                case LookupConstants.AgencyType.PublicLimitedCompany:
                    return Mapper.Map<CorporateRegistrationViewModel>(model);
            }
            return null;
        }

        public string GetHistoryErrorPath(long id) {
            var history = uploadRepository.Get(id);
            return history.ErrorFilePath;
        }

        public string GetHistorySuccessPath(long id) {
            var history = uploadRepository.Get(id);
            return history.SuccessFilePath;
        }

        public string GetHistoryRawPath(long id) {
            var history = uploadRepository.Get(id);
            return history.RawFilePath;
        }

        IbfimResult GetResult(IbfimExcelViewModel input, string part, string partCode) {
            if (!input.ExaminationTypeCode.Contains(partCode)) return null;
            var output = Mapper.Map<IbfimResult>(input);
            output.ExamType = part;
            return output;
        }

        UploadHistory SaveFiles(IEnumerable<ExcelViewModel> items, string code, string columns, string fileName, long companyId) {
            var path = Path.Combine(Path.Combine(config.UploadBaseDirectory, code),  Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            var rawFilePath = Path.Combine(path, "Raw.csv");
            var successFilePath = Path.Combine(path, "Success.csv");
            var errorFilePath = Path.Combine(path, "Error.csv");
            var successRecords = items.Where(p => p.IsValid());
            var errorRecords = items.Where(p => !p.IsValid());
            WriteFile(rawFilePath, columns, items);
            WriteFile(successFilePath, columns, successRecords);
            WriteFile(errorFilePath, columns, errorRecords);
            var output = new UploadHistory {
                Code = code, SuccessFilePath = successFilePath , ErrorFilePath = errorFilePath, RawFilePath = rawFilePath,
                TotalValidRecords = successRecords.Count(), TotalInvalidRecords = errorRecords.Count(),
                UploadFileName = fileName, CompanyID =  companyId
            };
            uploadRepository.Save(output);
            uploadRepository.SaveChanges();
            var variableSource = Mapper.Map<ExcelUploadVariableSource>(output);
            notificationService.Notify(MTACompany.ID, companyId, LookupConstants.Notifications.ExcelUpload, variableSource);
            return output;
        }

        void WriteFile(string path, string columns, IEnumerable<ExcelViewModel> items) {
            var sb = new StringBuilder();
            sb.Append(columns);
            sb.AppendLine(",Errors,Remarks");
            foreach (var item in items) {
                sb.Append(item.GetRawData());
                sb.Append(",");
                item.GetErrors().Each(p => sb.Append(p));
                sb.Append(",");
                sb.AppendLine(item.GetRemarks());
            }
            File.WriteAllText(path, sb.ToString());
        }
    }// class
}// namespace
