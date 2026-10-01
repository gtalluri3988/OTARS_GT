using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Builders.Interfaces;
using System.Linq.Expressions;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Repositories.Interfaces;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Validators.Shared.Fluent;
using MTAoarsGeneral.Validators.Operations.Fluents;
using MTAoarsGeneral.ViewModels.Maintenance;
using System.Globalization;

namespace MTAoarsGeneral.Builders.Operations
{
    public abstract class RegistrationCreator
    {

        public List<string> Parameters { get; set; }

        private RegistrationViewModel Model;
        private ILookupRepository lookupRepository;

        private IUnityContainer container;
        public virtual Dictionary<string, string> ErrorHandler { get; set; }
        protected Dictionary<string, int> ValueIndexes;

        public RegistrationCreator(ILookupRepository lookupRepository, IUnityContainer container)
        {
            this.lookupRepository = lookupRepository;
            this.container = container;

        }


        public virtual void Process(RegistrationViewModel model)
        {
            this.Model = model;
            this.ErrorHandler = new Dictionary<string, string>();
            this.ValueIndexes = new Dictionary<string, int>();

            ValueIndexes.Add("AgencyViewModel.BusinesssRegistrationNumber", 0);
            ValueIndexes.Add("RegistrationViewModel.AgencyType", 2);
            ValueIndexes.Add("AgencyViewModel.AuthorizedCapital", 4);
            ValueIndexes.Add("AgencyViewModel.PaidupCapital", 5);
            ValueIndexes.Add("AgencyViewModel.Name", 6);
            ValueIndexes.Add("RegistrationViewModel.CompanyID", 7);
            ValueIndexes.Add("AddressViewModel.Address1", 8);
            ValueIndexes.Add("AddressViewModel.Address2", 9);
            ValueIndexes.Add("AddressViewModel.City", 10);
            ValueIndexes.Add("AddressViewModel.PostalCode", 12);
            ValueIndexes.Add("AddressViewModel.State", 11);
            ValueIndexes.Add("CorporateNomineeViewModel.Email", 22);
            ValueIndexes.Add("CorporateNomineeViewModel.Fax", 25);
            ValueIndexes.Add("CorporateNomineeViewModel.Gender", 17);
            ValueIndexes.Add("CorporateNomineeViewModel.IsBancaStaff", 26);
            ValueIndexes.Add("CorporateNomineeViewModel.IsCitizen", 28);
            ValueIndexes.Add("CorporateNomineeViewModel.JoinedOn", 13);
            ValueIndexes.Add("CorporateNomineeViewModel.Level", 30);
            ValueIndexes.Add("CorporateNomineeViewModel.MaritalStatus", 19);
            ValueIndexes.Add("CorporateNomineeViewModel.Mobile", 23);
            ValueIndexes.Add("CorporateNomineeViewModel.Phone", 24);
            ValueIndexes.Add("CorporateNomineeViewModel.NewICNumber", 14);
            ValueIndexes.Add("CorporateNomineeViewModel.OldICNumber", 15);
            ValueIndexes.Add("CorporateNomineeViewModel.Race", 27);
            ValueIndexes.Add("CorporateNomineeViewModel.Religion", 21);
            ValueIndexes.Add("CorporateNomineeViewModel.TBECategory", 29);
            ValueIndexes.Add("CorporateNomineeViewModel.IsBumiputera", 20);
            ValueIndexes.Add("CorporateNomineeViewModel.BirthDate", 18);
            ValueIndexes.Add("CorporateNomineeViewModel.ICType", 33);
            ValueIndexes.Add("CorporateNomineeViewModel.Name", 32);
            ValueIndexes.Add("CorporateNomineeViewModel.PassportNumber", 31);
            ValueIndexes.Add("CorporateNomineeViewModel.Experiences", 53);

            ValueIndexes.Add("SpouseViewModel.Gender", 38);
            ValueIndexes.Add("SpouseViewModel.IsBumiputera", 42);
            ValueIndexes.Add("SpouseViewModel.MaritalStatusID", 43);
            ValueIndexes.Add("SpouseViewModel.Name", 34);
            ValueIndexes.Add("SpouseViewModel.NewICNumber", 36);
            ValueIndexes.Add("SpouseViewModel.OldICNumber", 35);
            ValueIndexes.Add("SpouseViewModel.Race", 41);
            ValueIndexes.Add("SpouseViewModel.Religion", 40);
            ValueIndexes.Add("SpouseViewModel.BirthDate", 37);



            ValueIndexes.Add("QualificationViewModel.SchoolName", 44);
            ValueIndexes.Add("QualificationViewModel.Year", 45);
            ValueIndexes.Add("QualificationViewModel.EducationalQualification", 46);
            ValueIndexes.Add("QualificationViewModel.InsuranceQualification", 47);
            ValueIndexes.Add("GuarantorViewModel.Details", 48);
            ValueIndexes.Add("GuarantorViewModel.Amount", 49);
            ValueIndexes.Add("GuarantorViewModel.FromDate", 50);
            ValueIndexes.Add("GuarantorViewModel.ToDate", 51);
            ValueIndexes.Add("GuarantorViewModel.GuarantorType", 52);


            ValueIndexes.Add("AgencyBankerViewModel.AccountNumber", 71);
            ValueIndexes.Add("AgencyBankerViewModel.AccountType", 72);
            ValueIndexes.Add("AgencyBankerViewModel.Bank", 65);
            ValueIndexes.Add("AgencyBankerViewModel.AddressViewModel.Address1", 66);
            ValueIndexes.Add("AgencyBankerViewModel.AddressViewModel.Address2", 67);
            ValueIndexes.Add("AgencyBankerViewModel.AddressViewModel.City", 68);
            ValueIndexes.Add("AgencyBankerViewModel.AddressViewModel.PostalCode", 70);
            ValueIndexes.Add("AgencyBankerViewModel.AddressViewModel.State", 69);

            ValueIndexes.Add("IExistableBoardMembers.Directors", 73);
            ValueIndexes.Add("IExistableBoardMembers.Shareholders", 113);
            ValueIndexes.Add("PartnershipRegistrationViewModel.Partners", 73);

            ValueIndexes.Add("AgencyViewModel.MFPC", 5);
            ValueIndexes.Add("AgencyViewModel.FPAM", 5);

            PopulateModel();


        }


        #region Create
        protected virtual void PopulateModel()
        {
            AssignAgencyType();
            AssignAddress();

            AssignAgency();


            AssignCompany();
            AssignCorporateNominee();
            AssignExperience();
            AssignSpouse();
            AssignQualification();
            AssignGuarantor();

            if (Model is IExistableBoardMembers)
            {
                AssignDirectors();
                AssignShareHolders();
            }
            if (Model is IExistableAgencyBanker)
            {
                AssignAgencyBanker();
                AssignPartners();

            }

            Validate();

        }


        protected virtual void AssignAddress()
        {
            try
            {
                //Populate address value
                var address = new AddressViewModel();
                address.Address1 = GetValue(Parameters, GetIndex(p => address.Address1, address));
                address.Address2 = GetValue(Parameters, GetIndex(p => address.Address2, address));
                address.City = GetValue(Parameters, GetIndex(p => address.City, address));
                address.PostalCode = GetValue(Parameters, GetIndex(p => address.PostalCode, address));
                address.State = Get<LookupState>(GetValue(Parameters, GetIndex(p => address.State, address)));
                Model.Address = address;
            }
            catch
            {
            }
        }
        protected virtual void AssignAgency()
        {
            try
            {
                var agency = new AgencyViewModel();
                //Populate agency Values
                agency.BusinessRegistrationNumber = GetValue(Parameters, GetIndex(p => agency.BusinessRegistrationNumber, agency));
                agency.AuthorizedCapital = GetDeciamlValue(Parameters, GetIndex(p => agency.AuthorizedCapital, agency));
                agency.PaidupCapital = GetDeciamlValue(Parameters, GetIndex(p => agency.PaidupCapital, agency));
                agency.Name = GetValue(Parameters, GetIndex(p => agency.Name, agency));
                agency.M2Exam = GetM2Exams(GetValue(Parameters, GetIndex(p => p.M2Exam, agency)));
                agency.DateOfExam = GetDate(Parameters, GetIndex(p => p.DateOfExam, agency));
                agency.JoinYear = new LookupItem { ID = Convert.ToInt16(GetValue(Parameters, GetIndex(p => p.JoinYear, agency))), Code = GetValue(Parameters, GetIndex(p => p.JoinYear, agency)) };
                agency.JoinMonth = GetAllMonth(GetValue(Parameters, GetIndex(p => p.JoinMonth, agency)));
                agency.SocialMediaAddress = GetValue(Parameters, GetIndex(p => p.SocialMediaAddress, agency));
                agency.MTAAwards = GetLookupList<LookupMTAAward>(GetValue(Parameters, GetIndex(p => p.MTAAwards, agency)));

                Model.Agency = agency;
            }
            catch (Exception)
            {
                ErrorHandler.Add("Agency", "Error at create agency");
                //Log exception
            }
        }
        public LookupItem GetM2Exams(string value)
        {
            var list = new List<LookupItem>{   new LookupItem { ID = 1, Code = "MFPC", Description = "Malaysian Financial Planning Council (MFPC)" },
           new LookupItem { ID = 2, Code = "FPAM", Description = "Financial Planning Association of Malaysia (FPAM)" },
            new LookupItem { ID = 3, Code = "Exempted", Description = "Exempted" },
            new LookupItem { ID = 4, Code = "NotYetCompleted", Description = "Not yet completed" }
        };

            return list.FirstOrDefault(x => x.Code.ToUpper() == value?.ToUpper());

        }
        public LookupItem GetAllMonth(string value)
        {
            var current = DateTime.Now;
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            var months = new List<LookupItem>();
            for (int i = 1; i < 13; i++)
            {
                months.Add(new LookupItem { ID = i, Code = dtfi.GetMonthName(i).ToString(), Description = dtfi.GetMonthName(i) });
                //current = DateTime.Now.AddMonths(-1);
            }
            return months.FirstOrDefault(x => x.Code.ToUpper() == value?.ToUpper());

        }
        public LookupItem GetYN(string code)
        {
            var list = new List<LookupItem>{ new LookupItem { ID = 1, Code = "Y", Description = "Y" },
            new LookupItem { ID = 2, Code = "N", Description = "N" } };

            return list.FirstOrDefault(x => x.Code == code);

        }
        protected virtual void AssignAgencyType()
        {


            Model.AgencyType = Get<LookupAgencyType>(GetValue(Parameters, GetIndex(p => Model.AgencyType, Model)));
            if (Model.AgencyType == null)
            {
                Model.AgencyType = new LookupItem();
            }
        }
        protected virtual void AssignCompany()
        {
            try
            {
                var companyRepository = this.container.Resolve<ICompanyRepository>();
                //Model.CompanyID = GetLongValue(Parameters, GetIndex(p => Model.CompanyID, Model));
                Model.CompanyID = companyRepository.Get(GetValue(Parameters, GetIndex(p => Model.CompanyID, Model))).ID;
            }
            catch
            {
                //Invalid Company ID
            }
        }

        protected virtual void AssignCorporateNominee()
        {
            try
            {
                var nominee = new CorporateNomineeViewModel();
                //Todo se t BirthDate

                nominee.Email = GetValue(Parameters, GetIndex(p => nominee.Email, nominee));
                nominee.Fax = GetValue(Parameters, GetIndex(p => p.Fax, nominee));
                nominee.Gender = Get<LookupGender>(GetValue(Parameters, GetIndex(p => p.Gender, nominee)));
                //Todo set ICTYpe
                nominee.ICType = Get<LookupICType>(GetValue(Parameters, GetIndex(p => p.ICType, nominee)));
                nominee.IsBancaStaff = GetBoolValue(Parameters, GetIndex(p => p.IsBancaStaff, nominee));
                nominee.IsBumiputera = GetBoolValue(Parameters, GetIndex(p => p.IsBumiputera, nominee));
                nominee.IsCitizen = GetBoolValue(Parameters, GetIndex(p => p.IsCitizen, nominee));
                //nominee.JoinedOn = GetDate(Parameters, GetIndex(p => p.JoinedOn, nominee));
                nominee.Level = Get<LookupAgencyType>(GetValue(Parameters, GetIndex(p => p.Level, nominee)));
                nominee.MaritalStatus = Get<LookupMaritalStatus>(GetValue(Parameters, GetIndex(p => p.MaritalStatus, nominee)));
                nominee.Mobile = GetValue(Parameters, GetIndex(p => p.Mobile, nominee));
                nominee.Phone = GetValue(Parameters, GetIndex(p => p.Phone, nominee));
                //Todo corporate nominee namenominee.Name
                nominee.Name = GetValue(Parameters, GetIndex(p => p.Name, nominee));
                nominee.NewICNumber = GetValue(Parameters, GetIndex(p => p.NewICNumber, nominee));
                nominee.OldICNumber = GetValue(Parameters, GetIndex(p => p.OldICNumber, nominee));

                //todo set passport number
                nominee.PassportNumber = GetValue(Parameters, GetIndex(p => p.PassportNumber, nominee));
                nominee.Race = Get<LookupRace>(GetValue(Parameters, GetIndex(p => p.Race, nominee)));
                nominee.Religion = Get<LookupReligion>(GetValue(Parameters, GetIndex(p => p.Religion, nominee)));
                nominee.TbeCategory = Get<LookupTbeCategory>(GetValue(Parameters, GetIndex(p => p.TbeCategory, nominee)));
                nominee.BirthDate = GetDate(Parameters, GetIndex(p => p.BirthDate, nominee));
                Model.CorporateNominee = nominee;


            }
            catch
            {
            }
        }

        protected virtual void AssignExperience()
        {
            var nominee = Model.CorporateNominee;
            var index = GetIndex(p => p.Experiences, nominee);
            var experiences = new List<MemberExperienceViewModel>();
            for (var i = index; i < index + 11; i = i + 4)
            {
                var experience = new MemberExperienceViewModel();
                experience.CompanyID = GetLongValue(Parameters, index);
                experience.AgencyCode = GetValue(Parameters, index + 1);
                if (GetDate(Parameters, index + 2).HasValue)
                {
                    experience.ValidFrom = GetDate(Parameters, index + 2).Value;

                }
                if (GetDate(Parameters, index + 3).HasValue)
                {
                    experience.ValidTo = GetDate(Parameters, index + 3).Value;
                }
                if (experience.CompanyID != 0)
                {
                    experiences.Add(experience);
                }
            }
            nominee.Experiences = experiences;
        }

        protected virtual void AssignSpouse()
        {
            var nominee = Model.CorporateNominee;
            var spouse = new SpouseViewModel();

            spouse.Gender = Get<LookupGender>(GetValue(Parameters, GetIndex(p => p.Gender, spouse)));
            spouse.IsBumiputera = GetBoolValue(Parameters, GetIndex(p => p.IsBumiputera, spouse));
            spouse.IsCitizen = GetBoolValue(Parameters, GetIndex(p => p.IsCitizen, spouse));
            var maritalStatus = Get<LookupMaritalStatus>(GetValue(Parameters, GetIndex(p => p.MaritalStatus, spouse)));
            //if(maritalStatus!=null)
            //{
            //    spouse.MaritalStatusID = maritalStatus.ID;
            //}
            spouse.Name = GetValue(Parameters, GetIndex(p => p.Name, spouse));
            spouse.NewICNumber = GetValue(Parameters, GetIndex(p => p.NewICNumber, spouse));
            spouse.OldICNumber = GetValue(Parameters, GetIndex(p => p.OldICNumber, spouse));
            spouse.Race = Get<LookupRace>(GetValue(Parameters, GetIndex(p => p.Race, spouse)));
            spouse.Religion = Get<LookupReligion>(GetValue(Parameters, GetIndex(p => p.Religion, spouse)));
            spouse.BirthDate = GetDate(Parameters, GetIndex(p => p.BirthDate, spouse));
            nominee.Spouse = spouse;
        }

        protected virtual void AssignQualification()
        {
            var nominee = Model.CorporateNominee;
            var qualification = new QualificationViewModel();
            qualification.SchoolName = GetValue(Parameters, GetIndex(p => p.SchoolName, qualification));
            qualification.Year = GetIntValue(Parameters, GetIndex(p => p.Year, qualification));
            qualification.EducationalQualification = Get<LookupEducationalQualification>(GetValue(Parameters, GetIndex(p => p.EducationalQualification, qualification)));

            nominee.Qualification = qualification;

        }

        protected virtual void AssignGuarantor()
        {

            var guarantor = new GuarantorViewModel();
            guarantor.Details = GetValue(Parameters, GetIndex(p => p.Details, guarantor));
            guarantor.Amount = GetDeciamlValue(Parameters, GetIndex(p => p.Amount, guarantor));
            guarantor.FromDate = GetDate(Parameters, GetIndex(p => p.FromDate, guarantor));
            guarantor.ToDate = GetDate(Parameters, GetIndex(p => p.ToDate, guarantor));
            guarantor.GuarantorType = Get<LookupGuarantorType>(GetValue(Parameters, GetIndex(p => p.GuarantorType, guarantor)));
            Model.Guarantor = guarantor;
        }

        protected virtual void AssignPartners()
        {

            var boardMemberModel = Model as PartnershipRegistrationViewModel;
            if (boardMemberModel != null)
            {
                var partners = new List<PartnerViewModel>();
                var index = GetIndex(p => p.Partners, boardMemberModel);
                for (var i = index; i < index + 40; i = i + 4)
                {
                    var partner = new PartnerViewModel();
                    partner.Name = GetValue(Parameters, i);
                    partner.ICNumber = GetValue(Parameters, i + 1);
                    partner.OldICNumber = GetValue(Parameters, i + 2);
                    partner.ICType = Get<LookupICType>(GetValue(Parameters, i + 3));

                    if (!string.IsNullOrEmpty(partner.Name))
                    {
                        partners.Add(partner);
                    }
                }
                boardMemberModel.Partners = partners;
            }
        }
        protected virtual void AssignDirectors()
        {
            var boardMemberModel = Model as IExistableBoardMembers;
            var directors = new List<DirectorViewModel>();
            var index = GetIndex(p => p.Directors, boardMemberModel);
            for (var i = index; i < index + 40; i = i + 4)
            {
                var director = new DirectorViewModel();
                director.Name = GetValue(Parameters, i);
                director.ICNumber = GetValue(Parameters, i + 1);
                director.OldICNumber = GetValue(Parameters, i + 2);
                director.ICType = Get<LookupICType>(GetValue(Parameters, i + 3));
                if (!string.IsNullOrEmpty(director.Name))
                {
                    directors.Add(director);
                }
            }
            boardMemberModel.Directors = directors;
        }

        protected virtual void AssignShareHolders()
        {
            var boardMemberModel = Model as IExistableBoardMembers;
            var shareHolders = new List<ShareholderViewModel>();
            var index = GetIndex(p => p.Shareholders, boardMemberModel);
            for (var i = index; i < index + 50; i = i + 6)
            {
                var shareholder = new ShareholderViewModel();
                shareholder.Name = GetValue(Parameters, i);
                shareholder.ICType = Get<LookupICType>(GetValue(Parameters, i + 1));
                shareholder.ICNumber = GetValue(Parameters, i + 2);
                shareholder.OldICNumber = GetValue(Parameters, i + 3);
                shareholder.ShareAmount = GetDeciamlValue(Parameters, i + 4);
                shareholder.SharePercentage = GetDeciamlValue(Parameters, i + 5);
                if (!string.IsNullOrEmpty(shareholder.Name))
                {
                    shareHolders.Add(shareholder);
                }
            }
            boardMemberModel.Shareholders = shareHolders;
        }

        protected virtual void AssignAgencyBanker()
        {

            var agencyBanker = new AgencyBankerViewModel();
            agencyBanker.AccountNumber = GetValue(Parameters, GetIndex(p => p.AccountNumber, agencyBanker));
            agencyBanker.Bank = Get<LookupBank>(GetValue(Parameters, GetIndex(p => p.Bank, agencyBanker)));
            agencyBanker.AccountType = Get<LookupBankAccountType>(GetValue(Parameters, GetIndex(p => p.AccountType, agencyBanker)));
            agencyBanker.Address = new AddressViewModel();
            agencyBanker.Address.Address1 = GetValue(Parameters, GetIndex(p => p.Address.Address1, agencyBanker));
            agencyBanker.Address.Address2 = GetValue(Parameters, GetIndex(p => p.Address.Address2, agencyBanker));
            agencyBanker.Address.City = GetValue(Parameters, GetIndex(p => p.Address.City, agencyBanker));
            agencyBanker.Address.State = Get<LookupState>(GetValue(Parameters, GetIndex(p => p.Address.State, agencyBanker)));
            agencyBanker.Address.PostalCode = GetValue(Parameters, GetIndex(p => p.Address.PostalCode, agencyBanker));

            ((IExistableAgencyBanker)Model).AgencyBanker = agencyBanker;
        }
        # endregion

        #region validate
        public virtual void Validate()
        {

            //Validate Address
            ValidateAddress();
            //Validate  corporate nominee
            ValidateNominee();
            //Validate spouse
            ValidateSpouse();
            //validate Guarantee
            ValidateGuarantee();
        }

        public virtual void ValidateAddress()
        {
            var validator = container.Resolve<AddressViewModelValidator>();
            var errors = validator.Validate(Model.Address).Errors.Select(e => new { Name = e.PropertyName, Message = e.ErrorMessage });
            AssignErrors(errors);
        }

        public virtual void ValidateNominee()
        {
            var validator = container.Resolve<CorporateNomineeViewModelValidator>();
            var errors = validator.Validate(Model.CorporateNominee).Errors.Select(e => new { Name = e.PropertyName, Message = e.ErrorMessage });
            AssignErrors(errors);
        }

        public virtual void ValidateSpouse()
        {
            var validator = container.Resolve<SpouseViewModelValidator>();
            var errors = validator.Validate(Model.CorporateNominee.Spouse).Errors.Select(e => new { Name = e.PropertyName, Message = e.ErrorMessage });
            AssignErrors(errors);
        }
        public virtual void ValidateGuarantee()
        {
            var validator = container.Resolve<GuarantorViewModelValidator>();
            var errors = validator.Validate(Model.Guarantor).Errors.Select(e => new { Name = e.PropertyName, Message = e.ErrorMessage });
            AssignErrors(errors);
        }
        public virtual void AssignErrors(dynamic errors)
        {
            foreach (var error in errors)
            {
                ErrorHandler.Add(error.Name, error.Message);
            }
        }

        #endregion

        #region GetValues

        private string GetValue(List<string> parameters, int index)
        {
            var value = string.Empty;
            if (parameters.Count > index && index != -1)
            {
                value = parameters[index];
            }
            return value;
        }

        private decimal GetDeciamlValue(List<string> parameters, int index)
        {
            decimal result;
            var value = GetValue(parameters, index);
            decimal.TryParse(value, out result);
            return result;
        }

        private long GetLongValue(List<string> parameters, int index)
        {
            long result;
            var value = GetValue(parameters, index); ;

            long.TryParse(value, out result);
            return result;
        }


        private int GetIntValue(List<string> parameters, int index)
        {
            int result;
            var value = GetValue(parameters, index);
            int.TryParse(value, out result);
            return result;
        }
        private bool GetBoolValue(List<String> parameters, int index)
        {
            return Convert.ToBoolean(GetLongValue(parameters, index));
        }

        private DateTime? GetDate(List<string> parameters, int index)
        {
            DateTime result;
            var value = GetValue(parameters, index);
            if (!string.IsNullOrEmpty(value))
            {
                switch (value.Length)
                {
                    case 7:
                        value = value.Insert(1, "-");
                        value = value.Insert(4, "-");
                        break;
                    case 8:
                        value = value.Insert(2, "-");
                        value = value.Insert(5, "-");
                        break;
                    default:
                        break;
                }
            }
            DateTime.TryParse(value, out result);
            if (result == DateTime.MinValue)
            {
                return null;
            }
            return result;
        }

        private int GetIndex<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression, TModel model)
        {
            var propertyName = string.Empty;
            var index = -1;
            MemberExpression member = expression.Body as MemberExpression;
            if (member != null)
            {
                propertyName = (member.Expression.Type.Name == member.Member.ReflectedType.Name ? member.Member.ReflectedType.Name : member.Expression.Type.Name + "." + member.Member.ReflectedType.Name) + "." + member.Member.Name;
            }
            if (!string.IsNullOrEmpty(propertyName))
            {

                if (!ValueIndexes.TryGetValue(propertyName, out index))
                {
                    index = -1;
                }
            }

            return index;
        }


        private LookupItem Get<T>(string code) where T : EntityObject, ILookupEntity
        {

            return AutoMapper.Mapper.Map<T, LookupItem>(lookupRepository.Get<T>(code));

        }

        #endregion

        List<LookupItem> GetLookupList<T>(string code) where T : EntityObject, ILookupEntity
        {
            List<LookupItem> items = new List<LookupItem>();
            if (!string.IsNullOrEmpty(code))
            {
                var modelItems = lookupRepository.GetAll<T>().Where(x => code.Split(';').Contains(x.Code)).ToList();
                modelItems.ForEach(x => items.Add(new LookupItem { Code = x.Code, Description = x.Description, ID = x.ID }));
            }
            return items;
        }
    }
}
