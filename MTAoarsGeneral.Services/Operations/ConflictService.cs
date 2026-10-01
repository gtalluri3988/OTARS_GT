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
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Services.Operations {
    public class ConflictService : BaseService, IConflictService {

        INotificaitonService notificationService;
        ILookupRepository lookupRepository;
        IConflictRepository conflictRepository;
        IAgencyService agencyService;
        ConfigManager config;
        IRepository<ConflictAttachment> attachmentRepository;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        public ConflictService(IValidationProvider validationProvider, INotificaitonService notificationService, 
            ILookupRepository lookupRepository, IConflictRepository conflictRepository, ConfigManager config,
            IAgencyService  agencyService, IRepository<ConflictAttachment> attachmentRepository)
            : base(validationProvider) {
            this.notificationService = notificationService;
            this.lookupRepository = lookupRepository;
            this.conflictRepository = conflictRepository;
            this.config = config;
            this.agencyService = agencyService;
            this.attachmentRepository = attachmentRepository;
        }

        public void SendFirstReminders() {
            logger.Info("-----------------------------------------------------------------------------");
            logger.Info("ConflictService.SendFirstReminders");

            try
            {
                var list = conflictRepository.SelectOpenConflicts(config.ConflictFirstReminderDuration)
                        .Where(p => p.IsFirstReminderSent == false);
                logger.Info(string.Format("List count: {0}", list.Count()));
                foreach (var item in list)
                {
                    logger.Info(string.Format("AgencyPrincipalConflict ID: {0}", item.ID));
                    var source = GetVariableSource(item);
                    item.IsFirstReminderSent = true;
                    logger.Info("SourceConflictFirstReminder");
                    notificationService.Notify(MTACompany.ID, item.SourceCompanyID.Value, LookupConstants.Notifications.SourceConflictFirstReminder, source);
                    logger.Info("DestinationConflictFirstReminder");
                    notificationService.Notify(MTACompany.ID, item.DestinationCompanyID, LookupConstants.Notifications.DestinationConflictFirstReminder, source);
                    logger.Info("Done");
                }
                conflictRepository.SaveChanges();
                logger.Info("Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void SendSecondReminders() {
            logger.Info("-----------------------------------------------------------------------------");
            logger.Info("ConflictService.SendSecondReminders");

            try
            {
                var list = conflictRepository.SelectOpenConflicts(config.ConflictSecondReminderDuration)
                        .Where(p => p.IsSecondReminderSent == false);
                logger.Info(string.Format("List count: {0}", list.Count()));
                foreach (var item in list)
                {
                    logger.Info(string.Format("AgencyPrincipalConflict ID: {0}", item.ID));
                    var source = GetVariableSource(item);
                    item.IsSecondReminderSent = true;
                    logger.Info("SourceConflictSecondReminder");
                    notificationService.Notify(MTACompany.ID, item.SourceCompanyID.Value, LookupConstants.Notifications.SourceConflictSecondReminder, source);
                    logger.Info("DestinationConflictSecondReminder");
                    notificationService.Notify(MTACompany.ID, item.DestinationCompanyID, LookupConstants.Notifications.DestinationConflictSecondReminder, source);
                    logger.Info("Done");
                }
                conflictRepository.SaveChanges();
                logger.Info("Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void DefaultOpenCases() {
            logger.Info("-----------------------------------------------------------------------------");
            logger.Info("ConflictService.DefaultOpenCases");

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan))
                {
                    var list = conflictRepository.SelectOpenConflicts(config.ConflictThirdReminderDuration);
                    var closeId = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Defaulted).ID;
                    logger.Info(string.Format("List count: {0}", list.Count()));
                    foreach (var item in list)
                    {
                        logger.Info(string.Format("AgencyPrincipalConflict ID: {0}", item.ID));
                        item.StatusID = closeId;
                        var source = GetVariableSource(item);
                        logger.Info("SourceConflictThirdReminder");
                        notificationService.Notify(MTACompany.ID, item.SourceCompanyID.Value, LookupConstants.Notifications.SourceConflictThirdReminder, source);
                        logger.Info("DestinationConflictThirdReminder");
                        notificationService.Notify(MTACompany.ID, item.DestinationCompanyID, LookupConstants.Notifications.DestinationConflictThirdReminder, source);
                        logger.Info("Done");
                    }
                    conflictRepository.SaveChanges();
                    scope.Complete();
                    logger.Info("Completed");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void AutoClose() {
            logger.Info("-----------------------------------------------------------------------------");
            logger.Info("ConflictService.AutoClose");

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan))
                {
                    var list = conflictRepository.SelectConflicts(config.ConflictAutoCloseDuration, LookupConstants.ConflictStatus.Defaulted);
                    var statusId = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.AutoClose).ID;
                    logger.Info(string.Format("List count: {0}", list.Count()));
                    foreach (var item in list)
                    {
                        logger.Info(string.Format("AgencyPrincipalConflict ID: {0}", item.ID));
                        var principal = conflictRepository.GetSourceCompanyPrincipal(item.ID);
                        logger.Info(string.Format("AgencyPrincipal ID: {0}", principal.ID));
                        if (principal != null) agencyService.Terminate(principal.ID, DateTime.Now, true);
                        item.StatusID = statusId;
                        var source = GetVariableSource(item);
                        notificationService.Notify(MTACompany.ID, item.DestinationCompanyID, LookupConstants.Notifications.DestinationConflictAutoClose, source);
                        logger.Info("Done");
                    }
                    conflictRepository.SaveChanges();
                    scope.Complete();
                    logger.Info("Completed");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void Close(long id) {
            logger.Info("-----------------------------------------------------------------------------");
            logger.Info("ConflictService.Close");

            try
            {
                using (var scope = new TransactionScope())
                {
                    logger.Info(string.Format("AgencyPrincipalConflict ID: {0}", id));
                    var conflict = conflictRepository.Get(id);
                    var principal = conflictRepository.GetSourceCompanyPrincipal(id);
                    logger.Info(string.Format("AgencyPrincipal ID: {0}", principal.ID));
                    agencyService.Terminate(principal.ID, DateTime.Now, false);
                    conflict.StatusID = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Closed).ID;
                    conflictRepository.SaveChanges();
                    scope.Complete();
                    logger.Info("Completed");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void Reject(long id) {
            logger.Info("-----------------------------------------------------------------------------");
            logger.Info("ConflictService.Reject");

            try
            {
                logger.Info(string.Format("AgencyPrincipalConflict ID: {0}", id));
                var conflict = conflictRepository.Get(id);
                conflict.StatusID = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Rejected).ID;
                conflictRepository.SaveChanges();
                logger.Info("Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public List<ConflictAttachmentViewModel> GetAttachments(long id) {
            var conflict = conflictRepository.Get(id);
            var output = new List<ConflictAttachmentViewModel>();
            foreach (var item in conflict.ConflictAttachments) {
                output.Add(Mapper.Map<ConflictAttachmentViewModel>(item));
            }
            return output;
        }

        public ConflictAttachmentViewModel GetAttachment(long attachmentId) {
            var attachment = attachmentRepository.Get(attachmentId);
            return Mapper.Map<ConflictAttachmentViewModel>(attachment);
        }

        ConflictVariableSource GetVariableSource(AgencyPrincipalConflict conflict) {

            var ap = conflictRepository.GetSourceCompanyPrincipal(conflict.ID);
            return new ConflictVariableSource {
                AgencyNumber = ap.AgencyNumber,
                Date = conflict.CreatedDate.Value.ToString(GlobalConstants.DateFormat),
                SourceCompany = conflict.SourceCompany.Name,
                DestinationCompany = conflict.DestinationCompany.Name,
                ICNumber = ap.Agency.AgencyMembers.First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee).Member.NewICNumber
            };
        }
    }// class
}// namespace
