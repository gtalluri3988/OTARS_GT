using AutoMapper;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using System.Collections.Generic;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Repositories.Reports;
using System;
using MTAoarsGeneral.Builders.Shared;
using MTAoarsGeneral.Validators;
using System.Linq;
using MTAoarsGeneral.Utilities.Config;
namespace MTAoarsGeneral.Builders.Operations
{
    public class AgencyBuilder : BaseBuilder, IAgencyBuilder
    {

        IAgencyRepository agencyRepository;
        IRepository<AgencyPrincipal> agencyPrincipalRepository;
        IScopeDataProvider dataProvider;
        ILookupRepository lookupRepository;
        IMemberRepository memberRepository;
        IIbfimResultRepository ibfimResultRepository;
        IMiiResultRepository miiResultRepository;
        ICompanyRepository companyRepository;
        IPhotoRepository photoRepository;
        IUserRepository userRepository;
        ConfigManager config;

        public AgencyBuilder(IAgencyRepository agencyRepository, IRepository<AgencyPrincipal> agencyPrincipalRepository, IScopeDataProvider dataProvider, ILookupRepository lookupRepository, IMemberRepository memberRepository,
            IIbfimResultRepository iibfimResultRepository, IMiiResultRepository miiResultRepository, ICompanyRepository companyRepository,
            IPhotoRepository photoRepository, IUserRepository userRepository, ConfigManager config)
        {
            this.agencyRepository = agencyRepository;
            this.agencyPrincipalRepository = agencyPrincipalRepository;
            this.dataProvider = dataProvider;
            this.lookupRepository = lookupRepository;
            this.memberRepository = memberRepository;
            this.ibfimResultRepository = iibfimResultRepository;
            this.miiResultRepository = miiResultRepository;
            this.companyRepository = companyRepository;
            this.photoRepository = photoRepository;
            this.userRepository = userRepository;
            this.config = config;
        }

        public AgencyDisplayViewModel GetAgency(long agencyId)
        {
            var agency = agencyRepository.Get(agencyId);
            var viewModel = Mapper.Map<AgencyDisplayViewModel>(agency);
            if (agency != null && agency.AgencyMembers != null)
            {
                //[2021-02-18] - if current user is not ISM/MTA, then check Company intermediaryType with referred member listing
                var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
                var companyID = (identity.IsISMOrMTA) ? (long?)null : identity.CompanyID;

                agency.AgencyMembers.Each((m) =>
                {
                    if (memberRepository.IsMemberReferred(m.Member.OldICNumber, companyID) ||
                        memberRepository.IsMemberReferred(m.Member.NewICNumber, companyID))
                    {
                        viewModel.IsReferred = true;
                        var referredMembers = memberRepository.GetReferredMembers(m.Member.NewICNumber, companyID);
                        var referredMemberList = new List<ReferredMemberViewModel>();
                        foreach (var refMember in referredMembers)
                            referredMemberList.Add(Mapper.Map<ReferredMemberViewModel>(refMember));
                        viewModel.ReferredMembers = referredMemberList;
                    }

                    if (!string.IsNullOrEmpty(m.Member.OldICNumber))
                        viewModel.ICNumber = m.Member.OldICNumber;
                    if (!string.IsNullOrEmpty(m.Member.NewICNumber))
                        viewModel.ICNumber = m.Member.NewICNumber;
                });
            }
            if (viewModel != null && viewModel.AgencyPrincipals != null)
            {
                viewModel.AgencyPrincipals.Each((m) =>
                {
                    m.PhotoPath = string.IsNullOrEmpty(m.PhotoPath) ? "" : m.PhotoPath.Replace(config.UploadBaseDirectory + @"\Photo\", config.PhotoUrl);
                });
            }

            return viewModel;
        }
        public AgencyDisplayViewModel GetAgency(long agencyId, long agencyPrincipalID, long memberID)
        {
            var agency = agencyRepository.Get(agencyId);
            var viewModel = Mapper.Map<AgencyDisplayViewModel>(agency);

            /* 20200318 - Re-Map norminee by MemberID */
            if (viewModel.Nominee.ID != memberID)
            {
                logger.Info("Remap norminee from memberID: {0}, to memberID: {1}", viewModel.Nominee.ID, memberID);
                var member = memberRepository.Get(memberID);
                var am = agency.AgencyMembers.First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
                var result = Mapper.Map<MemberDisplayViewModel>(member);
                result.IsPartTime = am.IsPartTime.GetValueOrDefault();
                result.Qualification = new QualificationDisplayViewModel();
                var qualification = member.MemberEducationalQualifications.FirstOrDefault();
                if (qualification != null)
                {
                    result.Qualification.EducationalQualification = Mapper.Map<LookupItem>(qualification.LookupEducationalQualification);
                    result.Qualification.SchoolName = qualification.SchoolName;
                    result.Qualification.Year = qualification.Year;
                }
                var ins = member.MemberInsuranceQualifications.FirstOrDefault();
                if (ins != null)
                    result.Qualification.InsuranceQualification = ins.LookupInsuranceQualification.Description;
                viewModel.Nominee = result;
            }


            if (agency != null && agency.AgencyMembers != null)
            {
                agency.AgencyMembers.Each((m) =>
                {
                    if (memberRepository.IsMemberReferred(m.Member.OldICNumber) || memberRepository.IsMemberReferred(m.Member.NewICNumber))
                    {
                        viewModel.IsReferred = true;
                        var referredMembers = memberRepository.GetReferredMembers(m.Member.NewICNumber);
                        var referredMemberList = new List<ReferredMemberViewModel>();
                        foreach (var refMember in referredMembers)
                            referredMemberList.Add(Mapper.Map<ReferredMemberViewModel>(refMember));
                        viewModel.ReferredMembers = referredMemberList;
                    }

                    if (!string.IsNullOrEmpty(m.Member.OldICNumber))
                        viewModel.ICNumber = m.Member.OldICNumber;
                    if (!string.IsNullOrEmpty(m.Member.NewICNumber))
                        viewModel.ICNumber = m.Member.NewICNumber;
                });
            }
            if (viewModel != null && viewModel.AgencyPrincipals != null)
            {
                viewModel.AgencyPrincipals.Each((m) =>
                {
                    m.PhotoPath = string.IsNullOrEmpty(m.PhotoPath) ? "" : m.PhotoPath.Replace(config.UploadBaseDirectory + @"\Photo\", config.PhotoUrl);
                });
            }

            return viewModel;
        }

        public AgencyDisplayViewModel GetAgencyByPrincipal(long principalId)
        {
            var principal = agencyPrincipalRepository.Get(principalId);
            var agency = agencyRepository.Get(principal.AgencyID);
            var result = Mapper.Map<AgencyDisplayViewModel>(agency);
            result.DateAppointed = principal.DateAppointed;
            return result;
        }

        public AgencyDisplayViewModel GetAgency(RegistrationViewModel model)
        {
            var result = Mapper.Map<AgencyDisplayViewModel>(model);
            //result.Address.State = model.Address.State.Description;
            // result.TypeDescription = lookupRepository.Get<LookupAgencyType>(result.TypeCode).Description;
            result.Directors = new List<MemberDisplayViewModel>();
            result.Shareholders = new List<MemberDisplayViewModel>();
            result.Partners = new List<MemberDisplayViewModel>();
            result.Nominee = Mapper.Map<MemberDisplayViewModel>(model.CorporateNominee);
            if (model.AgencyType.Code == LookupConstants.AgencyType.Individual)
            {
                result.Name = model.CorporateNominee.Name;
                result.Spouse = Mapper.Map<MemberDisplayViewModel>(model.CorporateNominee.Spouse);
            }
            if (model as IExistableBoardMembers != null)
            {
                var boardMembers = model as IExistableBoardMembers;
                foreach (var director in boardMembers.Directors)
                {
                    result.Directors.Add(Mapper.Map<MemberDisplayViewModel>(director));
                }
                foreach (var shareholder in boardMembers.Shareholders)
                {
                    result.Shareholders.Add(Mapper.Map<MemberDisplayViewModel>(shareholder));
                }
            }
            if (model as PartnershipRegistrationViewModel != null)
            {
                var partnerModel = model as PartnershipRegistrationViewModel;
                result.Partners.Add(result.Nominee);
                foreach (var partner in partnerModel.Partners)
                {
                    result.Partners.Add(Mapper.Map<MemberDisplayViewModel>(partner));
                }
            }
            result.Nominee.Qualification = Mapper.Map<QualificationDisplayViewModel>(model.CorporateNominee.Qualification);
            PopualteQualification(result.Nominee.Qualification, model.CorporateNominee.Qualification);
            result.Guarantor = Mapper.Map<GuarantorDisplayViewModel>(model.Guarantor);
            PopulateGuarantor(result.Guarantor, model.Guarantor);

            return result;
        }

        public IEnumerable<T> Search<T>(AgencySearchRequestViewModel request)
        {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var companyId = identity.CompanyID;
            var result = agencyRepository.Search(request.AgencyNumber, request.MemberName, request.ICNumber,
                request.BusinessRegistrationNumber, request.NewBusinessRegistrationNumber, companyId, isGeneral: true, isFamily: false);

            var output = new List<T>();
            foreach (var item in result)
            {
                if (item.CompanyID != companyId && !identity.IsISMOrMTA)
                    item.AgencyID = 0;
                /*if (identity.IsISMOrMTA)
                    item.IsReferredMember = false;*/
                output.Add(Mapper.Map<T>(item));
            }
            return output;
        }

        public IEnumerable<T> SearchArchive<T>(AgencySearchRequestViewModel request)
        {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var companyId = identity.CompanyID;
            var resultArchive = agencyRepository.SearchArchive(request.AgencyNumber, request.MemberName, request.ICNumber, request.BusinessRegistrationNumber, companyId); /* [License Split] -Added */
            var output = new List<T>();
            foreach (var item in resultArchive)
            {
                if (item.CompanyID != companyId && !identity.IsISMOrMTA)
                    item.AgencyID = 0;
                output.Add(Mapper.Map<T>(item));
            }
            return output;
        }

        public TBEEnquirySearchViewModel Search(string icNumber)
        {
            var searchResult = new TBEEnquirySearchViewModel();
            var output = new List<TBEEnquiryResponseViewModel>();
            var isIbfimfound = false;
            var isMIIFound = false;
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var companies = companyRepository.GetAll().ToList();
            var companyAbbreviation = companyRepository.GetAbbreviations(identity.CompanyID);

            var ibfimResults = ibfimResultRepository.Search(icNumber);
            foreach (var item in ibfimResults)
            {
                isIbfimfound = true;
                //All TO's can view all TBE result [2019-01-31]
                //if (!identity.IsISMOrMTA) {
                //    if (item.CompanyCode != identity.CompanyCode && !string.IsNullOrEmpty(item.CompanyCode))
                //        continue;
                //}
                var result = Mapper.Map<TBEEnquiryResponseViewModel>(item);
                result.Exam = LookupConstants.TbeCategory.IBFIM;
                var company = companies.FirstOrDefault(c => c.Abbreviation == result.CompanyCode);
                if (company != null)
                    result.CompanyName = company.Name;
                output.Add(result);
            }
            var miiResults = miiResultRepository.Search(icNumber);
            foreach (var item in miiResults)
            {
                isMIIFound = true;
                //All TO's can view all TBE result [2019-01-31]
                //if (!identity.IsISMOrMTA) {
                //    if (item.CompanyCode != identity.CompanyCode && !string.IsNullOrEmpty(item.CompanyCode))
                //        continue;
                //}
                var result = Mapper.Map<TBEEnquiryResponseViewModel>(item);
                result.Exam = LookupConstants.TbeCategory.MII;
                var company = companies.FirstOrDefault(c => c.Abbreviation == result.CompanyCode);
                if (company != null)
                    result.CompanyName = company.Name;
                output.Add(result);
            }
            if (!isIbfimfound && !isMIIFound)
                searchResult.Message = "No Record Found";
            if ((isIbfimfound || isMIIFound) && output.Count == 0)
                searchResult.Message = "This candidate is not sponsered by your company. Please check with MTA";
            searchResult.Results = output;
            return searchResult;

        }

        //public List<TBEEnquiryResponseViewModel> SearchTBEResult(string icNumber)
        //{
        //    var SearchResult = new List<TBEEnquiryResponseViewModel>();

        //    var output = new List<TBEEnquiryResponseViewModel>();

        //    var ibfimResults = ibfimResultRepository.Search(icNumber);
        //    var isIbfimfound = false;
        //    var isMIIFound = false;
        //    var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
        //    var companies = companyRepository.GetAll().ToList();
        //    foreach (var item in ibfimResults)
        //    {
        //        isIbfimfound = true;
        //        if (!identity.IsISMOrMTA)
        //        {
        //            if (item.CompanyCode != identity.CompanyCode && !string.IsNullOrEmpty(item.CompanyCode))
        //                continue;
        //        }
        //        var result = Mapper.Map<TBEEnquiryResponseViewModel>(item);
        //        result.Exam = LookupConstants.TbeCategory.IBFIM;
        //        var company = companies.SingleOrDefault(c => c.Abbreviation == result.CompanyCode);
        //        if (company != null)
        //            result.CompanyName = company.Name;
        //        output.Add(result);
        //    }
        //    var miiResults = miiResultRepository.Search(icNumber);
        //    foreach (var item in miiResults)
        //    {
        //        isMIIFound = true;
        //        if (!identity.IsISMOrMTA)
        //        {
        //            if (item.CompanyCode != identity.CompanyCode && !string.IsNullOrEmpty(item.CompanyCode))
        //                continue;
        //        }
        //        var result = Mapper.Map<TBEEnquiryResponseViewModel>(item);
        //        result.Exam = LookupConstants.TbeCategory.MII;
        //        var company = companies.SingleOrDefault(c => c.Abbreviation == result.CompanyCode);
        //        if (company != null)
        //            result.CompanyName = company.Name;
        //        output.Add(result);
        //    }
        //    if (!isIbfimfound && !isMIIFound)
        //        SearchResult.TBEResult.Message = "No Record Found";
        //    if ((isIbfimfound || isMIIFound) && output.Count == 0)
        //        SearchResult.TBEResult.Message = "This candidate is not sponsered by your company. Please check with MTA";
        //    SearchResult.TBEResult.Results = output;

        //    return SearchResult;
        //}

        public TBEEnquiryResponseViewModel getResult(int ID, string Exam)
        {
            var searchResult = new TBEEnquiryResponseViewModel();
            string Type = null;
            dynamic ResultItems = "";
            switch (Exam)
            {
                case LookupConstants.TbeCategory.MII:
                    ResultItems = miiResultRepository.Search(ID);
                    Type = LookupConstants.TbeCategory.MII;
                    break;
                case LookupConstants.TbeCategory.IBFIM:
                    ResultItems = ibfimResultRepository.Search(ID);
                    Type = LookupConstants.TbeCategory.IBFIM;
                    break;
            }
            var result = Mapper.Map<TBEEnquiryResponseViewModel>(ResultItems);

            searchResult = result;
            searchResult.Exam = Type;
            return searchResult;
        }

        public void SaveTBEResult(TBEEnquirySearchViewModel model, out string message)
        {
            dynamic item;
            var Message = "";
            switch (model.Result.Exam)
            {
                case LookupConstants.TbeCategory.MII:
                    item = new MiiResult
                    {
                        ID = model.Result.ID,
                        ICNumber = model.Result.ICNumber,
                        Name = model.Result.Name.ToUpper(),
                        //Result = model.Result.Result,
                        Grade = model.Result.Grade,
                        ExamDate = model.Result.ExamDate
                    };
                    miiResultRepository.Update(item, out Message);
                    break;
                case LookupConstants.TbeCategory.IBFIM:
                    item = new IbfimResult
                    {
                        ID = model.Result.ID,
                        ICNumber = model.Result.ICNumber,
                        Name = model.Result.Name.ToUpper(),
                        Result = model.Result.Result,
                        Grade = model.Result.Grade,
                        ExamDate = model.Result.ExamDate
                    };
                    ibfimResultRepository.Update(item, out Message);
                    break;
            }
            message = Message;


        }

        public IEnumerable<T> Search<T>(AgencySearchRequestViewModel request, string maintenanceType) where T : MaintenanceSearchResponseViewModel
        {
            var result = new List<T>();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var searchItem = Search<T>(request.AgencyNumber, identity.CompanyID);
            if (searchItem != null && searchItem.CompanyID == identity.CompanyID) result.Add(searchItem);
            IEnumerable<T> output = new List<T>();
            var individualDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.Individual).Description;
            var cooperativeDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.Cooperative).Description;
            var governmentAgencyDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.GovernmentAgency).Description;
            var privateCompanyDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.PrivateLimitedCompany).Description;
            var publicCompanyDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.PublicLimitedCompany).Description;
            var soleProprietorshipDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.SoleProprietorship).Description;
            var partnershipDesc = lookupRepository.Get<LookupAgencyType>(LookupConstants.AgencyType.Partnership).Description;
            switch (maintenanceType)
            {
                case LookupConstants.Maintenance.ChangeNominee:
                    result.Each(item => {
                        if (item.AgencyTypeDescription == cooperativeDesc
                            || item.AgencyTypeDescription == governmentAgencyDesc
                            || item.AgencyTypeDescription == privateCompanyDesc
                            || item.AgencyTypeDescription == publicCompanyDesc
                            || item.AgencyTypeDescription == partnershipDesc
                           )
                        {

                            ((List<T>)output).Add(item);
                        }
                        else
                        {
                            CurrentContext.ValidationMessages.Add(new ValidationMessage("", "Change corporate nominee is not applicable for '{0}' type registration", item.AgencyTypeDescription));
                        }

                    });
                    break;
                case LookupConstants.Maintenance.ChangeAgency:
                    result.Each(item =>
                    {
                        /*if (item.AgencyTypeDescription == cooperativeDesc
                            || item.AgencyTypeDescription == governmentAgencyDesc
                            || item.AgencyTypeDescription == privateCompanyDesc
                            || item.AgencyTypeDescription == publicCompanyDesc
                            ||item.AgencyTypeDescription==soleProprietorshipDesc
                            || item.AgencyTypeDescription == partnershipDesc
                           )
                        {

                            ((List<T>)output).Add(item);
                        }*/
                        ((List<T>)output).Add(item);

                    });
                    break;

                case LookupConstants.Maintenance.ChangeBoardMembers:
                case LookupConstants.Maintenance.ChangeAdditionalCorporateNominee:
                    result.Each(item =>
                    {
                        if (item.AgencyTypeDescription == cooperativeDesc
                            || item.AgencyTypeDescription == governmentAgencyDesc
                            || item.AgencyTypeDescription == privateCompanyDesc
                            || item.AgencyTypeDescription == publicCompanyDesc
                           )
                        {

                            ((List<T>)output).Add(item);
                        }
                        else
                        {
                            CurrentContext.ValidationMessages.Add(new ValidationMessage("", "Change Board Members/Additional Corporate Nominee is not applicable for '{0}' type registration", item.AgencyTypeDescription));
                        }

                    });
                    break;
                case LookupConstants.Maintenance.ChangeCorporateStatus:
                    result.Each(item => {
                        if (item.AgencyTypeDescription == privateCompanyDesc
                            || item.AgencyTypeDescription == publicCompanyDesc
                           )
                        {

                            ((List<T>)output).Add(item);
                        }
                        else
                        {
                            CurrentContext.ValidationMessages.Add(new ValidationMessage("", "Change corporate status is not applicable for '{0}' type registration", item.AgencyTypeDescription));
                        }

                    });
                    break;
                case LookupConstants.Maintenance.ChangeCopmanyName:
                    result.Each(item => {
                        if (item.AgencyTypeDescription != individualDesc)
                        {

                            ((List<T>)output).Add(item);
                        }
                        else
                        {
                            CurrentContext.ValidationMessages.Add(new ValidationMessage("", "Change company name is not applicable for '{0}' type registration", item.AgencyTypeDescription));
                        }

                    });
                    break;
                case LookupConstants.Maintenance.ChangePartner:
                    result.Each(item =>
                    {
                        if (item.AgencyTypeDescription == partnershipDesc
                           )
                        {

                            ((List<T>)output).Add(item);
                        }
                        else
                        {
                            CurrentContext.ValidationMessages.Add(new ValidationMessage("", "Change partner is not applicable for '{0}' type registration", item.AgencyTypeDescription));
                        }

                    });
                    break;
                default:
                    output = result;
                    break;
            }
            return output;
        }

        public T Search<T>(string agencyNumber)
        {
            var item = agencyRepository.Search(agencyNumber);
            return Mapper.Map<T>(item);
        }

        public T SearchHistory<T>(string agencyNumber)
        {
            var item = agencyRepository.SearchHistories(agencyNumber);
            return Mapper.Map<T>(item);
        }

        public T Search<T>(string agencyNumber, long companyId) where T : class
        {
            var output = agencyRepository.Search(agencyNumber);
            if (output == null)
            {
                CurrentContext.ValidationMessages.Add(new ValidationMessage("", "The agent is not available"));
                return null;
            }
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (identity.IsMTA == true) return Mapper.Map<T>(output);
            if (output.CompanyID != identity.CompanyID) CurrentContext.ValidationMessages.Add(new ValidationMessage("", "This agent does not belong to your company"));
            return Mapper.Map<T>(output);
        }

        public IEnumerable<T> Search<T>(DateTime fromDate, DateTime toDate)
        {
            foreach (var item in agencyRepository.Search(fromDate, toDate))
            {
                yield return Mapper.Map<T>(item);
            }
        }

        void PopualteQualification(QualificationDisplayViewModel input, QualificationViewModel qualification)
        {
            //input.EducationalQualification = GetLookupDescription<LookupEducationalQualification>(qualification.EducationalQualificationID);
            //input.InsuranceQualification = GetLookupDescription<LookupInsuranceQualification>(qualification.InsuranceQualificationID);
        }

        void PopulateGuarantor(GuarantorDisplayViewModel input, GuarantorViewModel guarantor)
        {
            //input.TypeDescription = GetLookupDescription<LookupGuarantorType>(guarantor.TypeID);
            //input.TypeCode = GetLookupCode<LookupGuarantorType>(guarantor.TypeID);
        }

        string GetLookupDescription<T>(long? id) where T : EntityObject, ILookupEntity
        {
            if (id.HasValue == false) return null;
            var entity = lookupRepository.Get<T>(id.Value);
            if (entity == null) return null;
            return entity.Description;
        }

        string GetLookupCode<T>(long? id) where T : EntityObject, ILookupEntity
        {
            if (id.HasValue == false) return null;
            var entity = lookupRepository.Get<T>(id.Value);
            if (entity == null) return null;
            return entity.Code;
        }

        public PhotoUploadEnquirySearchViewModel SearchPhotoUpload(string agencyNumber)
        {
            var searchResult = new PhotoUploadEnquirySearchViewModel();
            var output = new List<PhotoUploadEnquiryResponseViewModel>();

            var items = photoRepository.Enquiry(agencyNumber);
            var companies = companyRepository.GetAll().ToList();

            foreach (var item in items)
            {
                var searchAgencyResult = agencyRepository.Search(item.AgencyNumber);
                var result = Mapper.Map<PhotoUploadEnquiryResponseViewModel>(item);
                result.CompanyName = companies.SingleOrDefault(c => c.ID == Convert.ToInt64(searchAgencyResult.CompanyID)).Name;
                if (item.CreatedBy != null)
                    result.UserName = userRepository.Get(Convert.ToInt64(item.CreatedBy)).Name;
                output.Add(result);
            }

            if (items.Count == 0)
                searchResult.Message = "No Record Found";
            searchResult.Results = output;

            return searchResult;
        }

    }// class
}// namespace
