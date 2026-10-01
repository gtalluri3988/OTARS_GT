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

namespace MTAoarsGeneral.Services.Operations {
    public class CBCService : BaseService, ICBCService {
        INotificaitonService notificationService;
        ICBCRepository cbcRepository;
        IAgencyService agencyService;
        ILookupRepository lookupRepository;
        IAgencyRepository agencyRepository;
        IRepository<CBCDetail> detailRepository;
        IRepository<AgencyStatus> agencyStatusRepository;
        long agencySuspended, agencyTerminated, noAction, suspended, terminated;

        public CBCService(IValidationProvider validationProvider, INotificaitonService notificationService, 
            ICBCRepository cbcRepository, ILookupRepository lookupRepository, IAgencyService agencyService, 
            IAgencyRepository agencyRepository, IRepository<CBCDetail> detailRepository,
            IRepository<AgencyStatus> agencyStatusRepository)
            : base(validationProvider) {
            this.notificationService = notificationService;
            this.cbcRepository = cbcRepository;
            this.lookupRepository = lookupRepository;
            this.agencyService = agencyService;
            this.agencyRepository = agencyRepository;
            this.detailRepository = detailRepository;
            this.agencyStatusRepository = agencyStatusRepository;
            var agencyStatuses = lookupRepository.GetAll<LookupAgencyStatus>();
            var detailStatuses = lookupRepository.GetAll<LookupCBCDetailStatus>();
            agencySuspended = agencyStatuses.First(p => p.Code == LookupConstants.AgencyStatus.CBCSuspended).ID;
            agencyTerminated = agencyStatuses.First(p => p.Code == LookupConstants.AgencyStatus.CBCTerminated).ID;
            noAction = detailStatuses.First(p => p.Code == LookupConstants.CBCDetailStatus.NoAction).ID;
            suspended = detailStatuses.First(p => p.Code == LookupConstants.CBCDetailStatus.Suspended).ID;
            terminated = detailStatuses.First(p => p.Code == LookupConstants.CBCDetailStatus.Terminated).ID;
        }

        public void Process() {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, GlobalConstants.HugeTransactionTimeSpan)) {
                var date = DateTime.Now.GetPreviousQuarterEndDate();
                var year = date.Year;
                var quarter = date.GetQuarter();
                var list = cbcRepository.GetPivot(year, quarter);
                var lastTermList = cbcRepository.GetDetails((year * 4) + quarter - 3);
                 var mailCompanies = new Dictionary<long, CBCProcessVariableSource>();
                foreach (var item in list) {
                    if (item.First && item.Second && item.Third) {
                        Suspend(item, mailCompanies);
                        ChangeDetailStatus(year, quarter, item.AgencyID, suspended);
                    } else if (item.Third && lastTermList.Count(p => p.StatusID == suspended && p.AgencyID == item.AgencyID) > 0) {
                        Terminate(item, mailCompanies);
                        ChangeDetailStatus(year, quarter, item.AgencyID, terminated);
                    } else {
                        ChangeDetailStatus(year, quarter, item.AgencyID, noAction);
                    }
                }
                agencyStatusRepository.SaveChanges();
                cbcRepository.SaveChanges();
                foreach (var key in mailCompanies.Keys) {
                    notificationService.Notify(MTACompany.ID, key, LookupConstants.Notifications.CBCProcessed, mailCompanies[key]);
                }
                scope.Complete();
            }
        }

        public bool Check(CBCStartViewModel model) {
            return Validate(model);
        }

        public CBCHeaderViewModel GetHeader(CBCStartViewModel model) {
            var year = Convert.ToInt32(model.Year.ID);
            var quarter = Convert.ToInt32(model.Quarter.ID);
            var header = cbcRepository.Get(year, quarter);
            if (header == null) header = Save(year, quarter, model.Company.ID);
            return Mapper.Map<CBCHeaderViewModel>(header);
        }

        public CBCDetailViewModel Add(long headerId, string agencyNumber) {
            var header = cbcRepository.Get(headerId);
            var agency = agencyRepository.Search(agencyNumber);
            if (agency == null) return null;
            var detail = header.CBCDetails.FirstOrDefault(p => p.AgencyID == agency.AgencyID 
                && p.IntermediaryTypeID == agency.IntermediaryTypeID);
            if (detail != null) {
                CurrentContext.ValidationMessages.Add(new ValidationMessage("", "This agent is alreaded added in breach list for the selected year & quarter"));
                return null;
            }
            var item = new CBCDetail {
                HeaderID = headerId, IntermediaryTypeID = agency.IntermediaryTypeID.Value, AgencyID = agency.AgencyID,
                StatusID = lookupRepository.Get<LookupCBCDetailStatus>(LookupConstants.CBCDetailStatus.Generated).ID
            };
            detailRepository.Save(item);
            detailRepository.SaveChanges();
            var output = Mapper.Map<CBCDetailViewModel>(agency);
            output.ID = item.ID;
            return output;
        }

        public void ChangeHeaderStatus(long headerId, string newStatusCode) {
            var header = cbcRepository.Get(headerId);
            header.StatusID = lookupRepository.Get<LookupCBCHeaderStatus>(newStatusCode).ID;
            cbcRepository.SaveChanges();
        }


        CBCHeader Save(int year, int quarter, long companyId) {
            var statusId = lookupRepository.Get<LookupCBCHeaderStatus>(LookupConstants.CBCHeaderStatus.Saved).ID;
            var header = new CBCHeader {
                Year = year, Quarter = quarter, CompanyID = companyId, StatusID = statusId
            };
            cbcRepository.Save(header);
            cbcRepository.SaveChanges();
            return header;
        }

        void Suspend(CBCPivotDetail detail, Dictionary<long, CBCProcessVariableSource> mailCompanies) {
            var agencyStatus = new AgencyStatus {
                AgencyID = detail.AgencyID, StatusID = agencySuspended, FromDate = DateTime.Now, ToDate = DateTime.Now.AddYears(6),
                IsActive = true
            };
            agencyStatusRepository.Save(agencyStatus);
            var ag = agencyRepository.Get(detail.AgencyID);
            foreach (var principal in ag.AgencyPrincipals) {
                if (!mailCompanies.ContainsKey(principal.CompanyID)) mailCompanies.Add(principal.CompanyID, new CBCProcessVariableSource());
                mailCompanies[principal.CompanyID].SuspensionList.Add(new TerminatedVariableSource {
                    AgencyName = principal.Agency.Name, AgencyNumber = principal.AgencyNumber,
                    IntermediaryTypeDescription = principal.LookupIntermediaryType.Description
                });
            }
           
        }

        void Terminate(CBCPivotDetail detail, Dictionary<long, CBCProcessVariableSource> mailCompanies) {
            var ag = agencyRepository.Get(detail.AgencyID);
            foreach (var principal in ag.AgencyPrincipals) {
                agencyService.Terminate(principal.ID, DateTime.Now, true);
                if (!mailCompanies.ContainsKey(principal.CompanyID)) mailCompanies.Add(principal.CompanyID, new CBCProcessVariableSource());
                mailCompanies[principal.CompanyID].TerminationList.Add(new TerminatedVariableSource {
                    AgencyName = principal.Agency.Name, AgencyNumber = principal.AgencyNumber,
                    IntermediaryTypeDescription = principal.LookupIntermediaryType.Description
                });
            }
            var agencyStatus = new AgencyStatus {
                AgencyID = detail.AgencyID, StatusID = agencyTerminated, FromDate = DateTime.Now, ToDate = DateTime.Now.AddYears(6),
                IsActive = true
            };
            agencyStatusRepository.Save(agencyStatus);
        }

        void ChangeDetailStatus(int year, int quarter, long agencyId, long statusId) {
             foreach (var item in cbcRepository.GetDetails(year, quarter, agencyId)) {
                 item.StatusID = statusId;
            }
        }

    }// class
}// namespace
