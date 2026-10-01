using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Repositories.Interfaces;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Validators.Operations.Fluents;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using AutoMapper;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Validators.Maintenance
{
    public class RegistrationExcelValidator : IValidator<RegistrationExcelViewModel>
    {

        ILookupRepository lookupRepository;
        ICompanyRepository companyRepository;
        IScopeDataProvider dataProvider;

        public RegistrationExcelValidator(ILookupRepository lookupRepository, ICompanyRepository companyRepository, IScopeDataProvider dataProvider)
        {
            this.lookupRepository = lookupRepository;
            this.companyRepository = companyRepository;
            this.dataProvider = dataProvider;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationExcelViewModel item)
        {
            var output = new List<ValidationMessage>();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (identity.CompanyCode != item.CompanyCode)
            {
                output.Add(new ValidationMessage("", $"Registration upload can be done only for the company code {identity.CompanyCode}. {item.CompanyCode} is not allowed"));

            }
            CheckLookup<LookupAgencyType>(item.AgencyTypeCode, "Agency Type", output);
            CheckLookup<LookupState>(item.AgencyStateCode, "State", output);
            CheckLookup<LookupBank>(item.BankCode, "Bank code in Agency Banker", output);
            CheckLookup<LookupState>(item.BankStateCode, "State code in Agency Banker", output);
            CheckCompany(item.CompanyCode, "Company code", output);
            CheckLookup<LookupGuarantorType>(item.GuarantorTypeCode, "Guarantor Type", output);
            CheckLookup<LookupIntermediaryType>(item.IntermediaryTypeCode, "Intermediary Type", output);
            CheckLookup<LookupDesignation>(item.Member1DesignationCode, "Designation for Member 1", output);
            CheckLookup<LookupDesignation>(item.Member2DesignationCode, "Designation for Member 2", output);
            CheckLookup<LookupDesignation>(item.Member3DesignationCode, "Designation for Member 3", output);
            CheckLookup<LookupDesignation>(item.Member4DesignationCode, "Designation for Member 4", output);
            CheckLookup<LookupDesignation>(item.Member5DesignationCode, "Designation for Member 5", output);
            CheckLookup<LookupDesignation>(item.Member6DesignationCode, "Designation for Member 6", output);
            CheckLookup<LookupDesignation>(item.Member7DesignationCode, "Designation for Member 7", output);
            CheckLookup<LookupDesignation>(item.Member8DesignationCode, "Designation for Member 8", output);
            CheckLookup<LookupDesignation>(item.Member9DesignationCode, "Designation for Member 9", output);
            CheckLookup<LookupICType>(item.Member1ICType, "ICType for Member 1", output);
            CheckLookup<LookupICType>(item.Member2ICType, "ICType for Member 2", output);
            CheckLookup<LookupICType>(item.Member3ICType, "ICType for Member 3", output);
            CheckLookup<LookupICType>(item.Member4ICType, "ICType for Member 4", output);
            CheckLookup<LookupICType>(item.Member5ICType, "ICType for Member 5", output);
            CheckLookup<LookupICType>(item.Member6ICType, "ICType for Member 6", output);
            CheckLookup<LookupICType>(item.Member7ICType, "ICType for Member 7", output);
            CheckLookup<LookupICType>(item.Member8ICType, "ICType for Member 8", output);
            CheckLookup<LookupICType>(item.Member9ICType, "ICType for Member 9", output);
            CheckLookup<LookupEducationalQualification>(item.NomineeEducationalQualificationCode, "Educational qualificaiton code", output);
            CheckLookup<LookupGender>(item.NomineeGenderCode, "Nominee Gender Code", output);
            CheckLookup<LookupICType>(item.NomineeICTypeCode, "Nominee IC Type", output);
            CheckLookup<LookupMaritalStatus>(item.NomineeMaritalStatusCode, "Nominee Marital Status", output);
            CheckLookup<LookupRace>(item.NomineeRaceCode, "Nominee Race Code", output);
            CheckLookup<LookupMemberLevel>(item.NomineeRankCode, "Rank for Nominee", output);
            CheckLookup<LookupReligion>(item.NomineeReligionCode, "Nominee Religion Code", output);
            CheckLookup<LookupTbeCategory>(item.NomineeTbeCategoryCode, "Nominee TBE Category Code", output);
            CheckLookup<LookupMaritalStatus>(item.SpouseMaritalStatusCode, "Spouse Marital Status", output);
            CheckLookup<LookupRace>(item.SpouseRaceCode, "Spouse Race Code", output);
            CheckLookup<LookupReligion>(item.SpouseReligionCode, "Spouse Religion Code", output);

            ValidateNewBusinessRegistrationNumber(item.NewBusinessRegistrationNumber, "New Business Registration Number", output);
            ValidateNewBusinessRegistrationNumber(item.Member1NewBusinessRegistrationNumber, "New Business Registration Number 1", output);
            ValidateNewBusinessRegistrationNumber(item.Member2NewBusinessRegistrationNumber, "New Business Registration Number 2", output);
            ValidateNewBusinessRegistrationNumber(item.Member3NewBusinessRegistrationNumber, "New Business Registration Number 3", output);
            ValidateNewBusinessRegistrationNumber(item.Member4NewBusinessRegistrationNumber, "New Business Registration Number 4", output);
            ValidateNewBusinessRegistrationNumber(item.Member5NewBusinessRegistrationNumber, "New Business Registration Number 5", output);
            ValidateNewBusinessRegistrationNumber(item.Member6NewBusinessRegistrationNumber, "New Business Registration Number 6", output);
            ValidateNewBusinessRegistrationNumber(item.Member7NewBusinessRegistrationNumber, "New Business Registration Number 7", output);
            ValidateNewBusinessRegistrationNumber(item.Member8NewBusinessRegistrationNumber, "New Business Registration Number 8", output);
            ValidateNewBusinessRegistrationNumber(item.Member9NewBusinessRegistrationNumber, "New Business Registration Number 9", output);

            if (output.Count == 0)
            {
                var validateModel = GetModel(item);
                var validator = GetValidator(item);
                Validate(validateModel, validator, output);
                Validate(validateModel.Guarantor, ObjectContainer.Container.Resolve<GuarantorViewModelValidator>(), output);
                Validate(validateModel.CorporateNominee.Qualification, ObjectContainer.Container.Resolve<QualificationViewModelValidator>(), output);
                Validate(validateModel.CorporateNominee, ObjectContainer.Container.Resolve<CorporateNomineeViewModelValidator>(), output);
            }
            foreach (var msg in output)
            {
                item.AddError(msg.ErrorMessage);
            }
            return output;
        }

        void CheckLookup<T>(string value, string caption, List<ValidationMessage> list) where T : EntityObject, ILookupEntity
        {
            if (String.IsNullOrWhiteSpace(value)) return;
            var lookups = lookupRepository.GetAll<T>();
            if (lookups.Where(p => p.Code == value).Count() == 1) return;
            var values = String.Join(",", lookups.Select(p => p.Code).ToArray());
            list.Add(new ValidationMessage("", "The value {0} is not permitted for {1}. List of permissible values are {2}", value, caption, values));
        }

        void CheckCompany(string value, string caption, List<ValidationMessage> list)
        {
            if (companyRepository.Get(value) == null)
            {
                list.Add(new ValidationMessage("", "Company code {0} is not a valid code", value));
            }
        }

        void Validate(object model, FluentValidation.IValidator validator, List<ValidationMessage> list)
        {
            var validationList = validator.Validate(model);
            if (validationList.IsValid) return;
            foreach (var error in validationList.Errors)
            {
                list.Add(new ValidationMessage("", error.ErrorMessage));
            }
        }

        RegistrationViewModel GetModel(RegistrationExcelViewModel model)
        {
            switch (model.AgencyTypeCode)
            {
                case LookupConstants.AgencyType.Individual:
                    return Mapper.Map<IndividualRegistrationViewModel>(model);
                case LookupConstants.AgencyType.SoleProprietorship:
                    return Mapper.Map<SoleProprietorshipRegistrationViewModel>(model);
                case LookupConstants.AgencyType.Partnership:
                    return Mapper.Map<PartnershipRegistrationViewModel>(model);
                case LookupConstants.AgencyType.PrivateLimitedCompany:
                case LookupConstants.AgencyType.PublicLimitedCompany:
                case LookupConstants.AgencyType.GovernmentAgency:
                case LookupConstants.AgencyType.Cooperative:
                    return Mapper.Map<CorporateRegistrationViewModel>(model);
            }
            return null;
        }

        FluentValidation.IValidator GetValidator(RegistrationExcelViewModel model)
        {
            var container = ObjectContainer.Container;
            switch (model.AgencyTypeCode)
            {
                case LookupConstants.AgencyType.Individual:
                    return container.Resolve<IndividualRegistrationViewModelValidator>();
                case LookupConstants.AgencyType.SoleProprietorship:
                    return container.Resolve<SoleProprietorshipRegistrationViewModelValidator>();
                case LookupConstants.AgencyType.Partnership:
                    return container.Resolve<PartnershipRegistrationViewModelValidator>();
                case LookupConstants.AgencyType.PrivateLimitedCompany:
                case LookupConstants.AgencyType.PublicLimitedCompany:
                case LookupConstants.AgencyType.GovernmentAgency:
                case LookupConstants.AgencyType.Cooperative:
                    return container.Resolve<CorporateRegistrationViewModelValidator<CorporateRegistrationViewModel>>();
            }
            return null;
        }

        void ValidateNewBusinessRegistrationNumber(string input, string caption, List<ValidationMessage> list)
        {
            if (String.IsNullOrWhiteSpace(input))
                return;
            if (!input.IsNewBusinessRegistrationNumber())
                list.Add(new ValidationMessage("", "{0} should be 12 digits".FormatWith(caption)));
        }

    }// class
}// namesapce
