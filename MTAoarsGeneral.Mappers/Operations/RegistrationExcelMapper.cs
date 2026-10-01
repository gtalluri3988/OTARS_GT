using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Utilities.Mvc;
using System.Data.Objects.DataClasses;
using System.Reflection;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MTAoarsGeneral.Mappers.Operations
{
    public class RegistrationExcelMapper : IMapper
    {

        ILookupRepository lookupRepository;
        ICompanyRepository companyRepository;
        Dictionary<string, PropertyInfo> properties;

        public RegistrationExcelMapper(ILookupRepository lookupRepository, ICompanyRepository companyRepository)
        {
            this.lookupRepository = lookupRepository;
            this.companyRepository = companyRepository;
            properties = typeof(RegistrationExcelViewModel).GetProperties().ToDictionary(p => p.Name);
        }

        public void Map()
        {
            MapToIndex();
            MapToAgency();
            MapToAddress();
            MapToCorporateNominee();
            MapToQualification();
            MapToSpouse();
            MapToAgencyBanker();
            MapToGuarantor();
            MapToIndividual();
            MapToSoleProprietorship();
            MapToPartnership();
            MapToCorporate();
            MapToUploadHistory();
        }

        void MapToIndex()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, RegistrationIndexViewModel>()

      .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => GetM2Exams(src.M2Exam)))
              .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => GetAllMonth(src.JoinMonth)))
              .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear }))
              .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => GetLookupList<LookupMTAAward>(src.MTAAwards)))
              .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => ParseDate(src.DateOfExam)))

                .AfterMap((Action<RegistrationExcelViewModel, RegistrationIndexViewModel>)((src, dest) =>
                {
                    dest.AgencyType = GetLookup<LookupAgencyType>(src.AgencyTypeCode);
                    dest.CompanyID = GetCompanyID(src.CompanyCode);
                    dest.ICNumber = src.NomineeICNumber;
                    dest.ICType = GetLookup<LookupICType>(src.NomineeICTypeCode);
                    dest.IsBancaStaff = src.NomineeIsBanacaStaff;
                    dest.IsFamily = false;
                    dest.IsGeneral = true;
                    dest.IsPartTime = src.NomineeIsPartTime;
                    dest.Level = GetLookup<LookupMemberLevel>(src.NomineeRankCode);
                    dest.Name = src.NomineeName;
                    dest.TbeCategory = GetLookup<LookupTbeCategory>(src.NomineeTbeCategoryCode);
                    dest.M2Exam = GetM2Exams(src.M2Exam);
                    dest.DateOfExam = ParseDate(src.DateOfExam);
                    dest.JoinMonth = GetAllMonth(src.JoinMonth);
                    dest.JoinYear = new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear };
                    dest.MTAAwards = GetLookupList<LookupMTAAward>(src.MTAAwards);
                    dest.SocialMediaAddress = src.SocialMediaAddress;

                }));
        }

        void MapToAgency()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, AgencyViewModel>()
              .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => GetM2Exams(src.M2Exam)))
              .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => GetAllMonth(src.JoinMonth)))
              .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear }))
              .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => GetLookupList<LookupMTAAward>(src.MTAAwards)))
              .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => ParseDate(src.DateOfExam)))

              .AfterMap((src, dest) =>
              {
                  dest.IsFamily = false;
                  dest.IsGeneral = true;
                  if (src.AgencyTypeCode == LookupConstants.AgencyType.Individual)
                  {
                      dest.Name = src.NomineeName;
                  }
                  else
                  {
                      dest.Name = src.AgencyName;
                  }
                  dest.M2Exam = GetM2Exams(src.M2Exam);
                  dest.DateOfExam = ParseDate(src.DateOfExam);
                  dest.JoinMonth = GetAllMonth(src.JoinMonth);
                  dest.JoinYear = new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear };
                  dest.MTAAwards = GetLookupList<LookupMTAAward>(src.MTAAwards);
              });
        }

        void MapToAddress()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, AddressViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.Address1 = src.AgencyAddress1;
                    dest.Address2 = src.AgencyAddress2;
                    dest.City = src.AgencyCity;
                    dest.State = GetLookup<LookupState>(src.AgencyStateCode);
                    dest.PostalCode = src.AgencyPostalCode;
                });
        }

        void MapToCorporateNominee()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, CorporateNomineeViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.BirthDate = src.NomineeBirthDate;
                    dest.Email = src.NomineeEmail;
                    dest.Fax = src.NomineeFax;
                    dest.Gender = GetLookup<LookupGender>(src.NomineeGenderCode);
                    dest.ICType = GetLookup<LookupICType>(src.NomineeICTypeCode);
                    dest.IsBancaStaff = src.NomineeIsBanacaStaff;
                    dest.IsBumiputera = src.NomineeIsBumiputera;
                    dest.IsCitizen = src.NomineeIsCitizen;
                    dest.IsPartTime = src.NomineeIsPartTime;
                    dest.Level = GetLookup<LookupMemberLevel>(src.NomineeRankCode);
                    dest.MaritalStatus = GetLookup<LookupMaritalStatus>(src.NomineeMaritalStatusCode);
                    dest.Mobile = src.NomineeMobile;
                    dest.Name = src.NomineeName;
                    if (dest.ICType.Code == LookupConstants.ICTypes.NewIc)
                    {
                        dest.NewICNumber = src.NomineeICNumber;
                    }
                    else
                    {
                        dest.PassportNumber = src.NomineeICNumber;
                    }
                    dest.OldICNumber = src.NomineeOldICNumber;
                    dest.Phone = src.NomineePhone;
                    dest.Qualification = Mapper.Map<QualificationViewModel>(src);
                    dest.Race = GetLookup<LookupRace>(src.NomineeRaceCode);
                    dest.Religion = GetLookup<LookupReligion>(src.NomineeReligionCode);
                    dest.Spouse = Mapper.Map<SpouseViewModel>(src);

                    dest.TbeCategory = GetLookup<LookupTbeCategory>(src.NomineeTbeCategoryCode);
                    dest.IsOld = src.IsOld;
                    //dest.MFPC = src.MFPC;
                    //dest.FPAM = src.FPAM;
                });
        }

        void MapToQualification()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, QualificationViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.EducationalQualification = GetLookup<LookupEducationalQualification>(src.NomineeEducationalQualificationCode);
                    dest.SchoolName = src.NomineeSchoolName;
                    dest.Year = src.NomineeYear;
                });
        }

        void MapToSpouse()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, SpouseViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.BirthDate = src.SpouseBirthDate;
                    dest.Gender = GetLookup<LookupGender>(src.SpouseGenderCode);
                    dest.IsBumiputera = src.SpouseIsBumiputera;
                    dest.IsCitizen = src.SpouseIsCitizen;
                    dest.MaritalStatus = GetLookup<LookupMaritalStatus>(src.SpouseMaritalStatusCode);
                    dest.Name = src.SpouseName;
                    dest.NewICNumber = src.SpouseNewICNumber;
                    dest.OldICNumber = src.SpouseOldICNumber;
                    dest.Race = GetLookup<LookupRace>(src.SpouseRaceCode);
                    dest.Religion = GetLookup<LookupReligion>(src.SpouseReligionCode);
                });
        }

        void MapToAgencyBanker()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, AgencyBankerViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.AccountNumber = src.AccountNumber;
                    dest.AccountType = GetLookup<LookupBankAccountType>(src.AccountTypeCode);
                    dest.Address = new AddressViewModel()
                    {
                        Address1 = src.BankAddress1,
                        Address2 = src.BankAddress2,
                        City = src.BankCity,
                        PostalCode = src.BankPostalCode,
                        State = GetLookup<LookupState>(src.BankStateCode)
                    };
                });
        }

        void MapToGuarantor()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, GuarantorViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.Amount = src.GuarantorAmount;
                    dest.Details = src.GuarantorDetails;
                    dest.FromDate = src.GuarantorFromDate;
                    dest.GuarantorType = GetLookup<LookupGuarantorType>(src.GuarantorTypeCode);
                    dest.ToDate = src.GuarantorToDate;
                });
        }

        void MapToIndividual()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, IndividualRegistrationViewModel>()
                .AfterMap((src, dest) =>
                {
                    Populate(src, dest);
                });
        }

        void MapToSoleProprietorship()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, SoleProprietorshipRegistrationViewModel>()
                .AfterMap((src, dest) =>
                {
                    Populate(src, dest);
                });
        }

        void MapToPartnership()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, PartnershipRegistrationViewModel>()
                .AfterMap((src, dest) =>
                {
                    Populate(src, dest);
                    AssignPartners(src, dest.Partners);
                });
        }

        void MapToCorporate()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, CorporateRegistrationViewModel>()
                .AfterMap((src, dest) =>
                {
                    Populate(src, dest);
                    AssignDirectors(src, dest.Directors);
                    AssignShareholders(src, dest.Shareholders);
                });
        }

        void MapToUploadHistory()
        {
            Mapper.CreateMap<RegistrationExcelViewModel, RegistrationUploadHistory>()
                .ForMember(dest => dest.RawData, opt => opt.MapFrom(src => src.GetRawData()))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.GetErrors().Count == 0))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.GetRemarks()))
                .AfterMap((src, dest) =>
                {
                    dest.Errors = String.Join(",", src.GetErrors().ToArray());
                });


        }

        void Populate(RegistrationExcelViewModel src, RegistrationViewModel dest)
        {
            dest.Agency = Mapper.Map<AgencyViewModel>(src);
            dest.Address = Mapper.Map<AddressViewModel>(src);
            dest.AgencyType = GetLookup<LookupAgencyType>(src.AgencyTypeCode);
            dest.CompanyID = GetCompanyID(src.CompanyCode);
            dest.CorporateNominee = Mapper.Map<CorporateNomineeViewModel>(src);
            dest.Guarantor = Mapper.Map<GuarantorViewModel>(src);
            dest.DateAppointed = src.NomineeJoinedOn;

        }



        void AssignPartners(RegistrationExcelViewModel input, List<PartnerViewModel> partners)
        {
            for (var i = 1; i <= GlobalConstants.MaxUploadMemberCount; i++)
            {
                var designation = Convert.ToString(properties[String.Format("Member{0}DesignationCode", i)].GetValue(input, null));
                if (designation != LookupConstants.Designation.Partner) continue;
                var partner = new PartnerViewModel();
                if (AssignMemberValue(input, partner, i)) partners.Add(partner);
            }
        }

        void AssignDirectors(RegistrationExcelViewModel input, List<DirectorViewModel> directors)
        {
            for (var i = 1; i <= GlobalConstants.MaxUploadMemberCount; i++)
            {
                var designation = Convert.ToString(properties[String.Format("Member{0}DesignationCode", i)].GetValue(input, null));
                if (designation != LookupConstants.Designation.Director) continue;
                var director = new DirectorViewModel();
                if (AssignMemberValue(input, director, i)) directors.Add(director);
            }
        }

        void AssignShareholders(RegistrationExcelViewModel input, List<ShareholderViewModel> shareholders)
        {
            for (var i = 1; i <= GlobalConstants.MaxUploadMemberCount; i++)
            {
                var designation = Convert.ToString(properties[String.Format("Member{0}DesignationCode", i)].GetValue(input, null));
                if (designation != LookupConstants.Designation.Shareholder) continue;
                var shareholder = new ShareholderViewModel();
                if (AssignMemberValue(input, shareholder, i))
                {
                    shareholder.ShareAmount = Convert.ToDecimal(properties[String.Format("Member{0}ShareAmount", i)].GetValue(input, null));
                    shareholder.SharePercentage = Convert.ToDecimal(properties[String.Format("Member{0}SharePercentage", i)].GetValue(input, null));
                    shareholders.Add(shareholder);
                }
            }
        }

        bool AssignMemberValue(RegistrationExcelViewModel input, IMemberViewModel item, int index)
        {
            var name = properties[String.Format("Member{0}Name", index)].GetValue(input, null);
            if (String.IsNullOrWhiteSpace(Convert.ToString(name))) return false;
            item.Name = name.ToString();
            item.ICType = GetLookup<LookupICType>(Convert.ToString(properties[String.Format("Member{0}ICType", index)].GetValue(input, null)));
            item.ICNumber = Convert.ToString(properties[String.Format("Member{0}ICNumber", index)].GetValue(input, null));
            return true;
        }


        LookupItem GetLookup<T>(string code) where T : EntityObject, ILookupEntity
        {
            return Mapper.Map<LookupItem>(lookupRepository.Get<T>(code));
        }

        long GetCompanyID(string companyCode)
        {
            return companyRepository.Get(companyCode).ID;
        }

        public LookupItem GetYNLookup(string value)
        {
            var list = new List<LookupItem>{new  LookupItem { ID = 1, Code = "Y", Description = "Y" },
               new LookupItem { ID = 2, Code = "N", Description = "N" } };

            return list.FirstOrDefault(x => x.Code == value?.ToUpper());

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


        public DateTime? ParseDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Extract only the part that looks like a date (dd/MM/yyyy or MM/dd/yyyy)
            var match = Regex.Match(input, @"\b(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})\b");
            if (!match.Success)
                return null;

            string datePart = match.Value;

            // Possible formats
            string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };

            DateTime parsedDate;
            if (DateTime.TryParseExact(datePart, formats,
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }

            return null;
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
    }// class
}// namespace
