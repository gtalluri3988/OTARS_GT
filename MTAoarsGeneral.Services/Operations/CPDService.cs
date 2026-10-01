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
using MTAoarsGeneral.ViewModels.Masters;

namespace MTAoarsGeneral.Services.Operations {
    public class CPDService : BaseService, ICPDService {
        INotificaitonService notificationService;
        ICPDRepository cpdRepository;
        IAgencyService agencyService;
        ILookupRepository lookupRepository;
        IAgencyUnitOfWork agencyUnitOfWork;
        long general, cpdTerminated, terminated, noaction;

        public CPDService(IValidationProvider validationProvider, IAgencyUnitOfWork agencyUnitOfWork, INotificaitonService notificationService, ICPDRepository cpdRepository, ILookupRepository lookupRepository, IAgencyService agencyService)
            : base(validationProvider) {
            this.notificationService = notificationService;
            this.cpdRepository = cpdRepository;
            this.lookupRepository = lookupRepository;
            this.agencyService = agencyService;
            this.agencyUnitOfWork = agencyUnitOfWork;
            general = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            cpdTerminated = lookupRepository.Get<LookupAgencyStatus>(LookupConstants.AgencyStatus.CPDTerminated).ID;
            terminated = lookupRepository.Get<LookupCPDStatus>(LookupConstants.CPDStatus.Terminated).ID;
            noaction = lookupRepository.Get<LookupCPDStatus>(LookupConstants.CPDStatus.NoAction).ID;
        }

        public bool Check(TrainingListViewModel model) {
            return Validate(model);
        }

        public void Process() {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, GlobalConstants.HugeTransactionTimeSpan)) {
                var year = DateTime.Now.Year - 1;
                cpdRepository.Process(year);
                var mailCompanies = new Dictionary<long, BatchRenewalVariableSource>();
                var details = cpdRepository.Get(year, LookupConstants.CPDStatus.Generated);
                foreach (var detail in details) {
                    if (CanTerminate(detail)) {
                        Terminate(detail, mailCompanies);
                        detail.StatusID = terminated;
                    } else {
                        detail.StatusID = noaction;
                    }
                }
                cpdRepository.SaveChanges();
                foreach (var key in mailCompanies.Keys) {
                    notificationService.Notify(MTACompany.ID, key, LookupConstants.Notifications.CPDProcessed, mailCompanies[key]);
                }
                scope.Complete();
            }
        }

        bool CanTerminate(CPDDetail detail) {
            if (detail.IntermediaryTypeID != general) return false;
            return CanTerminate(detail, 20);
        }

        bool CanTerminate(CPDDetail detail, double cutoff) {
            /*if (detail.TotalCreditHours < cutoff) return true;
            var nonTechnicalCutoff = (40d/100d) * cutoff;
            var nonTechnicalHours = detail.NonTechnicalCreditHours > nonTechnicalCutoff ? nonTechnicalCutoff : detail.NonTechnicalCreditHours;
            if ((detail.TechnicalCreditHours + nonTechnicalHours) < cutoff) return true;
            var dialogueCutoff = (12.5d/100d) * nonTechnicalCutoff;
            var dialogueHours = detail.NonTechnicalDialogueHours > dialogueCutoff ? dialogueCutoff : detail.NonTechnicalDialogueHours;
            var nonDialogueHours = detail.NonTechnicalCreditHours - detail.NonTechnicalDialogueHours;
            if ((detail.TechnicalCreditHours + dialogueHours + nonDialogueHours) < cutoff) return true;
            return false;*/
            return detail.IsAchieved;
        }

        void Terminate(CPDDetail detail, Dictionary<long, BatchRenewalVariableSource> mailCompanies) {
            var list = detail.Agency.AgencyPrincipals.Where(p => p.IntermediaryTypeID == general).ToList();
            foreach (var principal in list) {
                agencyService.Terminate(principal.ID, DateTime.Now, true);
                if (!mailCompanies.ContainsKey(principal.CompanyID)) mailCompanies.Add(principal.CompanyID, new BatchRenewalVariableSource());

                mailCompanies[principal.CompanyID].TerminationList.Add(new TerminatedVariableSource {
                    AgencyName = principal.Agency.Name, AgencyNumber = principal.AgencyNumber,
                    IntermediaryTypeDescription = principal.LookupIntermediaryType.Description
                });
            }

            var agencyStatus = new AgencyStatus {
                AgencyID = detail.AgencyID, StatusID = cpdTerminated, FromDate = DateTime.Now, ToDate = DateTime.Now.AddYears(1),
                IsActive = true
            };
            detail.Agency.AgencyStatus.Add(agencyStatus);
        }

    }// class
}// namespace
