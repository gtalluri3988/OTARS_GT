using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;
using AutoMapper;
using System.Transactions;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Services.Operations
{
    public class TerminationService : BaseService, ITerminationService
    {

        AgencyService agencyService;
        IRepository<TerminationSchedule> terminationScheduleRepository;
        ILookupRepository lookupRepository;
        INotificaitonService notificationService;
        IRenewalRepository renewalRepository;
        readonly long generalIntermediaryTypeId;

        public TerminationService(IValidationProvider validationProvider, AgencyService agencyService, IRepository<TerminationSchedule> terminationScheduleRepository, ILookupRepository lookupRepository, INotificaitonService notificationService, IRenewalRepository renewalRepository)
            : base(validationProvider)
        {
            this.agencyService = agencyService;
            this.terminationScheduleRepository = terminationScheduleRepository;
            this.lookupRepository = lookupRepository;
            this.notificationService = notificationService;
            this.renewalRepository = renewalRepository;
            generalIntermediaryTypeId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
        }

        public void Process(IEnumerable<TerminationSearchResponseViewModel> list)
        {
            var mailCompanies = new Dictionary<long, BatchTerminationVariableSource>();
            using (var scope = new TransactionScope())
            {
                foreach (var item in list)
                {
                    if (item.IsValid == false) continue;
                    var action = lookupRepository.Get<LookupTerminationAction>(item.ActionID.Value);
                    if (item.ScheduledOn < DateTime.Now.AddDays(1) || action.Code == LookupConstants.TerminationAction.NotRelease)
                    {

                        Process(item);
                        if (action.Code != LookupConstants.TerminationAction.NotRelease)
                        {
                            if (!mailCompanies.ContainsKey(item.CompanyID)) mailCompanies.Add(item.CompanyID, new BatchTerminationVariableSource());

                            mailCompanies[item.CompanyID].TerminationList.Add(new TerminatedVariableSource
                            {
                                AgencyName = item.NomineeName,
                                IntermediaryTypeDescription = item.IntermediaryTypeDescription,
                                AgencyNumber = item.AgencyNumber
                            });
                        }
                    }
                    else
                    {
                        var schedule = Mapper.Map<TerminationSchedule>(item);
                        terminationScheduleRepository.Save(schedule);
                    }
                }
                terminationScheduleRepository.SaveChanges();
                foreach (var key in mailCompanies.Keys)
                {
                    notificationService.Notify(MTACompany.ID, key, LookupConstants.Notifications.BatchTermination, mailCompanies[key]);
                }
                scope.Complete();
            }

        }

        void Process(TerminationSearchResponseViewModel item)
        {
            if (item.ActionID.HasValue == false) return;

            var action = lookupRepository.Get<LookupTerminationAction>(item.ActionID.Value);
            switch (action.Code)
            {
                case LookupConstants.TerminationAction.Terminate:

                    agencyService.Terminate(item.AgencyPrincipalID, item.Remarks, item.ScheduledOn, false, false);
                    break;
                case LookupConstants.TerminationAction.NotRelease:
                    agencyService.NotRelease(item.AgencyPrincipalID, item.Remarks, item.ScheduledOn, false);
                    break;
                case LookupConstants.TerminationAction.Resign:
                    agencyService.Resign(item.AgencyPrincipalID, item.Remarks, item.ScheduledOn, false);
                    break;
            }
        }

        public void Reinstate(IEnumerable<TerminationSearchResponseViewModel> list)
        {
            using (var scope = new TransactionScope())
            {
                foreach (var item in list)
                {
                    if (item.IsValid == false) continue;
                    Reinstate(item);
                }
                scope.Complete();
            }
        }

        void Reinstate(TerminationSearchResponseViewModel item)
        {
            if (item.IsValid) agencyService.ReinstateTermination(item.AgencyID, item.CompanyID, generalIntermediaryTypeId);
        }

        public void Renew(IEnumerable<TerminationSearchResponseViewModel> list)
        {
            using (var scope = new TransactionScope())
            {
                foreach (var item in list)
                {
                    if (item.IsValid == false) continue;
                    Renew(item);
                }
                scope.Complete();
            }
        }
        void Renew(TerminationSearchResponseViewModel item)
        {
            if (item.IsValid)
                agencyService.RenewTermination(item.AgencyID, item.CompanyID, generalIntermediaryTypeId, item.AgencyPrincipalID);
        }

        public void AddIntoRenewalDetail(List<TerminationSearchResponseViewModel> terminationAgents)
        {
            using (var scope = new TransactionScope())
            {
                foreach (var item in terminationAgents)
                {
                    if (item.IsValid == false) continue;
                    AddIntoRenewalDetail(item);
                }
                scope.Complete();
            }

        }

        void AddIntoRenewalDetail(TerminationSearchResponseViewModel terminationAgent)
        {
            string To_Date = string.Empty;
            var detailStatus = lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Generated);
            List<string> lstDate = terminationAgent.ValidToLabel != null ? terminationAgent.ValidToLabel.Split('/').ToList() : new List<string>();
            if (lstDate.Count > 0)
            {
                To_Date = lstDate[1] + "/" + lstDate[0] + "/" + lstDate[2];
            }
            var validTo = Convert.ToDateTime(To_Date);


            var principals = renewalRepository.GetRenewableAgencies(terminationAgent.CompanyID, validTo);
            var principal = principals.FirstOrDefault(x => x.AgencyNumber == terminationAgent.AgencyNumber);
            var header = renewalRepository.Get(terminationAgent.CompanyID, validTo.Year, validTo.GetQuarter());
            ////GetGeneratedRenewalHeaders(terminationAgent.CompanyID).OrderByDescending(x => x.Year).ThenByDescending(y => y.Quarter).FirstOrDefault();

            if (header == null)
            {
                header = GetHeader(terminationAgent.CompanyID, validTo, principal);
                renewalRepository.Save(header);
            }
            else
            {

                var detail = new RenewalDetail
                {
                    AgencyID = principal.AgencyID,
                    StatusID = detailStatus.ID,
                    IntermediaryTypeID = generalIntermediaryTypeId,
                    AgencyPrincipalID = principal.ID, /* [20190926] Add in AgencyPrincipalID to show correct information in Invoice */
                    AgencyNumber = principal.AgencyNumber, /* [20191121] - Do not use AgencyPrincipalID in table dbo.Journal due to it is not unique, instead use AgencyNumber */
                    TypeID = principal.TypeID, /* [20200115] Usage on SQL View [RenewalViewDetail] */
                    //RecordVersion = ;
                    IsProcessed = false

                };
                header.RenewalDetails.Add(detail);

            }



            renewalRepository.SaveChanges();
        }

        RenewalHeader GetHeader(long companyId, DateTime validTo, AgencyPrincipal principal)
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

            return header;
        }

    }// class
}// namespace
