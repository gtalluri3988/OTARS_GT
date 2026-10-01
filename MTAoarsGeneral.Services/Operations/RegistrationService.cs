using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Validators;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Shared;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Mappers.Operations;
using System.Transactions;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Utilities.Config;


namespace MTAoarsGeneral.Services.Operations
{
    public class RegistrationService : BaseService, IRegistrationService
    {
        IRegistrationUnitOfWork registrationUnitOfWork;
        ILookupService lookupService;
        IScopeDataProvider dataProvider;
        Identity currentIdentity;
        INotificaitonService notificationService;
        AgencyMemberCreator memberCreator;
        ConfigManager config;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RegistrationService(IValidationProvider validationProvider, IRegistrationUnitOfWork registrationUnitOfWork, ILookupService lookupService, IScopeDataProvider dataProvider, INotificaitonService notificationService, AgencyMemberCreator memberCreator, ConfigManager config)
            : base(validationProvider)
        {
            this.registrationUnitOfWork = registrationUnitOfWork;
            this.lookupService = lookupService; // new LookupService(registrationUnitOfWork.LookupRepository, registrationUnitOfWork.CompanyRepository);
            this.dataProvider = dataProvider;
            currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            this.notificationService = notificationService;
            this.memberCreator = memberCreator;
            this.config = config;
        }

        public RegistrationCheckOptions Check(RegistrationIndexViewModel model)
        {
            logger.Info("[RegistrationValidation][General][RegistrationService.Check] Start. IC={0}, CompanyID={1}, AgencyType={2}, IsIndividual={3}, IsGeneral={4}, AllowConflict={5}",
                model == null ? "" : model.ICNumber,
                model == null ? 0 : model.CompanyID,
                model == null || model.AgencyType == null ? "" : model.AgencyType.Code,
                model != null && model.IsIndividual,
                model != null && model.IsGeneral,
                model != null && model.AllowConflict);

            var validationPassed = Validate(model);
            logger.Info("[RegistrationValidation][General][RegistrationService.Check] Index validation result: {0}; ContextMessages={1}", validationPassed, CurrentContext.ValidationMessages.Count);
            if (!validationPassed)
            {
                logger.Warn("[RegistrationValidation][General][RegistrationService.Check] STOPPED with validation error.");
                return RegistrationCheckOptions.Error;
            }

            //[20210125] - To allow TO to register referred member as “General” if referred member only exists in “Family” (vice-versa)
            //if (registrationUnitOfWork.MemberRepository.IsMemberReferred(model.ICNumber) && model.AllowReferredMember == null)
            //    return RegistrationCheckOptions.Referred;
            if (registrationUnitOfWork.MemberRepository.IsMemeberReasonNotAllowedForRegistrationByIntermediary(model.ICNumber, false, true)
                && model.AllowReferredMember == null)
            {
                logger.Info("[RegistrationValidation][General][RegistrationService.Check] Result=Referred.");
                return RegistrationCheckOptions.Referred;
            }

            //var aph = registrationUnitOfWork.AgencyRepository.GetReinstateHistory(model.ICNumber, model.CompanyID, GetGeneralIntermediaryTypeId(), model.AgencyType.ID);
            var aph = registrationUnitOfWork.AgencyRepository.GetCorporateNomineeReinstateHistory(
                model.ICNumber,
                model.CompanyID,
                GetGeneralIntermediaryTypeId(),
                model.AgencyType.ID
            );
            if (aph != null)
            {
                logger.Info("[RegistrationValidation][General][RegistrationService.Check] Result=Reinstate. PrincipalHistoryID={0}", aph.ID);
                return RegistrationCheckOptions.Reinstate;
            }

            //var agency = registrationUnitOfWork.MemberRepository.GetActiveAgencies(model.ICNumber).FirstOrDefault();          
            var agency = registrationUnitOfWork.MemberRepository.GetCorporateNomineeActiveAgencies(model.ICNumber).FirstOrDefault();
            if (agency == null)
            {
                logger.Info("[RegistrationValidation][General][RegistrationService.Check] No active corporate-nominee agency. Result=New.");
                return RegistrationCheckOptions.New;
            }

            if (model.AllowConflict == true)
            {
                logger.Info("[RegistrationValidation][General][RegistrationService.Check] AllowConflict=true. Running conflict validation.");
                model.AllowConflict = false;
                if (!Validate(model, true))
                {
                    logger.Warn("[RegistrationValidation][General][RegistrationService.Check] Result=Conflict.");
                    return RegistrationCheckOptions.Conflict;
                }
            }
            //  if agency exist and in different type new change request 04-Mar-2014
            if (agency.AgencyPrincipals.Count(p => p.TypeID == model.AgencyType.ID && p.Member != null &&
                (p.Member.NewICNumber == model.ICNumber || p.Member.PassportNumber == model.ICNumber)) > 0)
            {
                logger.Info("[RegistrationValidation][General][RegistrationService.Check] Matching agency principal exists. Result=Include.");
                return RegistrationCheckOptions.Include;
            }

            logger.Info("[RegistrationValidation][General][RegistrationService.Check] No matching principal branch. Result=New.");
            return RegistrationCheckOptions.New;
        }

        public bool Check(RegistrationViewModel model)
        {
            bool result = Validate(model);
            result &= ValidateAgency(model as IExistableAgency);
            result &= ValidatePartners(model as PartnershipRegistrationViewModel);
            result &= ValidateBoardMembers(model as IExistableBoardMembers, model.AgencyType);

            if (result == true)
            {

            }

            return result;
        }

        public void Add(RegistrationUploadViewModel model)
        {
            logger.Info("---------------------------------------------------------------------------------");
            logger.Info("Add(RegistrationUploadViewModel)");

            var isSuccess = true;
            using (var scope = new TransactionScope())
            {
                foreach (var registration in model.Registrations)
                {
                    try
                    {
                        if (!Validate(registration.Model))
                        {
                            isSuccess = false;
                            continue;
                        }
                        registration.Model.CurrentAction = Actions.Add;
                        var agency = Mapper.Map<Agency>(registration.Model);
                        SaveAgency(agency, registration.Model);
                    }
                    catch (Exception ex)
                    {
                        if (registration.Errors == null)
                        {
                            registration.Errors = new Dictionary<string, string>();
                        }
                        registration.Errors.Add(string.Empty, ex.Message);
                        isSuccess = false;
                    }
                }
                if (isSuccess)
                {
                    scope.Complete();
                }

            }

        }

        public long Add(RegistrationViewModel model)
        {
            if (!Validate(model)) return -1;

            logger.Info("---------------------------------------------------------------------------------");
            logger.Info("Add(RegistrationViewModel)");

            model.CurrentAction = Actions.Add;
            //var agency = Mapper.Map<Agency>(model);
            String ic = model.CorporateNominee.ICType.Code == LookupConstants.ICTypes.NewIc ? model.CorporateNominee.NewICNumber : model.CorporateNominee.PassportNumber;
            logger.Info("CompanyID: {0}, IC: {1}", model.CompanyID, ic);

            //var agencies = registrationUnitOfWork.MemberRepository.GetActiveAgencies(ic);
            var agencies = registrationUnitOfWork.MemberRepository.GetAgencies(ic);
            if (agencies.Count() > 0)
                logger.Info("Agencies.ID: {0}", string.Join(";", agencies.Select(i => i.ID)));

            var agency = (agencies.Count() > 0) ? GetMappedAgency(model, agencies.First()) : Mapper.Map<Agency>(model);

            using (var scope = new TransactionScope())
            {
                SaveAgency(agency, model);
                scope.Complete();
            }
            return agency.ID;
        }


        Agency GetMappedAgency(RegistrationViewModel model, Agency agency)
        {
            if (model.GetType() == typeof(IndividualRegistrationViewModel))
                agency = Mapper.Map((IndividualRegistrationViewModel)model, agency);

            if (model.GetType() == typeof(SoleProprietorshipRegistrationViewModel))
                agency = Mapper.Map((SoleProprietorshipRegistrationViewModel)model, agency);

            if (model.GetType() == typeof(PartnershipRegistrationViewModel))
                agency = Mapper.Map((PartnershipRegistrationViewModel)model, agency);

            if (model.GetType() == typeof(CorporateRegistrationViewModel))
                agency = Mapper.Map((CorporateRegistrationViewModel)model, agency);

            return agency;
        }

        private void SaveAgency(Agency agency, RegistrationViewModel model)
        {
            logger.Info("---------------------------------------------------------------------------------");
            logger.Info("SaveAgency(Agency, RegistrationViewModel)");

            if (model is CorporateRegistrationViewModel)
                logger.Info("CorporateRegistrationViewModel");
            else if (model is IndividualRegistrationViewModel)
                logger.Info("IndividualRegistrationViewModel");
            else if (model is PartnershipRegistrationViewModel)
                logger.Info("PartnershipRegistrationViewModel");
            else if (model is SoleProprietorshipRegistrationViewModel)
                logger.Info("SoleProprietorshipRegistrationViewModel");

            /* [20191231] - Alway get the latest agency principal when add records to Journal */
            /* [20201206] - If AgencyPrincipal more than 1 record it will get wrong record, change to take ID = 0 newly added */
            AgencyPrincipal latestAgencyPrincipal;
            latestAgencyPrincipal = agency.AgencyPrincipals.Where(i => i.ID == 0).FirstOrDefault();
            if (latestAgencyPrincipal == null)
                latestAgencyPrincipal = agency.AgencyPrincipals.OrderByDescending(i => i.CreatedDate.HasValue).ThenByDescending(i => i.CreatedDate).FirstOrDefault();
            logger.Info("latestAgencyPrincipal.AgencyNumber: {0}", latestAgencyPrincipal.AgencyNumber);

            if (!string.IsNullOrEmpty(model.PhotoPath))
            {
                //model.PhotoPath.WatermarkImage(agency.AgencyPrincipals.FirstOrDefault().AgencyNumber);
                model.PhotoPath.WatermarkImage(latestAgencyPrincipal.AgencyNumber);
                registrationUnitOfWork.PhotoRepository.Save(new PhotoHistory()
                {
                    AgencyNumber = latestAgencyPrincipal.AgencyNumber,
                    IsActive = true,
                    Code = model.CurrentAction == Actions.Inclusion ?
                        LookupConstants.PhotoManageType.Inclusion :
                        LookupConstants.PhotoManageType.Registration,
                    PhotoPath = model.PhotoPath
                });
            }

            if (!(agency.ID > 0))
                registrationUnitOfWork.AgencyRepository.Save(agency);

            registrationUnitOfWork.Save();

            logger.Info(string.Format(@"Agency.ID: {0}, AgencyPrincipal.IDs: {1}, AgencyPrincipal.CompanyIDs: {2}, AgencyPrincipal.AgencyNumbers: {3}",
                agency.ID,
                string.Join(";", agency.AgencyPrincipals.Select(i => i.ID)),
                string.Join(";", agency.AgencyPrincipals.Select(i => i.CompanyID)),
                string.Join(";", agency.AgencyPrincipals.Select(i => i.AgencyNumber))
            ));
            if (agency.AgencyMembers != null && agency.AgencyMembers.Count() > 0)
            {
                logger.Info(string.Format(@"AgencyMember.IDs: {0}, AgencyMember.MemberIDs: {1}",
                    string.Join(";", agency.AgencyMembers.Select(i => i.ID)),
                    string.Join(";", agency.AgencyMembers.Select(i => i.MemberID))
                ));
            }

            //var agencyPrincipal = agency.AgencyPrincipals.OrderByDescending(i => i.CreatedDate).FirstOrDefault();

            var uri = String.Format("/Registration/Complete/{0}", agency.ID);
            var journal = GetJournal(latestAgencyPrincipal.ID, LookupConstants.Activities.NewRegistration, uri);
            //var journal = GetJournal(agency.AgencyPrincipals.First().ID, LookupConstants.Activities.NewRegistration, uri);
            registrationUnitOfWork.JournalRepository.Save(journal);
            logger.Info(string.Format(@"Description: {0}, AgencyID: {1}, CompanyID: {2}",
                string.IsNullOrEmpty(journal.Description) ? "" : journal.Description,
                journal.AgencyID ?? -1,
                journal.CompanyID ?? -1
            ));
            registrationUnitOfWork.Save();
            logger.Info(string.Format(@"Journal ID: {0}", journal.ID));

            notificationService.Notify(
                MTACompany.ID,
                model.CompanyID,
                LookupConstants.Notifications.NewRegistration,
                Mapper.Map<RegistrationVariableSource>(agency)
            );
        }


        public long Add(RegistrationConflictViewModel model)
        {
            if (!Validate(model)) return -1;
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var agency = registrationUnitOfWork.AgencyRepository.Get(model.AgencyID);
            using (var scope = new TransactionScope())
            {
                var principals = Mapper.Map<List<AgencyPrincipalConflict>>(model);
                foreach (var principal in principals)
                {
                    principal.Reason = LookupConstants.ConflictReasons.PrincipalLimitExceeded;
                    agency.AgencyPrincipalConflicts.Add(principal);
                    var ap = agency.AgencyPrincipals.First(p => p.IntermediaryTypeID == principal.IntermediaryTypeID);
                    var conflictVariableSource = new ConflictVariableSource
                    {
                        AgencyNumber = ap.AgencyNumber,
                        Date = DateTime.Now.ToString(GlobalConstants.DateFormat),
                        DestinationCompany = identity.CompanyName,
                        SourceCompany = ap.Company.Name,
                        ICNumber = agency.AgencyMembers.First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee).Member.NewICNumber
                    };
                    notificationService.Notify(MTACompany.ID, identity.CompanyID, LookupConstants.Notifications.DestinationConflictInit, conflictVariableSource);
                    notificationService.Notify(MTACompany.ID, model.SourceCompanyID, LookupConstants.Notifications.SourceConflictInint, conflictVariableSource);
                }
                registrationUnitOfWork.Save();
                scope.Complete();
            }
            return agency.ID;
        }

        public void Include(RegistrationInclusionViewModel model)
        {
            logger.Info("---------------------------------------------------------------------------------");
            logger.Info("Method Name: RegistrationService.Include");
            logger.Info(string.Format("Agency ID: {0}", model.AgencyID));

            logger.Info("[RegistrationValidation][General][RegistrationService.Include] Starting inclusion validation. CompanyID={0}, AgencyType={1}, IsGeneral={2}",
                model.CompanyID,
                model.AgencyType == null ? "" : model.AgencyType.Code,
                model.IsGeneral);

            var validationPassed = Validate(model);
            logger.Info("[RegistrationValidation][General][RegistrationService.Include] Inclusion validation result: {0}; ContextMessages={1}", validationPassed, CurrentContext.ValidationMessages.Count);
            if (!validationPassed)
            {
                logger.Warn("[RegistrationValidation][General][RegistrationService.Include] SAVE BLOCKED by validation.");
                return;
            }

            var agency = registrationUnitOfWork.AgencyRepository.Get(model.AgencyID);
            logger.Info("[RegistrationValidation][General][RegistrationService.Include] Agency lookup completed. Found={0}", agency != null);

            var principals = Mapper.Map<List<AgencyPrincipal>>(model);
            foreach (var principal in principals)
            {
                logger.Info(string.Format("Agency Principal Number (New): {0}", principal.AgencyNumber ?? ""));
                agency.AgencyPrincipals.Add(principal);
            }
            logger.Info("[RegistrationValidation][General][RegistrationService.Include] Prepared {0} principal(s) for save.", principals.Count);
            using (var scope = new TransactionScope())
            {
                if (!string.IsNullOrEmpty(model.PhotoPath))
                {
                    model.PhotoPath.WatermarkImage(agency.AgencyPrincipals.OrderByDescending(x => x.ID).FirstOrDefault().AgencyNumber);
                    registrationUnitOfWork.PhotoRepository.Save(new PhotoHistory()
                    {
                        AgencyNumber = agency.AgencyPrincipals.OrderByDescending(x => x.ID).FirstOrDefault().AgencyNumber,
                        IsActive = true,
                        Code = LookupConstants.PhotoManageType.Inclusion,
                        PhotoPath = model.PhotoPath
                    });
                }
                registrationUnitOfWork.Save();

                var uri = String.Format("/Registration/Complete/{0}", agency.ID);
                var journal = GetJournal(principals.First().ID, LookupConstants.Activities.Inclusion, uri);
                registrationUnitOfWork.JournalRepository.Save(journal);
                registrationUnitOfWork.Save();
                logger.Info(string.Format("Journal ID: {0}", journal.ID));

                scope.Complete();
            }

        }

        public void Reinstate(RegistrationReinstationViewModel model)
        {
            var agency = registrationUnitOfWork.AgencyRepository.Get(model.AgencyID);
            var intermediaryTypeId = GetGeneralIntermediaryTypeId();
            var aph = registrationUnitOfWork.AgencyRepository.GetReinstateHistory(model.AgencyID, model.CompanyID, intermediaryTypeId, agency.AgencyPrincipalHistories.OrderByDescending(p => p.TerminationDate).FirstOrDefault(p => p.CompanyID == model.CompanyID).LookupAgencyType.ID);
            var ap = Mapper.Map<AgencyPrincipal>(aph);
            aph.AgencyPrincipalGuarantorHistories.ToList().Each(apgh =>
            {
                ap.AgencyPrincipalGuarantors.Add(Mapper.Map<AgencyPrincipalGuarantor>(apgh));
                registrationUnitOfWork.AgencyPrincipalGuarantorHistoryRepository.Delete(apgh);
            });
            aph.AgencyPrincipalStatusHistories.ToList().Each(apsh =>
            {
                ap.AgencyPrincipalStatus.Add(Mapper.Map<AgencyPrincipalStatus>(apsh));
                registrationUnitOfWork.AgencyPrincipalStatusHistoryRepository.Delete(apsh);
            });
            agency.AgencyPrincipals.Add(ap);
            registrationUnitOfWork.AgencyPrincipalHistoryRepository.Delete(aph);
            registrationUnitOfWork.Save();
        }

        bool ValidatePartners(PartnershipRegistrationViewModel model)
        {
            if (model == null) return true;
            bool result = true;
            result &= Validate(model);
            foreach (var partner in model.Partners)
            {
                partner.AgencyType = model.AgencyType;
                result &= Validate(partner);
            }
            return result;
        }

        bool ValidateBoardMembers(IExistableBoardMembers model, LookupItem agencyType)
        {
            if (model == null) return true;
            bool result = true;
            result &= Validate(model);

            foreach (var director in model.Directors)
            {
                director.AgencyType = agencyType;
                result &= Validate(director);
            }
            foreach (var shareholder in model.Shareholders)
            {
                shareholder.AgencyType = agencyType;
                result &= Validate(shareholder);
            }
            foreach (var acn in model.AdditionalCorporateNominees)
            {
                acn.AgencyType = agencyType;
                result &= Validate(acn);
            }
            return result;
        }

        bool ValidateAgency(IExistableAgency model)
        {
            if (model == null) return true;
            bool result = true;
            result &= Validate(model);
            return result;
        }

        long GetGeneralIntermediaryTypeId()
        {
            return registrationUnitOfWork.LookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
        }

        public RegistrationViewModel Get(long id)
        {
            var agency = registrationUnitOfWork.AgencyRepository.Get(id);
            //to-do need to pass valid type
            var model = GetViewModel(agency, null);
            if (model.Guarantor == null)
            {
                model.Guarantor = new GuarantorViewModel();
            }

            return model;
        }

        public RegistrationViewModel GetByPrincipal(long id)
        {
            var agencyPrincipal = registrationUnitOfWork.AgencyPrincipalRepository.Get(id);
            var agency = agencyPrincipal.Agency;
            var model = GetViewModel(agency, agencyPrincipal.LookupAgencyType);
            if (model.Guarantor == null)
            {
                model.Guarantor = new GuarantorViewModel();
            }
            model.DateAppointed = agencyPrincipal.DateAppointed;
            model.AgencyNumber = agencyPrincipal.AgencyNumber;
            return model;
        }

        RegistrationViewModel GetViewModel(Agency agency, LookupAgencyType agencyType)
        {
            RegistrationViewModel model = null;

            switch (agencyType.Code)
            {
                case LookupConstants.AgencyType.Cooperative:
                case LookupConstants.AgencyType.GovernmentAgency:
                case LookupConstants.AgencyType.PrivateLimitedCompany:
                case LookupConstants.AgencyType.PublicLimitedCompany:
                    model = Mapper.Map<CorporateRegistrationViewModel>(agency);
                    break;
                case LookupConstants.AgencyType.Individual:
                    model = Mapper.Map<IndividualRegistrationViewModel>(agency);
                    break;
                case LookupConstants.AgencyType.Partnership:
                    model = Mapper.Map<PartnershipRegistrationViewModel>(agency);
                    break;
                case LookupConstants.AgencyType.SoleProprietorship:
                    model = Mapper.Map<SoleProprietorshipRegistrationViewModel>(agency);
                    break;
            }

            return model;

        }

        public void UpdateAddress(RegistrationViewModel model, long principalId)
        {
            using (var scope = new TransactionScope())
            {
                var agency = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId).Agency;
                Mapper.Map<AddressViewModel, Address>(model.Address, agency.Address);
                var variableSource = new UpdateAddressVariableSource { AgencyName = model.Agency.Name };
                SendMaintenanceMail(LookupConstants.Notifications.UpdateAddress, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void UpdatePhoto(string path, long principalId)
        {
            using (var scope = new TransactionScope())
            {
                var agencyPrincipal = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                agencyPrincipal.PhotoPath = path.WatermarkImage(agencyPrincipal.AgencyNumber);
                registrationUnitOfWork.PhotoRepository.Save(new PhotoHistory()
                {
                    AgencyNumber = agencyPrincipal.AgencyNumber,
                    IsActive = true,
                    Code = LookupConstants.PhotoManageType.Update,
                    PhotoPath = agencyPrincipal.PhotoPath
                });
                //var variableSource = new UpdateAddressVariableSource { AgencyName = model.Agency.Name };
                //SendMaintenanceMail(LookupConstants.Notifications.UpdateAddress, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void UpdatePartners(RegistrationViewModel model, long principalId)
        {
            if (ValidatePartners(model as PartnershipRegistrationViewModel) == false) return;
            using (var scope = new TransactionScope())
            {
                var agency = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId).Agency;
                model = model as PartnershipRegistrationViewModel;
                //var newAgency = Mapper.Map<Agency>(model);
                //var cn = memberCreator.GetCorporateNominee(model);
                var partners = memberCreator.GetPartners(model as PartnershipRegistrationViewModel);
                var partnerID = registrationUnitOfWork.LookupRepository.Get<LookupDesignation>(LookupConstants.Designation.Partner).ID;
                // var partners = newAgency.AgencyMembers.Where(m => m.DesignationID == partnerID).ToList();
                var oldPartners = agency.AgencyMembers.Where(desg => desg.DesignationID == partnerID).ToList();
                //agency.AgencyMembers.RemoveRange(oldPartners);

                foreach (var member in oldPartners)
                {
                    registrationUnitOfWork.AgencyMemberRepository.Delete(member);
                }
                var nominee = memberCreator.GetCorporateNomineeAsPartner(registrationUnitOfWork.MemberRepository.GetCorporateNominee(agency.ID));
                agency.AgencyMembers.Add(nominee);
                agency.AgencyMembers.AddRange(partners);
                AssignMaintenanceJournal(LookupConstants.Activities.UpdatePartners, agency, registrationUnitOfWork);
                var variableSource = new UpdatePartnersVariableSource { AgencyName = model.Agency.Name };
                SendMaintenanceMail(LookupConstants.Notifications.UpdatePartners, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }
        public void ChangeNominee(ChangeNomineeViewModel model)
        {
            if (!Validate(model)) return;
            using (var scope = new TransactionScope())
            {
                var agency = registrationUnitOfWork.AgencyPrincipalRepository.Get(model.PrincipalID).Agency;
                /* var nominee = registrationUnitOfWork.MemberRepository.GetCorporateNominee(model.ID);
                 nominee = Mapper.Map<Member, Member>(Mapper.Map<CorporateNomineeViewModel, Member>(model.RegistrationModel.CorporateNominee), nominee);*/
                var agencymMember = agency.AgencyMembers.SingleOrDefault(ag => ag.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);

                if (agencymMember != null)
                {
                    agencymMember.Member = Mapper.Map<CorporateNomineeViewModel, Member>(model.RegistrationModel.CorporateNominee);
                }
                AssignMaintenanceJournal(LookupConstants.Activities.ChangeNominee, agency, registrationUnitOfWork);
                var variableSource = Mapper.Map<ChangeNomineeVariableSource>(model.RegistrationModel.CorporateNominee);
                SendMaintenanceMail(LookupConstants.Notifications.ChangeNominee, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }
        public void UpdateGuarantor(RegistrationViewModel model, long principalId)
        {
            using (var scope = new TransactionScope())
            {
                var agency = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId).Agency;
                var companyId = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity).CompanyID;
                var ap = agency.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == companyId);
                if (ap != null)
                {
                    var guarantor = ap.AgencyPrincipalGuarantors.FirstOrDefault();
                    if (guarantor != null)
                    {
                        Mapper.Map<GuarantorViewModel, AgencyPrincipalGuarantor>(model.Guarantor, guarantor);
                    }
                    else
                    {
                        ap.AgencyPrincipalGuarantors.Add(Mapper.Map<GuarantorViewModel, AgencyPrincipalGuarantor>(model.Guarantor));
                    }
                }
                AssignMaintenanceJournal(LookupConstants.Activities.UpdateGuarantor, agency, registrationUnitOfWork);
                var variableSource = new UpdateGuarantorVariableSource { AgencyName = model.Agency.Name, GuarantorTypeDescription = model.Guarantor.GuarantorType.Description };
                SendMaintenanceMail(LookupConstants.Notifications.UpdateGuarantor, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void UpdateBoardMembers(RegistrationViewModel model, long principalId)
        {
            if (ValidateBoardMembers(model as IExistableBoardMembers, model.AgencyType) == false) return;
            using (var scope = new TransactionScope())
            {
                var agency = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId).Agency;
                UpdateBoardMembersAndACN(agency, model, principalId);
                AssignMaintenanceJournal(LookupConstants.Activities.UpdateBoardMembers, agency, registrationUnitOfWork);
                var variableSource = new UpdateBoardMembersVariableSource { AgencyName = model.Agency.Name };
                SendMaintenanceMail(LookupConstants.Notifications.UpdateBoardMembers, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void UpdateACN(RegistrationViewModel model, long principalId)
        {
            if (ValidateBoardMembers(model as IExistableBoardMembers, model.AgencyType) == false) return;
            using (var scope = new TransactionScope())
            {
                var agency = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId).Agency;
                UpdateBoardMembersAndACN(agency, model, principalId);
                AssignMaintenanceJournal(LookupConstants.Activities.ChangeACN, agency, registrationUnitOfWork);
                var variableSource = new ChangeACNVariableSource { AgencyName = model.Agency.Name };
                SendMaintenanceMail(LookupConstants.Notifications.ChangeACN, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        void UpdateBoardMembersAndACN(Agency agency, RegistrationViewModel model, long principalId)
        {

            var cn = agency.AgencyMembers.First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
            var boardMembers = memberCreator.GetBoardMembers(cn.Member, model as IExistableBoardMembers);
            var oldMembers = agency.AgencyMembers.Where(p => p.LookupDesignation.Code == LookupConstants.Designation.Director
                || p.LookupDesignation.Code == LookupConstants.Designation.Shareholder
                || p.LookupDesignation.Code == LookupConstants.Designation.AdditionalCorporateNominee).ToList();
            //agency.AgencyMembers.RemoveRange(oldMembers);
            foreach (var am in oldMembers)
            {
                registrationUnitOfWork.AgencyMemberRepository.Delete(am);
            }
            agency.AgencyMembers.AddRange(boardMembers);
        }

        public void ChangeAgencyType(long principalId, string newTypeCode)
        {
            using (var scope = new TransactionScope())
            {
                var agencyPrincipal = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                var agency = agencyPrincipal.Agency;
                var oldTypeDesc = agencyPrincipal.LookupAgencyType.Description;
                var newType = registrationUnitOfWork.LookupRepository.Get<LookupAgencyType>(newTypeCode);
                var newTypeDesc = newType.Description;
                agencyPrincipal.TypeID = newType.ID;
                registrationUnitOfWork.Save();
                var variableSource = new ChangeCorporateStatusVariableSource { AgencyName = agency.Name, NewAgencyType = newTypeDesc, OldAgencyType = oldTypeDesc };
                SendMaintenanceMail(LookupConstants.Notifications.ChangeCorporateStatus, agency, variableSource);
                scope.Complete();
            }
        }

        void AssignMaintenanceJournal(string activityCode, Agency agency, IRegistrationUnitOfWork registrationUnitOfWork)
        {
            var uri = String.Format("/Registration/Complete/{0}", agency.ID);
            foreach (var principal in agency.AgencyPrincipals)
            {
                var journal = GetJournal(principal.ID, activityCode, uri);
                registrationUnitOfWork.JournalRepository.Save(journal);
            }
        }

        void SendMaintenanceMail(string notificationCode, Agency agency, INotificationVariableSource source)
        {
            foreach (var companyId in agency.AgencyPrincipals.Select(p => p.CompanyID).Distinct())
            {
                notificationService.Notify(MTACompany.ID, companyId, notificationCode, source);
            }
        }

        public void UpdateCompanyName(ChangeCompanyViewModel model)
        {
            if (!Validate(model)) return;
            using (var scope = new TransactionScope())
            {
                var oldAgency = registrationUnitOfWork.AgencyPrincipalRepository.Get(model.PrincipalID).Agency;
                var oldAgencyName = oldAgency.Name;
                oldAgency.Name = model.RegistrationModel.Agency.Name;
                oldAgency.BusinessRegistrationNumber = model.RegistrationModel.Agency.BusinessRegistrationNumber;
                var variableSource = new ChangeCompanyVariableSource
                {
                    AgencyName = model.RegistrationModel.Agency.Name,
                    OldCompanyName = oldAgencyName,
                    NewCompanyName = oldAgency.Name
                };
                SendMaintenanceMail(LookupConstants.Notifications.ChangeCompany, oldAgency, variableSource);
                AssignMaintenanceJournal(LookupConstants.Activities.ChangeCompany, oldAgency, registrationUnitOfWork);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void UpdateAgency(RegistrationViewModel model, long principalId)
        {
            using (var scope = new TransactionScope())
            {
                var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);

                var principal = registrationUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                var agency = principal.Agency;
                var cn = registrationUnitOfWork.MemberRepository.GetCorporateNominee(agency.ID);
                Mapper.Map<AgencyViewModel, Agency>(model.Agency, agency);

                /*var agencyMember = agency.AgencyMembers.SingleOrDefault(am => am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
                if (agencyMember != null) {
                   agencyMember.Member =  Mapper.Map<Member, Member>(Mapper.Map<CorporateNomineeViewModel, Member>(model.CorporateNominee), agencyMember.Member);
                }*/
                // Mapper.Map<Member, Member>(Mapper.Map<CorporateNomineeViewModel, Member>(model.CorporateNominee), cn);
                cn.Name = model.CorporateNominee.Name;
                cn.RaceID = model.CorporateNominee.Race.ID;
                cn.ReligionID = model.CorporateNominee.Religion.ID;
                cn.MaritalStatusID = model.CorporateNominee.MaritalStatus.ID;
                cn.OldICNumber = model.CorporateNominee.OldICNumber;
                cn.BirthDate = model.CorporateNominee.BirthDate;
                cn.IsBancaStaff = model.CorporateNominee.IsBancaStaff;
                cn.IsBumiputera = model.CorporateNominee.IsBumiputera;
                cn.IsCitizen = model.CorporateNominee.IsCitizen;
                principal.DateAppointed = model.DateAppointed;
                cn.Phone = model.CorporateNominee.Phone;
                cn.Fax = model.CorporateNominee.Fax;
                cn.Email = model.CorporateNominee.Email;
                if (cn.MemberEducationalQualifications.Count == 0)
                {
                    cn.MemberEducationalQualifications.Add(Mapper.Map<MemberEducationalQualification>(model.CorporateNominee.Qualification));
                }
                else
                {
                    var qualification = cn.MemberEducationalQualifications.First();
                    qualification.SchoolName = model.CorporateNominee.Qualification.SchoolName;
                    qualification.Year = model.CorporateNominee.Qualification.Year;
                    qualification.EducationalQualificationID = model.CorporateNominee.Qualification.EducationalQualification.ID;
                }
                if (cn.Spouses.Count > 0)
                {
                    var destSpouse = cn.Spouses.First();
                    var srcSpouse = model.CorporateNominee.Spouse;
                    destSpouse.Name = srcSpouse.Name;
                    destSpouse.NewICNumber = srcSpouse.NewICNumber;
                    destSpouse.OldICNumber = srcSpouse.OldICNumber;
                    destSpouse.RaceID = GetLookupID(srcSpouse.Race);
                    destSpouse.ReligionID = GetLookupID(srcSpouse.Religion);
                    destSpouse.BirthDate = srcSpouse.BirthDate;
                    destSpouse.GenderID = GetLookupID(srcSpouse.Gender);
                    destSpouse.IsBumiputera = srcSpouse.IsBumiputera;
                    destSpouse.IsCitizen = srcSpouse.IsCitizen;
                    destSpouse.MaritalStatusID = GetLookupID(srcSpouse.MaritalStatus);
                }
                var variableSource = new UpdateAgencyVariableSource { AgencyName = model.Agency.Name };
                SendMaintenanceMail(LookupConstants.Notifications.UpdateAgency, agency, variableSource);
                registrationUnitOfWork.Save();
                scope.Complete();
            }
        }

        long? GetLookupID(LookupItem item)
        {
            if (item == null) return null;
            return item.ID;
        }
    }// class
}// namespace
