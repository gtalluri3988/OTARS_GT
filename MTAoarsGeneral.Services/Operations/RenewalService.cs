using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MTAoarsGeneral.ViewModels.Operations;
using System.Data;
using System.Threading.Tasks;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Administrative;

namespace MTAoarsGeneral.Services.Operations {
    public class RenewalService : BaseService, IRenewalService
    {

        IRenewalRepository renewalRepository;
        ILookupRepository lookupRepository;
        INotificaitonService notificationService;
        IAgencyService agencyService;
        IObjectCreator objectCreator;
        IRepository<RenewalHeader> renewalHeaderRepository;
        IAgencyRepository agencyRepository;
        readonly long generalIntermediaryTypeId;

        public RenewalService(IValidationProvider validationProvider, IRenewalRepository renewalRepository, ILookupRepository lookupRepository, INotificaitonService notificationService, 
            IAgencyService agencyService, IObjectCreator objectCreator, IRepository<RenewalHeader> renewalHeaderRepository, IAgencyRepository agencyRepository)
            : base(validationProvider) {
            this.renewalRepository = renewalRepository;
            this.lookupRepository = lookupRepository;
            this.notificationService = notificationService;
            this.agencyService = agencyService;
            this.objectCreator = objectCreator;
            this.renewalHeaderRepository = renewalHeaderRepository;
            this.agencyRepository = agencyRepository;
            generalIntermediaryTypeId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
        }

        public void GenerateRenewableRecords()
        {
            /* for debug purpose only */
            /* ------------------------------------------------------ */
            //var dt = new DateTime(2020, 2, 1);
            //var validTo = dt.GetCurrentQuarterEndDate();
            /* ------------------------------------------------------ */

            var validTo = DateTime.Now.GetCurrentQuarterEndDate();
            var companies = renewalRepository.GetRenewableCompanies(validTo);
            //   using (var scope = new TransactionScope()) {
            foreach (var companyId in companies)
            {
                var principals = renewalRepository.GetRenewableAgencies(companyId, validTo);
                var header = GetHeader(companyId, validTo, principals);
                renewalRepository.Save(header);
                
                var source = new RenewalVariableSource
                {
                    ValidToDate = validTo.ToString(GlobalConstants.DateFormat),
                    TotalAgents = header.RenewalDetails.Count
                };
                notificationService.Notify(MTACompany.ID, companyId, LookupConstants.Notifications.Renewal, source);
            }
            renewalRepository.SaveChanges();
            //  scope.Complete();
            // }
        }

        public void SendReminders() {
            var validTo = DateTime.Now.GetCurrentQuarterEndDate();
            var details = renewalRepository.GetReminderDetails(validTo.Year, validTo.GetQuarter());
            foreach (var detail in details) {
                var source = Mapper.Map<RenewalReminderVariableSource>(detail);
                notificationService.Notify(MTACompany.ID, detail.CompanyID, LookupConstants.Notifications.RenewalReminder, source);

            }
        }

        RenewalHeader GetHeader(long companyId, DateTime validTo, IEnumerable<AgencyPrincipal> principals)
        {
            var headerStatus = lookupRepository.Get<LookupRenewalHeaderStatus>(LookupConstants.RenewalHeaderStatus.Generated);
            var detailStatus = lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Generated);
            var header = new RenewalHeader
            {
                CompanyID = companyId,
                Quarter = validTo.GetQuarter(),
                Year = validTo.Year,
                StatusID = headerStatus.ID
            };
            foreach (var principal in principals)
            {
                var detail = new RenewalDetail
                {
                    AgencyID = principal.AgencyID,
                    StatusID = detailStatus.ID,
                    IntermediaryTypeID = generalIntermediaryTypeId,
                    AgencyPrincipalID = principal.ID, /* [20190926] Add in AgencyPrincipalID to show correct information in Invoice */
                    AgencyNumber = principal.AgencyNumber, /* [20191121] - Do not use AgencyPrincipalID in table dbo.Journal due to it is not unique, instead use AgencyNumber */
                    TypeID = principal.TypeID /* [20200115] Usage on SQL View [RenewalViewDetail] */
                };
                header.RenewalDetails.Add(detail);
            }
            return header;
        }

        public void Save(RenewalHeaderViewModel model) {
            Update(model, LookupConstants.RenewalHeaderStatus.Saved);

        }

        public void Submit(RenewalHeaderViewModel model) {
            Update(model, LookupConstants.RenewalHeaderStatus.Submitted);
            var identity = objectCreator.Create<IScopeDataProvider>().Get<Identity>(GlobalConstants.CurrentIdentity);
            var header = renewalHeaderRepository.Get(model.ID);
            var source = new RenewalSubmissionVariableSource { CompanyName = identity.CompanyName, Year = header.Year, Quarter = header.Quarter };
            notificationService.Notify(identity.CompanyID, MTACompany.ID, LookupConstants.Notifications.RenewalSubmitted, source);
        }

        public void Process() {
            var mailCompanies = new Dictionary<long, BatchRenewalVariableSource>();
            //using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan)) {
            var previusQuarterEndDate = DateTime.Now.GetPreviousQuarterEndDate();
            var headers = renewalRepository.GetHeaders(previusQuarterEndDate.Year, previusQuarterEndDate.GetQuarter());

            foreach (var header in headers) {
                Process(header, mailCompanies, true);
                header.StatusID = lookupRepository.Get<LookupRenewalHeaderStatus>(LookupConstants.RenewalHeaderStatus.Processed).ID;
            }
            renewalRepository.SaveChanges();
            foreach (var key in mailCompanies.Keys) {
                notificationService.Notify(MTACompany.ID, key, LookupConstants.Notifications.BatchRenewal, mailCompanies[key]);
            }
            //scope.Complete();
            // }
        }

        public void Accept(long headerId) {
            var mailCompanies = new Dictionary<long, BatchRenewalVariableSource>();
            using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan)) {
                var header = renewalRepository.Get(headerId);
                Process(header, mailCompanies, false);
                renewalRepository.SaveChanges();
                foreach (var key in mailCompanies.Keys) {
                    notificationService.Notify(MTACompany.ID, key, LookupConstants.Notifications.BatchRenewal, mailCompanies[key]);
                }
                scope.Complete();
            }
        }

        public void Reject(long headerId) {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan)) {
                var header = renewalRepository.Get(headerId);
                var details = header.RenewalDetails.Where(p => p.IsSubmitted == true && p.LookupRenewalDetailStatus.Code != LookupConstants.RenewalDetailStatus.Generated).ToList();
                foreach (var detail in details) detail.IsSubmitted = false;
                renewalRepository.SaveChanges();
                var variableSource = new RenewalRejectionVariableSource { Total = details.Count(), Year = header.Year, Quarter = header.Quarter };
                notificationService.Notify(MTACompany.ID, header.CompanyID, LookupConstants.Notifications.RenewalRejection, variableSource);
                scope.Complete();
            }
        }

        void Update(RenewalHeaderViewModel model, string statusCode)
        {
            var header = renewalRepository.Get(model.ID);
            header.StatusID = lookupRepository.Get<LookupRenewalHeaderStatus>(statusCode).ID;
            long generatedId = lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Generated).ID;
            var isSubmitted = statusCode == LookupConstants.RenewalHeaderStatus.Submitted;
            foreach (var detail in header.RenewalDetails)
            {
                logger.Info(string.Format("Renewal detail ID: {0}", detail.ID));
                var modelDetail = model.Details.SingleOrDefault(p => p.ID == detail.ID);
                if (modelDetail == null) continue;
                detail.IsSubmitted = isSubmitted && (modelDetail.RenewalStatus.ID != generatedId);
                detail.StatusID = modelDetail.RenewalStatus.ID;
                detail.Remarks = modelDetail.Remarks;

                // detail.Description = modelDetail.Description;
            }
            renewalRepository.SaveChanges();
        }

        public RenewalPageViewModel GetRenwalDetails(GridPageViewModel model, long headerId, bool isSubmitted) {

            var output = objectCreator.Create<RenewalPageViewModel>();
            var orderBy = GetSortOrder(model);
            var search = GetSearch(model);
            var list = renewalRepository.GetRenwalDetails(model.Page, model.PageSize, search, orderBy, headerId, isSubmitted);
            foreach (var item in list) {
                output.Data.Add(Mapper.Map<RenewalDetailViewModel>(item));
            }
            output.TotalRecords = renewalRepository.GetRenewalDetailsCount(headerId, search, isSubmitted);
            return output;
        }

        void Process(RenewalHeader header, Dictionary<long, BatchRenewalVariableSource> mailCompanies, bool allRecord)
        {
            logger.Info("============================================================");
            logger.Info("RenewalHeader.ID: {0}, RenewalHeader.CompanyID: {1}, RenewalHeader.Company.Name: {2}, RenewalHeader.Year: {3}, RenewalHeader.Quarter: {4}".FormatWith(
                header.ID, header.CompanyID, header.Company.Name, header.Year, header.Quarter));

            var details = header.RenewalDetails.Where(p => p.IsProcessed == false).ToList();
            if (!allRecord)
                details = details.Where(p => p.LookupRenewalDetailStatus.Code != LookupConstants.RenewalDetailStatus.Generated).ToList();

            foreach (var detail in details)
            {
                logger.Info("-------------------------------------------------");
                logger.Info("RenewalDetail.AgencyID: {0}, RenewalDetail.IntermediaryTypeID: {1}, RenewalDetail.LookupRenewalDetailStatus.Code: {2}, RenewalDetail.AgencyPrincipalID: {3}, RenewalDetail.AgencyNumber: {4}".FormatWith(
                    detail.AgencyID, detail.IntermediaryTypeID, detail.LookupRenewalDetailStatus.Code, detail.AgencyPrincipalID, detail.AgencyNumber));

                //var principal = detail.Agency.AgencyPrincipals.SingleOrDefault(
                //    p => p.CompanyID == detail.RenewalHeader.CompanyID && p.IntermediaryTypeID == detail.IntermediaryTypeID);

                /* [20190926] Add in AgencyPrincipalID to show correct information in Invoice */
                AgencyPrincipal principal = null;
                if (detail.AgencyPrincipalID.HasValue)
                    principal = agencyRepository.GetAgencyPrincipal(Convert.ToInt64(detail.AgencyPrincipalID));

                /* [20191121] - Do not use AgencyPrincipalID in table dbo.Journal due to it is not unique, instead use AgencyNumber */
                if (!string.IsNullOrEmpty(detail.AgencyNumber))
                    principal = agencyRepository.GetAgencyPrincipal(detail.AgencyNumber);

                /* Only pull the latest match AgencyPrincipal record if not record found */
                if (principal == null)
                {
                    var matchedAgentPrincipals = detail.Agency.AgencyPrincipals
                        .Where(p => p.CompanyID == detail.RenewalHeader.CompanyID && p.IntermediaryTypeID == generalIntermediaryTypeId).ToList();

                    if (matchedAgentPrincipals.Count > 0)
                    {
                        var latestAgencyPrincipalID = matchedAgentPrincipals.Max(i => i.ID);
                        principal = agencyRepository.GetAgencyPrincipal(latestAgencyPrincipalID); /* [20191107] Get the Max Agency Principal ID */
                    }
                }

                detail.IsProcessed = true;

                if (principal == null) continue;

                if (!mailCompanies.ContainsKey(principal.CompanyID))
                    mailCompanies.Add(principal.CompanyID, new BatchRenewalVariableSource());

                switch (detail.LookupRenewalDetailStatus.Code)
                {
                    case LookupConstants.RenewalDetailStatus.Renew:
                        agencyService.Renew(principal.ID, true);
                        mailCompanies[principal.CompanyID].RenewalList.Add(
                            new RenewedVariableSource
                            {
                                AgencyName = principal.Agency.Name,
                                AgencyNumber = principal.AgencyNumber,
                                IntermediaryTypeDescription = principal.LookupIntermediaryType.Description,
                                ValidToDate = principal.ValidTo.Value
                            }
                        );
                        break;

                    case LookupConstants.RenewalDetailStatus.Generated:
                    case LookupConstants.RenewalDetailStatus.Terminate:
                        agencyService.Terminate(principal.ID, detail.Remarks, principal.ValidTo.Value, allRecord, true);
                        mailCompanies[principal.CompanyID].TerminationList.Add(
                            new TerminatedVariableSource
                            {   
                                AgencyName = principal.Agency.Name,
                                AgencyNumber = principal.AgencyNumber,
                                IntermediaryTypeDescription = principal.LookupIntermediaryType.Description
                            }
                        );
                        break;

                    case LookupConstants.RenewalDetailStatus.NotReleased:
                        agencyService.NotRelease(principal.ID, detail.Remarks, principal.ValidTo.Value, true);
                        break;

                    case LookupConstants.RenewalDetailStatus.Resign:
                        agencyService.Resign(principal.ID, detail.Remarks, principal.ValidTo.Value, true);
                        break;
                }

            }
        }

        //new added
        public RenewalDetailsViewModel GetItemsRenewalDetails(GridPageViewModel model, long headerId, bool isSubmitted)
        {
            var output = objectCreator.Create<RenewalDetailsViewModel>();
            var orderBy = GetSortOrder(model);
            var search = GetSearch(model);
            var results = renewalRepository.GetRenwalDetails(model.Page, model.PageSize, search, orderBy, headerId, isSubmitted);
            results = results?.Where(x => x?.StatusID != lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Generated).ID).ToList();
            foreach (var i in results)
            {
                output.RenewalDetail.Add(Mapper.Map<RenewalDetailViewModel>(i));
            }

            //output.TotalRecord = renewalRepository.GetRenewalDetailsCount(headerId, search, isSubmitted);
            output.TotalRecord = results.Count();


            return output;
        }

        public void SaveAdminstrativeRenewalDetails(RenewalDetailsViewModel model)
        {
            var StatusCode = LookupConstants.RenewalHeaderStatus.Saved;
            var header = renewalRepository.Get(model.ID);
            header.StatusID = lookupRepository.Get<LookupRenewalHeaderStatus>(StatusCode).ID;
            long generatedId = lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Generated).ID;
            
            foreach (var detail in header.RenewalDetails)
            {
                var modelDetail = model.RenewalDetail.SingleOrDefault(p => p.ID == detail.ID);
                if (modelDetail == null) continue;
                if(modelDetail.RenewalStatus.ID == generatedId) detail.IsSubmitted = false;
                detail.StatusID = modelDetail.RenewalStatus.ID;
                detail.Remarks = modelDetail.Remarks;
            }
            renewalRepository.SaveChanges();
        }

        public void SaveRenewalDetails(RenewalDetailsViewModel model)
        {
            UpdateRenewalDetails(model, LookupConstants.RenewalHeaderStatus.Saved);

        }

        public void SubmitRenewalDetails(RenewalDetailsViewModel model)
        {
            UpdateRenewalDetails(model, LookupConstants.RenewalHeaderStatus.Submitted);
            var identity = objectCreator.Create<IScopeDataProvider>().Get<Identity>(GlobalConstants.CurrentIdentity);
            var header = renewalHeaderRepository.Get(model.ID);
            var source = new RenewalSubmissionVariableSource { CompanyName = identity.CompanyName, Year = header.Year, Quarter = header.Quarter };
            notificationService.Notify(identity.CompanyID, MTACompany.ID, LookupConstants.Notifications.RenewalSubmitted, source);
        }
        
        void UpdateRenewalDetails(RenewalDetailsViewModel model, string statusCode)
        {
            var header = renewalRepository.Get(model.ID);
            header.StatusID = lookupRepository.Get<LookupRenewalHeaderStatus>(statusCode).ID;
            long generatedId = lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Generated).ID;
            var isSubmitted = statusCode == LookupConstants.RenewalHeaderStatus.Submitted;
            foreach (var detail in header.RenewalDetails)
            {
                //var modelDetail = model.Details.SingleOrDefault(p => p.ID == detail.ID);
                //if (modelDetail == null) continue;
                //detail.IsSubmitted = isSubmitted && (modelDetail.RenewalStatus.ID != generatedId);
                //detail.StatusID = modelDetail.RenewalStatus.ID;
                //detail.Remarks = modelDetail.Remarks;

                //// detail.Description = modelDetail.Description;

                var modelDetail = model.RenewalDetail.SingleOrDefault(p => p.ID == detail.ID);
                if (modelDetail == null) continue;
                detail.StatusID = modelDetail.RenewalStatus.ID;
                detail.Remarks = modelDetail.Remarks;
                detail.IsSubmitted = isSubmitted && (modelDetail.RenewalStatus.ID != generatedId);
            }
            renewalRepository.SaveChanges();
        }


    }// class
}// namespace
