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
using MTAoarsGeneral.ViewModels.Administrative;

namespace MTAoarsGeneral.Services.Operations
{
    public class AdministrativeService : BaseService, IAdministrativeService
    {
        //IRegistrationUnitOfWork registrationUnitOfWork;
        IAdministrativeUnitOfWork administrativeUnitOfWork;
        ILookupService lookupService;
        IScopeDataProvider dataProvider;
        Identity currentIdentity;
        INotificaitonService notificationService;
        AgencyMemberCreator memberCreator;
        ConfigManager config;
        IIbfimResultRepository ibfim;
        IMiiResultRepository mii;
        IReferredMemberRepository referredMemberRepository;
        IRenewalRepository renewalRepository;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public AdministrativeService(IValidationProvider validationProvider, IAdministrativeUnitOfWork administrativeUnitOfWork, ILookupService lookupService, IScopeDataProvider dataProvider, INotificaitonService notificationService, AgencyMemberCreator memberCreator, ConfigManager config, IIbfimResultRepository ibfim, IMiiResultRepository mii, IReferredMemberRepository referredMemberRepository, IRenewalRepository renewalRepository)
            : base(validationProvider)
        {
            this.administrativeUnitOfWork = administrativeUnitOfWork;
            this.lookupService = lookupService; // new LookupService(registrationUnitOfWork.LookupRepository, registrationUnitOfWork.CompanyRepository);
            this.dataProvider = dataProvider;
            currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            this.notificationService = notificationService;
            this.memberCreator = memberCreator;
            this.config = config;
            this.ibfim = ibfim;
            this.mii = mii;
            this.referredMemberRepository = referredMemberRepository;
            this.renewalRepository = renewalRepository;
        }

        public void UpdateAgency(AdministrativeAgentViewModel model, long principalId, bool isTerminated)
        {
            bool isHistory = false;

            using (var scope = new TransactionScope())
            {
                var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);

                IEntity principal;
                principal = administrativeUnitOfWork.AgencyPrincipalRepository.Get(principalId);
                if (principal == null || isTerminated)
                {
                    principal = administrativeUnitOfWork.AgencyPrincipalHistoryRepository.Get(principalId);
                    isHistory = true;
                }

                Agency agency = (!isTerminated)
                    ? (principal as AgencyPrincipal).Agency
                    : (principal as AgencyPrincipalHistory).Agency;

                var cn = administrativeUnitOfWork.MemberRepository.GetCorporateNominee(agency.ID);

                var auditTrails = new List<Tuple<long, string, string, string, string>>();

                if (!isHistory)
                {
                    if (!(principal as AgencyPrincipal).DateAppointed.Equals(model.DateAppointed))
                    {
                        auditTrails.Add(AddAuditTrail(principal.ID, nameof(AgencyPrincipal), nameof(AgencyPrincipal.DateAppointed),
                            (principal as AgencyPrincipal).DateAppointed, model.DateAppointed));
                        (principal as AgencyPrincipal).DateAppointed = model.DateAppointed;
                    }
                }
                else
                {
                    if (!(principal as AgencyPrincipalHistory).DateAppointed.Equals(model.DateAppointed))
                    {
                        auditTrails.Add(AddAuditTrail((principal as AgencyPrincipalHistory).ID, nameof(AgencyPrincipalHistory), nameof(AgencyPrincipalHistory.DateAppointed),
                            (principal as AgencyPrincipalHistory).DateAppointed, model.DateAppointed));
                        (principal as AgencyPrincipalHistory).DateAppointed = model.DateAppointed;
                    }
                }
                if (!object.Equals(cn.OldICNumber ?? "", model.CorporateNominee.OldICNumber ?? ""))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Member), nameof(Member.OldICNumber),
                        cn.OldICNumber, model.CorporateNominee.OldICNumber));
                    cn.OldICNumber = model.CorporateNominee.OldICNumber;
                }
                if (!object.Equals(cn.NewICNumber ?? "", model.CorporateNominee.NewICNumber ?? ""))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Member), nameof(Member.NewICNumber),
                        cn.NewICNumber, model.CorporateNominee.NewICNumber));
                    cn.NewICNumber = model.CorporateNominee.NewICNumber;
                }

                var nomineeTbeCategoryID = model.CorporateNominee.TbeCategory == null ? default(long) : model.CorporateNominee.TbeCategory.ID;
                if (!object.Equals(cn.TbeCategoryID ?? default(long), nomineeTbeCategoryID))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Member), nameof(Member.TbeCategoryID),
                        cn.TbeCategoryID, model.CorporateNominee.TbeCategory.ID));
                    cn.TbeCategoryID = model.CorporateNominee.TbeCategory.ID;
                }
                if (!object.Equals(agency.M2Exam ?? default(string), model.Agency.M2Exam))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.M2Exam),
                        agency.M2Exam, model.Agency.M2Exam));
                    agency.M2Exam = model.Agency.M2Exam.Code;
                }
                if (!object.Equals(agency.DateOfExam ?? default(DateTime?), model.Agency.DateOfExam))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.DateOfExam),
                        agency.DateOfExam, model.Agency.DateOfExam));
                    agency.DateOfExam = model.Agency.DateOfExam;
                }

                if (!object.Equals(agency.JoinYear ?? default(string), model.Agency.JoinYear))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.JoinYear),
                        agency.JoinYear, model.Agency.JoinYear));
                    agency.JoinYear = model.Agency.JoinYear.Code;
                }
                if (!object.Equals(agency.JoinMonth ?? default(string), model.Agency.JoinMonth))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.JoinMonth),
                        agency.JoinMonth, model.Agency.JoinMonth));
                    agency.JoinMonth = model.Agency.JoinMonth.Code;
                }
                if (!object.Equals(agency.MTAAwards ?? default(string), model.Agency.MTAAwards))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.MTAAwards),
                        agency.MTAAwards, model.Agency.MTAAwards));
                    agency.MTAAwards = string.Join(";", model.Agency.MTAAwards.Select(x => x.Code));
                }
                if (!object.Equals(agency.SocialMediaAddress ?? default(string), model.Agency.SocialMediaAddress))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.SocialMediaAddress),
                        agency.SocialMediaAddress, model.Agency.SocialMediaAddress));
                    agency.SocialMediaAddress = model.Agency.SocialMediaAddress;
                }

                if (!object.Equals(cn.IsBancaStaff ?? default(bool), model.CorporateNominee.IsBancaStaff))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Member), nameof(Member.IsBancaStaff),
                        cn.IsBancaStaff, model.CorporateNominee.IsBancaStaff));
                    cn.IsBancaStaff = model.CorporateNominee.IsBancaStaff;
                }
                /* IsBancaStaff ---------------------------------------------------- */
                if (!object.Equals(cn.IsBancaStaff ?? default(bool), model.CorporateNominee.IsBancaStaff))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Member), nameof(Member.IsBancaStaff),
                        cn.IsBancaStaff, model.CorporateNominee.IsBancaStaff));
                    cn.IsBancaStaff = model.CorporateNominee.IsBancaStaff;
                }
                if (!isHistory && !object.Equals((principal as AgencyPrincipal).IsBancaStaff ?? default(bool), model.CorporateNominee.IsBancaStaff))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(AgencyPrincipal), nameof(AgencyPrincipal.IsBancaStaff),
                        (principal as AgencyPrincipal).IsBancaStaff, model.CorporateNominee.IsBancaStaff));
                    (principal as AgencyPrincipal).IsBancaStaff = model.CorporateNominee.IsBancaStaff;
                }
                if (!isHistory && !object.Equals(agency.IsBancaStaff, model.CorporateNominee.IsBancaStaff))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Agency), nameof(Agency.IsBancaStaff),
                        agency.IsBancaStaff, model.CorporateNominee.IsBancaStaff));
                    agency.IsBancaStaff = model.CorporateNominee.IsBancaStaff;
                }

                if (!object.Equals(agency.BusinessRegistrationNumber ?? "", model.Agency.BusinessRegistrationNumber ?? ""))
                {
                    auditTrails.Add(AddAuditTrail(agency.ID, nameof(Agency), nameof(Agency.BusinessRegistrationNumber),
                        agency.BusinessRegistrationNumber, model.Agency.BusinessRegistrationNumber));
                    agency.BusinessRegistrationNumber = model.Agency.BusinessRegistrationNumber;
                }
                if (!object.Equals(agency.Name ?? "", model.Agency.Name ?? ""))
                {
                    auditTrails.Add(AddAuditTrail(agency.ID, nameof(Agency), nameof(Agency.Name), agency.Name, model.Agency.Name));
                    agency.Name = model.Agency.Name;
                }
                if (!object.Equals(cn.Name ?? "", model.CorporateNominee.Name ?? ""))
                {
                    auditTrails.Add(AddAuditTrail(cn.ID, nameof(Member), nameof(Member.Name), cn.Name, model.CorporateNominee.Name));
                    cn.Name = model.CorporateNominee.Name;
                }
                //if (isHistory && !object.Equals((principal as AgencyPrincipalHistory).TerminationDate, model.DateTerminated))
                if (isHistory && !(principal as AgencyPrincipalHistory).TerminationDate.Equals(model.DateTerminated))
                {
                    auditTrails.Add(AddAuditTrail((principal as AgencyPrincipalHistory).ID, nameof(AgencyPrincipalHistory), nameof(AgencyPrincipalHistory.TerminationDate),
                        (principal as AgencyPrincipalHistory).TerminationDate, model.DateTerminated));
                    (principal as AgencyPrincipalHistory).TerminationDate = model.DateTerminated;
                }

                if (isHistory && !(principal as AgencyPrincipalHistory).StatusID.Equals(model.Agency.StatusID))
                {
                    auditTrails.Add(AddAuditTrail((principal as AgencyPrincipalHistory).ID, nameof(AgencyPrincipalHistory), nameof(AgencyPrincipalHistory.StatusID),
                        (principal as AgencyPrincipalHistory).StatusID, model.Agency.StatusID));
                    (principal as AgencyPrincipalHistory).StatusID = model.Agency.StatusID;
                }

                if (isHistory)
                {
                    if ((principal as AgencyPrincipalHistory).Remarks != model.CorporateNominee.Remarks)
                    {
                        model.CorporateNominee.Remarks = string.IsNullOrEmpty(model.CorporateNominee.Remarks) ? "" : model.CorporateNominee.Remarks.ToUpper();
                        auditTrails.Add(AddAuditTrail((principal as AgencyPrincipalHistory).ID, nameof(AgencyPrincipalHistory)
                            , nameof(AgencyPrincipalHistory.Remarks)
                            , (principal as AgencyPrincipalHistory).Remarks, model.CorporateNominee.Remarks));

                        (principal as AgencyPrincipalHistory).Remarks = model.CorporateNominee.Remarks;
                    }
                }
                else
                {
                    if ((principal as AgencyPrincipal).Remarks != model.CorporateNominee.Remarks)
                    {
                        model.CorporateNominee.Remarks = string.IsNullOrEmpty(model.CorporateNominee.Remarks) ? "" : model.CorporateNominee.Remarks.ToUpper();
                        auditTrails.Add(AddAuditTrail((principal as AgencyPrincipal).ID, nameof(AgencyPrincipal)
                            , nameof(AgencyPrincipal.Remarks)
                            , (principal as AgencyPrincipal).Remarks, model.CorporateNominee.Remarks));

                        (principal as AgencyPrincipal).Remarks = model.CorporateNominee.Remarks;
                    }
                }

                if (!isHistory)
                {
                    if (!(principal as AgencyPrincipal).Rating.Equals(model.Agency.Rating.ID))
                    {
                        auditTrails.Add(AddAuditTrail(principal.ID,
                            nameof(AgencyPrincipal), nameof(AgencyPrincipal.Rating),
                            (principal as AgencyPrincipal).Rating, model.Agency.Rating.ID));
                        (principal as AgencyPrincipal).Rating = Convert.ToByte(model.Agency.Rating.ID);
                    }
                }
                else
                {
                    if (!(principal as AgencyPrincipalHistory).Rating.Equals(model.Agency.Rating.ID))
                    {
                        auditTrails.Add(AddAuditTrail((principal as AgencyPrincipalHistory).ID, nameof(AgencyPrincipalHistory), nameof(AgencyPrincipalHistory.Rating),
                            (principal as AgencyPrincipalHistory).Rating, model.Agency.Rating.ID));
                        (principal as AgencyPrincipalHistory).Rating = Convert.ToByte(model.Agency.Rating.ID);
                    }
                }

                foreach (var trail in auditTrails)
                {
                    administrativeUnitOfWork.AdministrativeAuditTrailRepository.Save(new AdministrativeAuditTrail()
                    {
                        AgencyNumber = (!isHistory) ? (principal as AgencyPrincipal).AgencyNumber : (principal as AgencyPrincipalHistory).AgencyNumber,
                        TableID = trail.Item1,
                        TableName = trail.Item2,
                        ColumnName = trail.Item3,
                        DataOriginal = trail.Item4,
                        DataUpdated = trail.Item5,
                        CreatedBy = identity.UserID
                    });
                }

                //principal.Date = model.DateTerminated;

                administrativeUnitOfWork.Save();
                scope.Complete();
            }
        }

        public AdministrativeAgentViewModel GetByPrincipal(long id)
        {
            var agencyPrincipal = administrativeUnitOfWork.AgencyPrincipalRepository.Get(id);
            if (agencyPrincipal == null)
                return null;
            var agency = agencyPrincipal.Agency;
            var model = GetViewModel(agency, agencyPrincipal.LookupAgencyType);
            if (model.Guarantor == null)
            {
                model.Guarantor = new GuarantorViewModel();
            }
            model.DateAppointed = agencyPrincipal.DateAppointed;
            model.AgencyNumber = agencyPrincipal.AgencyNumber;
            model.CorporateNominee.Remarks = agencyPrincipal.Remarks;
            model.Agency.Rating = new LookupItem { ID = agencyPrincipal.Rating ?? 0, Description = agencyPrincipal.Rating?.ToString(), Code = agencyPrincipal.Rating?.ToString() };
            return model;
        }

        AdministrativeAgentViewModel GetViewModel(Agency agency, LookupAgencyType agencyType)
        {
            AdministrativeAgentViewModel model = null;

            switch (agencyType.Code)
            {
                case LookupConstants.AgencyType.Cooperative:
                case LookupConstants.AgencyType.GovernmentAgency:
                case LookupConstants.AgencyType.PrivateLimitedCompany:
                case LookupConstants.AgencyType.PublicLimitedCompany:
                    model = Mapper.Map<AdministrativeAgentCorporateViewModel>(agency);
                    break;
                case LookupConstants.AgencyType.Individual:
                    model = Mapper.Map<AdministrativeAgentIndividualViewModel>(agency);
                    break;
                case LookupConstants.AgencyType.Partnership:
                    model = Mapper.Map<AdministrativeAgentPartnershipViewModel>(agency);
                    break;
                case LookupConstants.AgencyType.SoleProprietorship:
                    model = Mapper.Map<AdministrativeAgentSoleProprietorshipViewModel>(agency);
                    break;
            }

            return model;
        }

        public AdministrativeAgentViewModel GetByPrincipalHistory(long id)
        {
            var agencyPrincipalHistory = administrativeUnitOfWork.AgencyPrincipalHistoryRepository.Get(id);
            if (agencyPrincipalHistory == null)
                return null;
            var agency = agencyPrincipalHistory.Agency;
            var model = GetViewModel(agency, agencyPrincipalHistory.LookupAgencyType);
            if (model.Guarantor == null)
            {
                model.Guarantor = new GuarantorViewModel();
            }
            model.DateAppointed = agencyPrincipalHistory.DateAppointed;
            model.DateTerminated = agencyPrincipalHistory.TerminationDate;
            model.AgencyNumber = agencyPrincipalHistory.AgencyNumber;
            model.IsTerminated = true;
            model.CorporateNominee.Remarks = agencyPrincipalHistory.Remarks;
            model.Agency.StatusID = !agencyPrincipalHistory.StatusID.HasValue ? 0 : Convert.ToInt64(agencyPrincipalHistory.StatusID);
            model.Agency.Rating = new LookupItem { ID = agencyPrincipalHistory.Rating ?? 0, Description = agencyPrincipalHistory.Rating?.ToString(), Code = agencyPrincipalHistory.Rating?.ToString() };

            return model;
        }

        public void InsertTBEResultToAuditTrail(TBEEnquirySearchViewModel model, string TableName)
        {
            using (var scope = new TransactionScope())
            {
                var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);

                var auditTrails = new List<Tuple<long, string, string, string, string>>();

                long ID = Convert.ToInt64(model.Result.ID);
                var ICnumber = "";
                switch (TableName)
                {
                    case "IbfimResult":
                        //ibfim
                        var getData = ibfim.Get(model.Result.ID);
                        ICnumber = getData.ICNumber;
                        //IC
                        if (model.Result.ICNumber != getData.ICNumber)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "ICNumber", getData.ICNumber, model.Result.ICNumber));
                        }
                        //NAME
                        if (model.Result.Name != getData.Name)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "Name", getData.Name, model.Result.Name));
                        }
                        //GRADE
                        if (model.Result.Grade != getData.Grade)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "Grade", getData.Grade, model.Result.Grade));
                        }
                        //RESULT
                        if (model.Result.Result != getData.Result)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "Result", getData.Result, model.Result.Result));
                        }
                        //EXAM DATE
                        if (model.Result.ExamDate != getData.ExamDate)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "ExamDate", getData.ExamDate, model.Result.ExamDate));
                        }
                        break;
                    case "MiiResult":
                        //Mii
                        var OriData = mii.Get(model.Result.ID);
                        ICnumber = OriData.ICNumber;
                        //IC
                        if (model.Result.ICNumber != OriData.ICNumber)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "ICNumber", OriData.ICNumber, model.Result.ICNumber));
                        }
                        //NAME
                        if (model.Result.Name != OriData.Name)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "Name", OriData.Name, model.Result.Name));
                        }
                        //GRADE
                        if (model.Result.Grade != OriData.Grade)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "Grade", OriData.Grade, model.Result.Grade));
                        }
                        //EXAM DATE
                        if (model.Result.ExamDate != OriData.ExamDate)
                        {
                            auditTrails.Add(AddAuditTrail(ID, TableName, "ExamDate", OriData.ExamDate, model.Result.ExamDate));
                        }
                        break;
                }

                foreach (var trail in auditTrails)
                {
                    administrativeUnitOfWork.AdministrativeAuditTrailRepository.Save(new AdministrativeAuditTrail()
                    {
                        ICNumber = ICnumber,
                        TableID = trail.Item1,
                        TableName = trail.Item2,
                        ColumnName = trail.Item3,
                        DataOriginal = trail.Item4,
                        DataUpdated = trail.Item5,
                        CreatedBy = identity.UserID
                    });
                }

                administrativeUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void InsertReferredToAuditTrail(ReferredSearchViewModel model)
        {
            using (var scope = new TransactionScope())
            {
                var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);

                var auditTrails = new List<Tuple<long, string, string, string, string>>();

                //ReferredMember
                var ReferredMemberData = referredMemberRepository.Get(model.ReferredMember.ID);

                //ReferredDetail
                var ReferredDetailData = referredMemberRepository.ReferredDetail(ReferredMemberData);

                //if (ReferredDetailData == null || ReferredMemberData == null) return;

                //IC
                if (model.ReferredMember.ICNumber != ReferredMemberData.NewICNumber)
                {
                    auditTrails.Add(AddAuditTrail(ReferredMemberData.ID, nameof(ReferredMember), nameof(ReferredMember.NewICNumber), ReferredMemberData.NewICNumber, model.ReferredMember.ICNumber));
                }
                //NAME
                if (model.ReferredMember.Name != ReferredMemberData.Name)
                {
                    auditTrails.Add(AddAuditTrail(ReferredMemberData.ID, nameof(ReferredMember), nameof(ReferredMember.Name), ReferredMemberData.Name, model.ReferredMember.Name));
                }
                //REASON
                if (model.ReferredMember.ReasonID != ReferredMemberData.ReasonID)
                {
                    auditTrails.Add(AddAuditTrail(ReferredMemberData.ID, nameof(ReferredMember), nameof(ReferredMember.ReasonID), ReferredMemberData.ReasonID, model.ReferredMember.ReasonID));
                }

                if (ReferredDetailData != null)
                {
                    //IC
                    if (model.ReferredMember.ICNumber != ReferredDetailData.ICNumber)
                        auditTrails.Add(AddAuditTrail(ReferredMemberData.ID, nameof(ReferredDetail), nameof(ReferredDetail.ICNumber), ReferredDetailData.ICNumber, model.ReferredMember.ICNumber));
                    //NAME
                    if (model.ReferredMember.Name != ReferredDetailData.Name)
                        auditTrails.Add(AddAuditTrail(ReferredMemberData.ID, nameof(ReferredDetail), nameof(ReferredDetail.Name), ReferredDetailData.Name, model.ReferredMember.Name));
                    //REASON
                    if (model.ReferredMember.ReasonID != ReferredDetailData.ReasonID)
                        auditTrails.Add(AddAuditTrail(ReferredMemberData.ID, nameof(ReferredDetail), nameof(ReferredDetail.ReasonID), ReferredDetailData.ReasonID, model.ReferredMember.ReasonID));
                }


                foreach (var trail in auditTrails)
                {
                    administrativeUnitOfWork.AdministrativeAuditTrailRepository.Save(new AdministrativeAuditTrail()
                    {
                        ICNumber = ReferredMemberData.NewICNumber,
                        TableID = trail.Item1,
                        TableName = trail.Item2,
                        ColumnName = trail.Item3,
                        DataOriginal = trail.Item4,
                        DataUpdated = trail.Item5,
                        CreatedBy = identity.UserID
                    });
                }

                administrativeUnitOfWork.Save();
                scope.Complete();
            }
        }

        public void InsertRenewalToAuditTrail(RenewalDetailsViewModel model)
        {
            using (var scope = new TransactionScope())
            {
                var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);

                var auditTrails = new List<Tuple<long, string, string, string, string>>();

                var OriData = new RenewalViewDetail();

                var listAgencyNumber = new List<string>();

                foreach (var renewalDetail in model.RenewalDetail)
                {
                    OriData = renewalRepository.RenewalViewDetail(renewalDetail.ID);
                    //Status
                    if (OriData.StatusID != renewalDetail.RenewalStatus.ID)
                    {
                        listAgencyNumber.Add(OriData.AgencyNumber);
                        auditTrails.Add(AddAuditTrail(OriData.ID, nameof(RenewalDetail), nameof(RenewalDetail.StatusID), OriData.StatusID, renewalDetail.RenewalStatus.ID));
                    }
                    //Remarks
                    if (OriData.Remarks != renewalDetail.Remarks)
                    {
                        listAgencyNumber.Add(OriData.AgencyNumber);
                        auditTrails.Add(AddAuditTrail(OriData.ID, nameof(RenewalDetail), nameof(RenewalDetail.Remarks), OriData.Remarks, renewalDetail.Remarks));
                    }
                }

                //foreach (var trail in auditTrails)
                //{
                //    administrativeUnitOfWork.AdministrativeAuditTrailRepository.Save(new AdministrativeAuditTrail()
                //    {
                //        AgencyNumber = ,
                //        TableID = trail.Item1,
                //        TableName = trail.Item2,
                //        ColumnName = trail.Item3,
                //        DataOriginal = trail.Item4,
                //        DataUpdated = trail.Item5,
                //        CreatedBy = identity.UserID
                //    });
                //}

                for (var i = 0; i < auditTrails.Count(); i++)
                {
                    administrativeUnitOfWork.AdministrativeAuditTrailRepository.Save(new AdministrativeAuditTrail()
                    {
                        AgencyNumber = listAgencyNumber[i],
                        TableID = auditTrails[i].Item1,
                        TableName = auditTrails[i].Item2,
                        ColumnName = auditTrails[i].Item3,
                        DataOriginal = auditTrails[i].Item4,
                        DataUpdated = auditTrails[i].Item5,
                        CreatedBy = identity.UserID
                    });
                }

                administrativeUnitOfWork.Save();
                scope.Complete();
            }
        }

        private Tuple<long, string, string, string, string> AddAuditTrail(
            long tableID, string tableName, string columnName, object dataOriginal, object dataUpdated)
        {
            return Tuple.Create<long, string, string, string, string>(
                tableID, tableName, columnName, dataOriginal?.ToString(), dataUpdated?.ToString());
        }
    }
}
