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
using MTAoarsGeneral.Mappers.Operations;
using MTAoarsGeneral.ViewModels.Enquiries;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Mvc;
using System.Data.Objects.DataClasses;
using System.Globalization;

namespace MTAoarsGeneral.Services.Operations
{
    public class AgencyService : BaseService, IAgencyService
    {

        IAgencyUnitOfWork agencyUnitOfWork;
        INotificaitonService notificationService;
        ILookupRepository lookupRepository;
        IRenewalRepository renewalRepository;
        AgencyPrincipalCreator principalCreator;
        IAgencyRepository agencyRepository;
        IRepository<EnquiryLog> enquiryLogRepository;
        IRunnerRepository runnerRepository;
        ConfigManager config;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public AgencyService(IValidationProvider validationProvider, IAgencyUnitOfWork agencyUnitOfWork,
            INotificaitonService notificationService, ILookupRepository lookupRepository, IRenewalRepository renewalRepository,
            AgencyPrincipalCreator principalCreator, IAgencyRepository agencyRepository, IRepository<EnquiryLog> enquiryLogRepository, IRunnerRepository runnerRepository, ConfigManager config)
            : base(validationProvider)
        {
            this.agencyUnitOfWork = agencyUnitOfWork;
            this.notificationService = notificationService;
            this.lookupRepository = lookupRepository;
            this.renewalRepository = renewalRepository;
            this.principalCreator = principalCreator;
            this.agencyRepository = agencyRepository;
            this.enquiryLogRepository = enquiryLogRepository;
            this.runnerRepository = runnerRepository;
            this.config = config;
        }

        public bool Terminate(long principalId, string remarks, DateTime? scheduledOn, bool isAutoTerminate, bool isBatchProcess)
        {
            //if (isBatchProcess) return Terminate(principalId, scheduledOn);
            logger.Info("-------------------------------------------------");
            logger.Info("AgencyService.Terminate");
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                if (principal != null)
                {
                    logger.Info(string.Format("AgencyPrincipal ID: {0}", principal.ID));
                    logger.Info("Start Terminate...");
                    Terminate(principalId, remarks, scheduledOn, isAutoTerminate);
                    if (!isBatchProcess)
                    {
                        logger.Info("Start UpdateRenewal...");
                        UpdateRenewal(principal);

                        logger.Info("Start Notification...");
                        var source = new TerminatedVariableSource { AgencyNumber = principal.AgencyNumber };
                        notificationService.Notify(MTACompany.ID, principal.CompanyID, LookupConstants.Notifications.Terminated, source);
                    }
                }
                scope.Complete();
            }
            return true;
        }
        public bool Terminate(long principalId, DateTime? scheduledOn, bool isAutoTerminate, bool isBatchProcess)
        {
            return Terminate(principalId, string.Empty, scheduledOn, isAutoTerminate, isBatchProcess);
        }

        public bool Renew(long principalId, bool isBatchProcess)
        {
            //if (isBatchProcess) return Renew(principalId);
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                if (principal != null)
                {
                    Renew(principalId);
                    if (!isBatchProcess)
                    {
                        UpdateRenewal(principal);
                        var source = new RenewedVariableSource { AgencyNumber = principal.AgencyNumber, ValidToDate = principal.ValidTo.Value };
                        notificationService.Notify(MTACompany.ID, principal.CompanyID, LookupConstants.Notifications.Renewed, source);
                    }
                }
                scope.Complete();
            }
            return true;
        }

        public bool NotRelease(long principalId, string remarks, DateTime? scheduledOn, bool isBatchProcess)
        {
            //if (isBatchProcess) return NotRelease(principalId);
            using (var scope = new TransactionScope())
            {
                var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                if (principal != null)
                {
                    NotRelease(principalId, remarks, scheduledOn);
                    if (!isBatchProcess)
                    {
                        UpdateRenewal(principal);
                        var source = new NotReleasedVariableSource { AgencyNumber = principal.AgencyNumber, StartDate = principal.ValidTo.Value.AddDays(1) };
                        notificationService.Notify(MTACompany.ID, principal.CompanyID, LookupConstants.Notifications.NotReleased, source);
                    }
                }
                scope.Complete();
            }
            return true;
        }
        public bool NotRelease(long principalId, DateTime? scheduledOn, bool isBatchProces)
        {
            return NotRelease(principalId, string.Empty, scheduledOn, isBatchProces);
        }
        public bool Resign(long principalId, string remarks, DateTime? scheduledOn, bool isBatchProcess)
        {
            using (var scope = new TransactionScope())
            {
                var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                if (principal != null)
                {
                    Resign(principalId, remarks, scheduledOn);
                    if (!isBatchProcess)
                    {
                        UpdateRenewal(principal);
                        var source = new ResignedVariableSource { AgencyNumber = principal.AgencyNumber, StartDate = principal.ValidTo.Value.AddDays(1) };
                        notificationService.Notify(MTACompany.ID, principal.CompanyID, LookupConstants.Notifications.Resigned, source);
                    }
                }
                scope.Complete();
            }
            return true;
        }


        public bool Resign(long principalId, DateTime? scheduledOn, bool isBatchProcess)
        {
            return Resign(principalId, string.Empty, scheduledOn, isBatchProcess);
        }
        public bool Terminate(long principalId, string remarks, DateTime? scheduledOn, bool isAutoTerminate)
        {
            var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
            var history = Mapper.Map<AgencyPrincipalHistory>(principal);
            history.Remarks = remarks;
            history.TerminationDate = scheduledOn;
            history.IsAutoTerminate = isAutoTerminate;
            history.StatusID = lookupRepository.Get<LookupTerminationAction>(LookupConstants.TerminationAction.Terminate).ID;
            agencyUnitOfWork.AgencyPrincipalHistoryRepository.Save(history);
            foreach (var guarantor in principal.AgencyPrincipalGuarantors.ToList())
            {
                agencyUnitOfWork.AgencyPrincipalGuarantorRepository.Delete(guarantor);
            }
            foreach (var status in principal.AgencyPrincipalStatus.ToList())
            {
                agencyUnitOfWork.AgencyPrincipalStatusRepository.Delete(status);
            }
            var conflict = agencyUnitOfWork.ConflictRepository.GetNotClosedAndNotRejectedConflict(principal.AgencyID, principal.CompanyID);
            AgencyPrincipal newAgencyPrincipal = null;
            if (conflict != null)
            {
                if (agencyUnitOfWork.ConflictRepository.IsAgencyPrincipalExists(conflict.AgencyID, conflict.DestinationCompanyID, conflict.IntermediaryTypeID) == false)
                {
                    newAgencyPrincipal = principalCreator.GetPrincipal(conflict);
                    principal.Agency.AgencyPrincipals.Add(newAgencyPrincipal);
                    conflict.StatusID = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Closed).ID;
                }
            }
            agencyUnitOfWork.AgencyPrincipalRepository.Delete(principal);
            agencyUnitOfWork.Save();

            /* Add record to Journal [20191010] ------------------------------------------- */
            if (newAgencyPrincipal != null && newAgencyPrincipal.Agency.ID > 0)
            {
                var uri = String.Format("/Registration/Complete/{0}", newAgencyPrincipal.Agency.ID);
                var journal = GetJournal(newAgencyPrincipal.ID, LookupConstants.Activities.NewRegistration, uri);
                agencyUnitOfWork.JournalRepository.Save(journal);
                agencyUnitOfWork.Save();
            }

            return true;
        }

        public bool Renew(long principalId)
        {
            logger.Info("-------------------------------------------------");
            logger.Info("Method Name: AgencyService.Renew");
            logger.Info("Principal ID: " + principalId);

            var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
            principal.ValidFrom = principal.ValidTo.Value.AddDays(1);
            principal.ValidTo = principal.ValidTo.Value.AddYears(2);
            var uri = String.Format("/Agency/Renew/{0}", principalId);
            var journal = GetJournal(principalId, LookupConstants.Activities.Renewal, uri);
            agencyUnitOfWork.JournalRepository.Save(journal);
            agencyUnitOfWork.Save();

            logger.Info("Journal ID: {0}".FormatWith(journal.ID));

            return true;
        }


        public void ReinstateTermination(long agencyID, long companyID, long intermediaryTypeID)
        {

            //var aph = agencyUnitOfWork.AgencyRepository.GetFullReinstateHistory(agencyNumber);
            //var agency = agencyUnitOfWork.AgencyRepository.Get(aph.AgencyID);
            var agency = agencyUnitOfWork.AgencyRepository.Get(agencyID);
            var latestAph = agency.AgencyPrincipalHistories.OrderByDescending(p => p.TerminationDate).FirstOrDefault(p => p.CompanyID == companyID);
            var aph = agencyUnitOfWork.AgencyRepository.GetReinstateHistory(agencyID, companyID, intermediaryTypeID, latestAph.LookupAgencyType.ID);
            var ap = Mapper.Map<AgencyPrincipal>(aph);
            aph.AgencyPrincipalGuarantorHistories.ToList().Each(apgh =>
            {
                ap.AgencyPrincipalGuarantors.Add(Mapper.Map<AgencyPrincipalGuarantor>(apgh));
                agencyUnitOfWork.AgencyPrincipalGuarantorHistoryRepository.Delete(apgh);
            });

            /* Reinstate not bring back AgencyPrincipalStatus from history - [2018-10-22] */
            aph.AgencyPrincipalStatusHistories.ToList().Each(apsh =>
            {
                //ap.AgencyPrincipalStatus.Add(Mapper.Map<AgencyPrincipalStatus>(apsh));
                agencyUnitOfWork.AgencyPrincipalStatusHistoryRepository.Delete(apsh);
            });

            //reinstate only brings back the agent
            //ap.ValidFrom = DateTime.Now;
            //ap.ValidTo = DateTime.Now.GetCurrentQuarterEndDate().AddYears(2);
            agency.AgencyPrincipals.Add(ap);
            agencyUnitOfWork.AgencyPrincipalHistoryRepository.Delete(aph);
            agencyUnitOfWork.Save();
        }


        /* This method being use for action "Reappoint" */
        public void RenewTermination(long agencyID, long companyID, long intermediaryTypeID, long agencyPrincipalID)
        {
            logger.Info("---------------------------------------------------------------------------------");
            logger.Info("Method Name: AgencyService.RenewTermination");
            logger.Info("Agency Principal ID: " + agencyPrincipalID);

            var agency = agencyUnitOfWork.AgencyRepository.Get(agencyID);
            var aph = agencyUnitOfWork.AgencyRepository.GetReinstateHistory(
                agencyID, companyID, intermediaryTypeID, agency.AgencyPrincipalHistories.OrderByDescending(p => p.TerminationDate).FirstOrDefault(p => p.CompanyID == companyID).LookupAgencyType.ID);

            /* [20190926] Add in AgencyPrincipalID to show correct information in Invoice */
            if (agencyPrincipalID > 0)
                aph = agencyUnitOfWork.AgencyPrincipalHistoryRepository.Get(agencyPrincipalID);

            var ap = Mapper.Map<AgencyPrincipal>(aph);
            aph.AgencyPrincipalGuarantorHistories.ToList().Each(apgh =>
            {
                ap.AgencyPrincipalGuarantors.Add(Mapper.Map<AgencyPrincipalGuarantor>(apgh));
                //agencyUnitOfWork.AgencyPrincipalGuarantorHistoryRepository.Delete(apgh);
            });
            aph.AgencyPrincipalStatusHistories.ToList().Each(apsh =>
            {
                ap.AgencyPrincipalStatus.Add(Mapper.Map<AgencyPrincipalStatus>(apsh));
                //agencyUnitOfWork.AgencyPrincipalStatusHistoryRepository.Delete(apsh);
            });

            ap.ValidFrom = DateTime.Now;
            ap.ValidTo = DateTime.Now.GetCurrentQuarterEndDate().AddYears(2);
            ap.AgencyNumber = runnerRepository.GetNext(LookupConstants.Runner.AgencyRegistrationNumber);
            agency.AgencyPrincipals.Add(ap);

            //agencyUnitOfWork.AgencyPrincipalHistoryRepository.Delete(aph);
            agencyUnitOfWork.Save();
            logger.Info("[REAPPOINT] Agency Principal History ID: {0}, New Agency Principal ID: {1}, New Agency Number: {2}".FormatWith(aph.ID, ap.ID, ap.AgencyNumber));

            //When renew terminated agent need to add record to Journal [2019-05-21]
            logger.Info("Add record to Journal table...");
            var uri = String.Format("/Registration/Complete/{0}", agency.ID);
            var journal = GetJournal(ap.ID, LookupConstants.Activities.NewRegistration, uri);
            agencyUnitOfWork.JournalRepository.Save(journal);
            agencyUnitOfWork.Save();
            logger.Info(string.Format("Journal ID: {0}", journal.ID));

            //Renew(ap.ID);
        }


        public bool NotRelease(long principalId, string remarks, DateTime? scheduledOn)
        {
            var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
            var apStatus = new AgencyPrincipalStatus
            {
                // FromDate = principal.ValidTo.Value.AddDays(1),
                FromDate = scheduledOn.HasValue ? scheduledOn.Value : DateTime.Now,
                StatusID = lookupRepository.Get<LookupAgencyPrincipalStatus>(LookupConstants.AgencyPrincipalStatus.NotReleased).ID
            };
            var conflict = agencyUnitOfWork.ConflictRepository.GetNotClosedAndNotRejectedConflict(principal.AgencyID, principal.CompanyID);
            if (conflict != null)
            {
                if (agencyUnitOfWork.ConflictRepository.IsAgencyPrincipalExists(conflict.AgencyID,
                                   conflict.DestinationCompanyID,
                                   conflict.IntermediaryTypeID) == false)
                {
                    conflict.StatusID = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Rejected).ID;
                    var source = new ConflictNotReleasedVariableSource { AgencyNumber = principal.AgencyNumber, CompanyName = conflict.SourceCompany.Name };
                    notificationService.Notify(MTACompany.ID, conflict.DestinationCompanyID, LookupConstants.Notifications.ConflictNotReleased, source);
                }

            }
            apStatus.Remarks = remarks;
            principal.AgencyPrincipalStatus.Add(apStatus);
            principal.Remarks = remarks;
            agencyUnitOfWork.Save();
            return true;
        }
        public bool Resign(long principalId, string remarks, DateTime? scheduledOn)
        {
            var principal = agencyUnitOfWork.AgencyPrincipalRepository.Get(principalId);
            var history = Mapper.Map<AgencyPrincipalHistory>(principal);
            history.TerminationDate = scheduledOn;
            history.Remarks = remarks;
            history.StatusID = lookupRepository.Get<LookupTerminationAction>(LookupConstants.TerminationAction.Resign).ID;
            agencyUnitOfWork.AgencyPrincipalHistoryRepository.Save(history);
            foreach (var guarantor in principal.AgencyPrincipalGuarantors.ToList())
            {
                agencyUnitOfWork.AgencyPrincipalGuarantorRepository.Delete(guarantor);
            }
            foreach (var status in principal.AgencyPrincipalStatus.ToList())
            {
                agencyUnitOfWork.AgencyPrincipalStatusRepository.Delete(status);
            }
            var conflict = agencyUnitOfWork.ConflictRepository.GetNotClosedAndNotRejectedConflict(principal.AgencyID, principal.CompanyID);
            if (conflict != null)
            {
                if (agencyUnitOfWork.ConflictRepository.IsAgencyPrincipalExists(conflict.AgencyID,
                                    conflict.DestinationCompanyID,
                                    conflict.IntermediaryTypeID) == false)
                {
                    principal.Agency.AgencyPrincipals.Add(principalCreator.GetPrincipal(conflict));
                    conflict.StatusID = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Closed).ID;
                }
            }


            agencyUnitOfWork.AgencyPrincipalRepository.Delete(principal);
            agencyUnitOfWork.Save();
            return true;

        }
        void UpdateRenewal(AgencyPrincipal principal)
        {
            var rd = renewalRepository.GetInProcessDetail(principal.AgencyID, principal.CompanyID, principal.IntermediaryTypeID);
            if (rd == null) return;
            rd.IsProcessed = true;
            rd.IsManual = true;
            renewalRepository.SaveChanges();
        }

        public List<SearchResultViewModel> Enquiry(SearchViewModel search)
        {
            var aps = agencyRepository.Enquiry(search.RegistrationNumber, search.ICNumber, search.Company.ID);
            var output = new List<SearchResultViewModel>();
            var result = Mapper.Map<SearchResultViewModel>(search);

            if (aps.Count == 0) output.Add(result);

            foreach (var ap in aps)
            {
                result = Mapper.Map<SearchResultViewModel>(search);
                result.ValidTo = ap.ValidTo.Value;
                result.Rating = ap.Rating;
                if (!String.IsNullOrEmpty(ap.AgencyNumber)) result.RegistrationNumber = ap.AgencyNumber;
                result.IntermediaryType = ap.LookupIntermediaryType.Description;
                result.Company = ap.Company.Name;
                var member = ap.Agency.AgencyMembers.Where(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee).First().Member;
                //if (!String.IsNullOrEmpty(search.ICNumber)) result.ICNumber = member.NewICNumber;

                if (ap.LookupAgencyType.Code == LookupConstants.AgencyType.Individual)
                {
                    result.AgentName = member.Name;
                }
                else
                {
                    result.CorporateNominee = member.Name;
                    result.AgentName = ap.Agency.Name;
                }
                result.SocialMediaAddress = ap.Agency.SocialMediaAddress;
                var mtaAwards = GetLookupList<LookupMTAAward>(ap.Agency.MTAAwards);
                if (mtaAwards != null)
                {
                    result.MTAAwards = string.Join(",", mtaAwards.Select(x => x.Description).ToList());
                }
                result.JoinMonthYear = $@"{ap.Agency.JoinMonth?.ToUpper()}/{ap.Agency.JoinYear}";

                result.M2Exam = GetM2Exams(ap.Agency.M2Exam);

                result.Photo = string.IsNullOrEmpty(ap.PhotoPath) ? "" : ap.PhotoPath.Replace(config.UploadBaseDirectory + @"\Photo\", config.PhotoUrl);
                output.Add(result);
            }
            InsertEnquiryLog(search, aps.Count);

            return output;
        }
        LookupItem GetM2Exams(string value)
        {
            var list = new List<LookupItem>{   new LookupItem { ID = 1, Code = "MFPC", Description = "Malaysian Financial Planning Council (MFPC)" },
           new LookupItem { ID = 2, Code = "FPAM", Description = "Financial Planning Association of Malaysia (FPAM)" },
            new LookupItem { ID = 3, Code = "Exempted", Description = "Exempted" },
            new LookupItem { ID = 4, Code = "NotYetCompleted", Description = "Not yet completed" }
        };

            return list.FirstOrDefault(x => x.Code.ToUpper() == value?.ToUpper());

        }
        List<LookupItem> GetLookupList<T>(string code) where T : EntityObject, ILookupEntity
        {
            List<LookupItem> items = new List<LookupItem>();
            if (!string.IsNullOrEmpty(code))
            {
                var modelItems = agencyUnitOfWork.LookupRepository.GetAll<T>().Where(x => code.Split(';').Contains(x.Code)).ToList();
                modelItems.ForEach(x => items.Add(new LookupItem { Code = x.Code, Description = x.Description, ID = x.ID }));
            }
            return items;
        }
        void InsertEnquiryLog(SearchViewModel search, int total)
        {
            var log = Mapper.Map<EnquiryLog>(search);
            log.CreatedDate = DateTime.Now;
            log.ModifiedDate = DateTime.Now;
            log.Total = Convert.ToInt16(total);
            enquiryLogRepository.Save(log);
            enquiryLogRepository.SaveChanges();
        }

    }// class
}// namespace
