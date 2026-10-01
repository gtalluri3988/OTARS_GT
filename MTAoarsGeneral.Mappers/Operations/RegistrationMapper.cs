using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Mappers.Shared;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.ViewModels.Notifications;
using System.Globalization;

namespace MTAoarsGeneral.Mappers.Operations
{
    public class RegistrationMapper : IMapper
    {
        ILookupRepository lookupRepository;
        public RegistrationMapper(ILookupRepository lookupRepository)
        {
            this.lookupRepository = lookupRepository;
        }
        LookupItem GetLookup<T>(string code) where T : EntityObject, ILookupEntity
        {
            return Mapper.Map<LookupItem>(lookupRepository.Get<T>(code));
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
        public void Map()
        {
            MapGlobal();
            MapIndividual();
            MapSoleProprietorship();
            MapPartnership();
            MapCorporateRegsitration();
            MapBoardMembers();
            MapInclusion();
            MapConflict();
            MapReinstation();
            MapDisplay();
            MapVariableSource();
        }

        void MapGlobal()
        {
            Mapper.CreateMap<CorporateNomineeViewModel, Member>()
                .ForMember(dest => dest.GenderID, opt => opt.MapFrom(src => GetLookupID(src.Gender)))
                .ForMember(dest => dest.LevelID, opt => opt.MapFrom(src => GetLookupID(src.Level)))
                .ForMember(dest => dest.RaceID, opt => opt.MapFrom(src => GetLookupID(src.Race)))
                .ForMember(dest => dest.ReligionID, opt => opt.MapFrom(src => GetLookupID(src.Religion)))
                .ForMember(dest => dest.MaritalStatusID, opt => opt.MapFrom(src => GetLookupID(src.MaritalStatus)))
                .ForMember(dest => dest.TbeCategoryID, opt => opt.MapFrom(src => GetLookupID(src.TbeCategory)))
                .ForMember(dest => dest.ICTypeID, opt => opt.MapFrom(src => GetLookupID(src.ICType)))
                .AfterMap((src, dest) =>
                {
                    if (src.Spouse != null) dest.Spouses.Add(Mapper.Map<Spouse>(src.Spouse));

                    if (src.Experiences != null)
                    {
                        foreach (var experience in src.Experiences)
                        {
                            if (String.IsNullOrEmpty(experience.AgencyCode)) continue;
                            dest.MemberExperiences.Add(Mapper.Map<MemberExperience>(experience));
                        }
                    }

                    dest.MemberEducationalQualifications.Add(Mapper.Map<MemberEducationalQualification>(src.Qualification));

                });

            Mapper.CreateMap<Member, CorporateNomineeViewModel>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGender)))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMemberLevel)))
                .ForMember(dest => dest.Race, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupRace)))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReligion)))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMaritalStatu)))
                .ForMember(dest => dest.TbeCategory, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupTbeCategory)))
                .ForMember(dest => dest.ICType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupICType)))
                .AfterMap((src, dest) =>
                {
                    if (src.MemberEducationalQualifications.Count == 0)
                    {
                        dest.Qualification = new QualificationViewModel();
                    }

                    foreach (var qualification in src.MemberEducationalQualifications)
                    {
                        dest.Qualification = Mapper.Map<QualificationViewModel>(qualification);
                    }
                    if (src.Spouses.Count == 0)
                    {
                        dest.Spouse = new SpouseViewModel();
                    }
                    foreach (var spouse in src.Spouses)
                    {
                        dest.Spouse = Mapper.Map<SpouseViewModel>(spouse);
                    }
                });

            /* Mapper.CreateMap<Member, Member>()
                 .ForMember(dest => dest.ID, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupGenderReference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupMaritalStatuReference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupMemberLevelReference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupRaceReference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupReligion1Reference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupReligionReference, opt => opt.Ignore())
                 .ForMember(dest => dest.AgencyMembers, opt => opt.Ignore())
                 .ForMember(dest => dest.MemberExperiences, opt => opt.Ignore())
                 .ForMember(dest => dest.MemberEducationalQualifications, opt => opt.Ignore())
                 .ForMember(dest => dest.MemberInsuranceQualifications, opt => opt.Ignore())
                 .ForMember(dest => dest.MemberStatus, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupICTypeReference, opt => opt.Ignore())
                 .AfterMap((src, dest) => {
                     foreach (var qualification in src.MemberEducationalQualifications) {
                         dest.MemberEducationalQualifications.Add(qualification);
                     }
                 });*/

            Mapper.CreateMap<Spouse, SpouseViewModel>()
                 .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGender)))
                .ForMember(dest => dest.Race, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupRace)))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReligion)))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMaritalStatu)));



            Mapper.CreateMap<GuarantorViewModel, AgencyPrincipalGuarantor>()
                .ForMember(dest => dest.TypeID, opt => opt.MapFrom(src => GetLookupID(src.GuarantorType)));
            Mapper.CreateMap<AgencyPrincipalGuarantor, GuarantorViewModel>()
                .ForMember(dest => dest.GuarantorType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGuarantorType)));

            Mapper.CreateMap<MemberExperienceViewModel, MemberExperience>();

            Mapper.CreateMap<SpouseViewModel, Spouse>()
                .ForMember(dest => dest.GenderID, opt => opt.MapFrom(src => GetLookupID(src.Gender)))
                .ForMember(dest => dest.RaceID, opt => opt.MapFrom(src => GetLookupID(src.Race)))
                .ForMember(dest => dest.MaritalStatusID, opt => opt.MapFrom(src => GetLookupID(src.MaritalStatus)))
                .ForMember(dest => dest.ReligionID, opt => opt.MapFrom(src => GetLookupID(src.Religion)));

            Mapper.CreateMap<QualificationViewModel, MemberEducationalQualification>()
                .ForMember(dest => dest.EducationalQualificationID, opt => opt.MapFrom(src => GetLookupID(src.EducationalQualification)));
            Mapper.CreateMap<QualificationViewModel, MemberInsuranceQualification>();
            Mapper.CreateMap<MemberEducationalQualification, QualificationViewModel>()
                .ForMember(dest => dest.EducationalQualification, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupEducationalQualification)));
            Mapper.CreateMap<MemberInsuranceQualification, QualificationViewModel>();
            Mapper.CreateMap<RegistrationIndexViewModel, AgencyViewModel>()
                .ForMember(dest => dest.IsFamily, opt => opt.UseValue(false))
                .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.M2Exam))
                .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.DateOfExam))
                .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.JoinMonth))
                .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.JoinYear))
                .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.MTAAwards))
                .ForMember(dest => dest.SocialMediaAddress, opt => opt.MapFrom(src => src.SocialMediaAddress))

                .ForMember(dest => dest.IsGeneral, opt => opt.UseValue(true));
            Mapper.CreateMap<RegistrationIndexViewModel, CorporateNomineeViewModel>()
                //.ForMember(dest => dest.ICTypeID, opt => opt.MapFrom(src => src.ICTypeID))
                .ForMember(dest => dest.IsBancaStaff, opt => opt.MapFrom(src => src.IsBancaStaff))
                .ForMember(dest => dest.BirthDate, opt => opt.ResolveUsing<ConditionalNewICToBirthDateResolver>())
                // .ForMember(dest => dest.GenderID, opt => opt.ResolveUsing<ConditionalNewICToGenderResolver>())
                .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>());

            Mapper.CreateMap<RegistrationIndexViewModel, RegistrationIndexViewModel>();

            Mapper.CreateMap<RegistrationIndexViewModel, RegistrationIndexViewModel>()
                .ForMember(dest => dest.ICNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ICType, opt => opt.Ignore())
                .ForMember(dest => dest.IsFamily, opt => opt.UseValue(false))
                .ForMember(dest => dest.IsGeneral, opt => opt.UseValue(true))
                .ForMember(dest => dest.AgencyType, opt => opt.Ignore());

            Mapper.CreateMap<AgencyBankerViewModel, AgencyBanker>();
            Mapper.CreateMap<AgencyBanker, AgencyBankerViewModel>();
            Mapper.CreateMap<Agency, AgencyViewModel>()
                .ForMember(dest => dest.ShouldDisplayAuthorizedCapital, src => src.ResolveUsing<AuthorizedCapitalResolver>())
                .ForMember(dest => dest.IsFamily, opt => opt.UseValue(false))
                .ForMember(dest => dest.IsGeneral, opt => opt.UseValue(true))
                 .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => GetM2Exams(src.M2Exam)))
                 .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => GetAllMonth(src.JoinMonth)))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear }))
                 .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => GetLookupList<LookupMTAAward>(src.MTAAwards)))

                 .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.DateOfExam));


            Mapper.CreateMap<AgencyViewModel, Agency>()
                 .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.M2Exam.Code))
                 .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.DateOfExam))
                 .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.JoinMonth.Code))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.JoinYear.Code))
                 .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.MTAAwards != null && src.MTAAwards.Any() ? string.Join(";", src.MTAAwards.Select(x => x.Code)) : null))

                .ForMember(dest => dest.IsStockExchangeListed, opt => opt.Ignore());


            Mapper.CreateMap<RegistrationViewModel, Agency>()
               .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.Agency.M2Exam.Code))
               .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.Agency.JoinMonth.Code))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.Agency.JoinYear.Code))
                 .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.Agency.MTAAwards != null && src.Agency.MTAAwards.Any() ? string.Join(";", src.Agency.MTAAwards.Select(x => x.Code)) : null))

                .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.Agency.DateOfExam));

        }

        void MapIndividual()
        {
            Mapper.CreateMap<IndividualRegistrationViewModel, Agency>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Agency.Name) ? src.CorporateNominee.Name : src.Agency.Name))
                .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.Agency.M2Exam.Code))
                .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.Agency.DateOfExam))
                .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.Agency.JoinYear.Code))
                .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.Agency.JoinMonth.Code))
                .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.Agency.MTAAwards != null && src.Agency.MTAAwards.Any() ? string.Join(";", src.Agency.MTAAwards.Select(x => x.Code)) : null))
                .ForMember(dest => dest.SocialMediaAddress, opt => opt.MapFrom(src => src.Agency.SocialMediaAddress))

                //.ForMember(dest => dest.TypeID, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.RegistrationDate, opt => opt.Condition(IsAdd))
                .ForMember(dest => dest.RegistrationDate, opt => opt.UseValue(DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.UseValue(true))
                .AfterMap((src, dest) =>
                {
                    var memberCreator = ObjectContainer.Container.Resolve<AgencyMemberCreator>();
                    var cn = memberCreator.GetCorporateNominee(src);
                    long memberID = 0;
                    if (cn.Member.ID > 0)
                    {
                        memberID = cn.MemberID;
                        int index = dest.AgencyMembers.ToList().FindIndex(m => m.MemberID == cn.MemberID);
                        if (index >= 0)
                            dest.AgencyMembers.ElementAt(index).Member.IsBancaStaff = cn.Member.IsBancaStaff;
                        else if (dest.AgencyMembers.Count > 0)
                            dest.AgencyMembers.FirstOrDefault().Member.IsBancaStaff = cn.Member.IsBancaStaff;
                        if (cn.Agency != null) dest.Name = cn.Agency.Name;
                    }
                    else
                    {
                        dest.AgencyMembers.Add(cn);
                    }
                    AssignAgencyPrincipal(src, dest, memberID > 0 ? memberID : dest.AgencyMembers.First().MemberID, src.PhotoPath);
                });

            Mapper.CreateMap<Agency, IndividualRegistrationViewModel>()
                .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
                .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>()).AfterMap((src, dest) =>
                {
                    dest.Agency = Mapper.Map<AgencyViewModel>(src);
                });



        }

        void MapSoleProprietorship()
        {
            Mapper.CreateMap<SoleProprietorshipRegistrationViewModel, Agency>()
                  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Agency.Name))
                   .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.Agency.M2Exam.Code))
                .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.Agency.DateOfExam))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.Agency.JoinYear.Code))
                .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.Agency.JoinMonth.Code))

                .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.Agency.MTAAwards != null && src.Agency.MTAAwards.Any() ? string.Join(";", src.Agency.MTAAwards.Select(x => x.Code)) : null))
                .ForMember(dest => dest.SocialMediaAddress, opt => opt.MapFrom(src => src.Agency.SocialMediaAddress))

                  //.ForMember(dest => dest.TypeID, opt => opt.ResolveUsing<AgencyTypeResolver>())
                  .ForMember(dest => dest.BusinessRegistrationNumber, opt => opt.MapFrom(src => src.Agency.BusinessRegistrationNumber))
                  .ForMember(dest => dest.AuthorizedCapital, opt => opt.MapFrom(src => src.Agency.AuthorizedCapital))
                  .ForMember(dest => dest.PaidupCapital, opt => opt.MapFrom(src => src.Agency.PaidupCapital))
                  .ForMember(dest => dest.RegistrationDate, opt => opt.Condition(IsAdd))
                  .ForMember(dest => dest.RegistrationDate, opt => opt.UseValue(DateTime.Now))
                  .ForMember(dest => dest.IsActive, opt => opt.UseValue(true))
                  .AfterMap((src, dest) =>
                  {
                      var memberCreator = ObjectContainer.Container.Resolve<AgencyMemberCreator>();
                      var cn = memberCreator.GetCorporateNominee(src);
                      long memberID = 0;
                      if (cn.Member.ID > 0)
                      {
                          memberID = cn.MemberID;
                          int index = dest.AgencyMembers.ToList().FindIndex(m => m.MemberID == cn.MemberID);
                          if (index >= 0)
                              dest.AgencyMembers.ElementAt(index).Member.IsBancaStaff = cn.Member.IsBancaStaff;
                          else if (dest.AgencyMembers.Count > 0)
                              dest.AgencyMembers.FirstOrDefault().Member.IsBancaStaff = cn.Member.IsBancaStaff;
                          if (cn.Agency != null) dest.Name = cn.Agency.Name;
                      }
                      else
                      {
                          dest.AgencyMembers.Add(cn);
                      }
                      AssignAgencyPrincipal(src, dest, memberID > 0 ? memberID : dest.AgencyMembers.First().MemberID, src.PhotoPath);
                      AssignAgencyBankers(src, dest);

                  });

            Mapper.CreateMap<Agency, SoleProprietorshipRegistrationViewModel>()
             .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
             .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>())
               .AfterMap((src, dest) =>
               {
                   dest.Agency = Mapper.Map<AgencyViewModel>(src);
                   var banker = src.AgencyBankers.FirstOrDefault();
                   if (banker != null)
                   {
                       dest.AgencyBanker = Mapper.Map<AgencyBankerViewModel>(banker);
                   }
               });

        }

        void MapPartnership()
        {
            Mapper.CreateMap<PartnershipRegistrationViewModel, Agency>()
                  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Agency.Name))
                   .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.Agency.M2Exam.Code))
                .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.Agency.DateOfExam))
               .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.Agency.JoinYear.Code))
                .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.Agency.JoinMonth.Code))
                .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.Agency.MTAAwards != null && src.Agency.MTAAwards.Any() ? string.Join(";", src.Agency.MTAAwards.Select(x => x.Code)) : null))
                .ForMember(dest => dest.SocialMediaAddress, opt => opt.MapFrom(src => src.Agency.SocialMediaAddress))

                  // .ForMember(dest => dest.TypeID, opt => opt.ResolveUsing<AgencyTypeResolver>())
                  .ForMember(dest => dest.BusinessRegistrationNumber, opt => opt.MapFrom(src => src.Agency.BusinessRegistrationNumber))
                  .ForMember(dest => dest.AuthorizedCapital, opt => opt.MapFrom(src => src.Agency.AuthorizedCapital))
                  .ForMember(dest => dest.PaidupCapital, opt => opt.MapFrom(src => src.Agency.PaidupCapital))
                  .ForMember(dest => dest.RegistrationDate, opt => opt.Condition(IsAdd))
                  .ForMember(dest => dest.IsActive, opt => opt.UseValue(true))
                  .ForMember(dest => dest.RegistrationDate, opt => opt.UseValue(DateTime.Now))
                    .AfterMap((src, dest) =>
                    {
                        var memberCreator = ObjectContainer.Container.Resolve<AgencyMemberCreator>();
                        var cn = memberCreator.GetCorporateNominee(src);
                        if (cn.Member.ID > 0)
                        {
                            var existCn = dest.AgencyMembers.First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee).Member;
                            dest.AgencyMembers.Add(memberCreator.GetCorporateNomineeAsPartner(existCn));
                            int index = dest.AgencyMembers.ToList().FindIndex(m => m.MemberID == cn.MemberID);
                            if (index >= 0)
                                dest.AgencyMembers.ElementAt(index).Member.IsBancaStaff = cn.Member.IsBancaStaff;
                            else if (dest.AgencyMembers.Count > 0)
                                dest.AgencyMembers.FirstOrDefault().Member.IsBancaStaff = cn.Member.IsBancaStaff;
                        }
                        else
                        {
                            dest.AgencyMembers.Add(cn);
                            dest.AgencyMembers.Add(memberCreator.GetCorporateNomineeAsPartner(cn.Member));
                        }
                        foreach (var partner in memberCreator.GetPartners(src))
                        {
                            dest.AgencyMembers.Add(partner);
                        }
                        AssignAgencyPrincipal(src, dest, null, src.PhotoPath);
                        AssignAgencyBankers(src, dest);

                    });

            Mapper.CreateMap<PartnerViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))

                .ForMember(dest => dest.OldICNumber, opt => opt.MapFrom(src => src.OldICNumber))
                .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>())
                .ForMember(dest => dest.ICTypeID, opt => opt.MapFrom(src => src.ICType.ID));

            Mapper.CreateMap<Agency, PartnershipRegistrationViewModel>()
             .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
            .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>())

                .AfterMap((src, dest) =>
                {
                    var members = src.AgencyMembers.Where(desig => desig.LookupDesignation.Code == LookupConstants.Designation.Partner)
                     .Select(p => p.Member);
                    foreach (var member in members)
                    {
                        dest.Partners.Add(Mapper.Map<PartnerViewModel>(member));
                    }
                    dest.Agency = Mapper.Map<AgencyViewModel>(src);
                    // dest.AgencyType = Mapper.Map<LookupItem>(src.LookupAgencyType);                   
                    var banker = src.AgencyBankers.FirstOrDefault();
                    if (banker != null)
                    {
                        dest.AgencyBanker = Mapper.Map<AgencyBankerViewModel>(banker);
                    }
                    dest.Agency = Mapper.Map<AgencyViewModel>(src);
                });
        }

        void MapBoardMembers()
        {
            Mapper.CreateMap<DirectorViewModel, Member>()
                .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>());
            Mapper.CreateMap<ShareholderViewModel, Member>()
                .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>());
            Mapper.CreateMap<AdditionalCorporateNomineeViewModel, Member>()
               .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
               .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>())
               .ForMember(dest => dest.GenderID, opt => opt.MapFrom(src => GetLookupID(src.Gender)))
                .ForMember(dest => dest.RaceID, opt => opt.MapFrom(src => GetLookupID(src.Race)))
                .ForMember(dest => dest.ReligionID, opt => opt.MapFrom(src => GetLookupID(src.Religion)))
                .ForMember(dest => dest.MaritalStatusID, opt => opt.MapFrom(src => GetLookupID(src.MaritalStatus)))
                .ForMember(dest => dest.ICTypeID, opt => opt.MapFrom(src => GetLookupID(src.ICType)));
        }

        void MapCorporateRegsitration()
        {
            Mapper.CreateMap<CorporateRegistrationViewModel, Agency>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Agency.Name))
                  .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.Agency.M2Exam.Code))
                .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.Agency.DateOfExam))
                .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.Agency.JoinYear.Code))
                .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.Agency.JoinMonth.Code))
                .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => src.Agency.MTAAwards != null && src.Agency.MTAAwards.Any() ? string.Join(";", src.Agency.MTAAwards.Select(x => x.Code)) : null))
                .ForMember(dest => dest.SocialMediaAddress, opt => opt.MapFrom(src => src.Agency.SocialMediaAddress))

                 // .ForMember(dest => dest.TypeID, opt => opt.ResolveUsing<AgencyTypeResolver>())
                 .ForMember(dest => dest.BusinessRegistrationNumber, opt => opt.MapFrom(src => src.Agency.BusinessRegistrationNumber))
                 .ForMember(dest => dest.AuthorizedCapital, opt => opt.MapFrom(src => src.Agency.AuthorizedCapital))
                 .ForMember(dest => dest.PaidupCapital, opt => opt.MapFrom(src => src.Agency.PaidupCapital))
                 .ForMember(dest => dest.RegistrationDate, opt => opt.Condition(IsAdd))
                 .ForMember(dest => dest.RegistrationDate, opt => opt.UseValue(DateTime.Now))
                 .ForMember(dest => dest.IsActive, opt => opt.UseValue(true))
                 .AfterMap((src, dest) =>
                 {
                     AssignAgencyBankers(src, dest);
                     AssignBoardMembers(src, dest);
                     AssignAgencyPrincipal(src, dest, null, src.PhotoPath);
                 });


            Mapper.CreateMap<Agency, CorporateRegistrationViewModel>()
             .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
              .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>())
                .ForMember(dest => dest.Directors, opt => opt.ResolveUsing<DirectorResolver>())
                .ForMember(dest => dest.AdditionalCorporateNominees, opt => opt.ResolveUsing<AdditionalCorporateNomineeResolver>())
                .ForMember(dest => dest.Shareholders, opt => opt.ResolveUsing<ShareholderResolver>()).AfterMap((src, dest) =>
                {
                    dest.Agency = Mapper.Map<AgencyViewModel>(src);
                    var banker = src.AgencyBankers.FirstOrDefault();
                    if (banker != null)
                    {
                        dest.AgencyBanker = Mapper.Map<AgencyBankerViewModel>(banker);
                    }
                });
        }


        void MapInclusion()
        {
            Mapper.CreateMap<RegistrationIndexViewModel, RegistrationInclusionViewModel>()
                .ForMember(dest => dest.Guarantor, opt => opt.UseValue(new GuarantorViewModel()));

            //to-do:Need to check agency type
            Mapper.CreateMap<RegistrationInclusionViewModel, List<AgencyPrincipal>>()
                .ConvertUsing(s =>
                {
                    var principalCreator = ObjectContainer.Container.Resolve<AgencyPrincipalCreator>();
                    s.CorporateNominee = new CorporateNomineeViewModel();
                    long? memberID = s.Agency.Nominee.ID;
                    if (s.Agency.Nominee != null) s.CorporateNominee.IsBancaStaff = s.Agency.Nominee.IsBancaStaff;
                    return GetAgencyPrincipal(s, principalCreator.GetPrincipal, s.AgencyType.ID, true, memberID, s.PhotoPath);
                });

        }


        void MapConflict()
        {
            Mapper.CreateMap<RegistrationIndexViewModel, RegistrationConflictViewModel>()
                 .ForMember(dest => dest.Guarantor, opt => opt.UseValue(new GuarantorViewModel()));

            Mapper.CreateMap<RegistrationConflictViewModel, AgencyPrincipalConflict>()
                .ForMember(dest => dest.Agency, opt => opt.Ignore())
                .ForMember(dest => dest.TypeID, opt => opt.MapFrom(src => GetLookupID(src.AgencyType)))
                .ForMember(dest => dest.ConflictAttachments, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.UseValue(true));
            //.AfterMap((src, dest) =>
            //{
            //    var principalCreator = ObjectContainer.Container.Resolve<AgencyPrincipalCreator>();
            //    dest = GetAgencyPrincipalConflict(src, principalCreator.GetPrincipalConflict).First();
            //});


            Mapper.CreateMap<RegistrationConflictViewModel, List<AgencyPrincipalConflict>>()
                .ConvertUsing(s =>
                {
                    var principalCreator = ObjectContainer.Container.Resolve<AgencyPrincipalCreator>();
                    return GetAgencyPrincipalConflict(s, principalCreator.GetPrincipalConflict);
                });

            Mapper.CreateMap<ConflictAttachmentViewModel, ConflictAttachment>();

            Mapper.CreateMap<ConflictAttachment, ConflictAttachmentViewModel>();

            Mapper.CreateMap<ConflictGuarantor, GuarantorViewModel>();

            Mapper.CreateMap<GuarantorViewModel, ConflictGuarantor>()
                .ForMember(dest => dest.TypeID, opt => opt.MapFrom(src => GetLookupID(src.GuarantorType)));

            Mapper.CreateMap<ConflictGuarantor, AgencyPrincipalGuarantor>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.LookupGuarantorType, opt => opt.Ignore())
                .ForMember(dest => dest.LookupGuarantorTypeReference, opt => opt.Ignore())
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.EntityState, opt => opt.Ignore());

        }

        void MapReinstation()
        {
            Mapper.CreateMap<RegistrationIndexViewModel, RegistrationReinstationViewModel>();

            Mapper.CreateMap<AgencyPrincipalHistory, AgencyPrincipal>()
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.EntityState, opt => opt.Ignore())
                .ForMember(dest => dest.AgencyReference, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupAgencyTypeReference, opt => opt.Ignore())
                .ForMember(dest => dest.MemberReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupIntermediaryTypeReference, opt => opt.Ignore());
            Mapper.CreateMap<AgencyPrincipalGuarantorHistory, AgencyPrincipalGuarantor>()
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.EntityState, opt => opt.Ignore())
                .ForMember(dest => dest.AgencyPrincipalReference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupGuarantorTypeReference, opt => opt.Ignore());
            Mapper.CreateMap<AgencyPrincipalStatusHistory, AgencyPrincipalStatus>()
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.EntityState, opt => opt.Ignore())
                .ForMember(dest => dest.AgencyPrincipalReference, opt => opt.Ignore())
                 .ForMember(dest => dest.LookupAgencyPrincipalStatuReference, opt => opt.Ignore());
        }



        void MapDisplay()
        {
            Mapper.CreateMap<Agency, AgencyDisplayViewModel>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                //// .ForMember(dest => dest.ShouldDisplayAuthorizedCapital, opt => opt.MapFrom(src => (
                //     src.LookupAgencyType.Code == LookupConstants.AgencyType.PrivateLimitedCompany
                //     || src.LookupAgencyType.Code == LookupConstants.AgencyType.PublicLimitedCompany
                //     || src.LookupAgencyType.Code == LookupConstants.AgencyType.Cooperative
                //     || src.LookupAgencyType.Code == LookupConstants.AgencyType.GovernmentAgency
                //     )))
                //  .ForMember(dest => dest.ShouldDisplayAuthorizedCapital, opt => opt.MapFrom(src => src.LookupAgencyType.Code == LookupConstants.AgencyType.PrivateLimitedCompany))
                .ForMember(dest => dest.IsFamily, opt => opt.UseValue(false))
                .ForMember(dest => dest.IsGeneral, opt => opt.UseValue(true))
                .ForMember(dest => dest.Nominee, opt => opt.ResolveUsing<CorporateNomineeDisplayResolver>())
                .ForMember(dest => dest.Spouse, opt => opt.ResolveUsing<SpouseDisplayResolver>())
                .ForMember(dest => dest.Partners, opt => opt.ResolveUsing<PartnerDisplayResolver>())
                .ForMember(dest => dest.Directors, opt => opt.ResolveUsing<DirectorDisplayResolver>())
                .ForMember(dest => dest.Shareholders, opt => opt.ResolveUsing<ShareholderDisplayResolver>())
                .ForMember(dest => dest.AdditionalCorporateNominees, opt => opt.ResolveUsing<AdditionalCorporateNomineeDisplayResolver>())
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => Mapper.Map<AddressDisplayViewModel>(src.Address)))
                .ForMember(dest => dest.AgencyBanker, opt => opt.MapFrom(src => Mapper.Map<AgencyBankerDisplayViewModel>(src.AgencyBankers.FirstOrDefault())))
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorDisplayResolver>())
                .ForMember(dest => dest.AgencyPrincipals, opt => opt.ResolveUsing<AgencyPrincipalViewModelResolver>())
                 .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => GetM2Exams(src.M2Exam)))
                 .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => GetAllMonth(src.JoinMonth)))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear }))
                 .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => GetLookupList<LookupMTAAward>(src.MTAAwards)))

                 .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.DateOfExam));
            Mapper.CreateMap<Member, MemberDisplayViewModel>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMemberLevel)))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGender)))
                .ForMember(dest => dest.ICType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupICType)))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMaritalStatu)))
                .ForMember(dest => dest.Race, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupRace)))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReligion)));

            Mapper.CreateMap<Spouse, MemberDisplayViewModel>()
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGender)))
               .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMaritalStatu)))
               .ForMember(dest => dest.Race, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupRace)))
               .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReligion)));

            Mapper.CreateMap<AgencyPrincipal, AgencyPrincipalDisplayViewModel>()
                .ForMember(dest => dest.AgencyNumber, opt => opt.MapFrom(src => src.AgencyNumber))
                .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => src.Company.ID))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Company.Code))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.ItermediaryTypeDescription, opt => opt.MapFrom(src => src.LookupIntermediaryType.Description))
                 .ForMember(dest => dest.AgencyTypeDescription, opt => opt.MapFrom(src => src.LookupAgencyType.Description))
                .ForMember(dest => dest.AgencyType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupAgencyType)))
                .ForMember(dest => dest.IsBancaStaff, opt => opt.MapFrom(src => src.IsBancaStaff))
                .ForMember(dest => dest.Member, opt => opt.MapFrom(src => src.Member != null ? src.Member.Name : ""))
                .AfterMap((src, dest) =>
                {
                    var statuses = new StringBuilder();
                    var remarks = new StringBuilder();

                    src.AgencyPrincipalStatus.Each(aps =>
                    {
                        if (statuses.Length > 0) statuses.Append(", ");
                        if (remarks.Length > 0) remarks.Append(" ,");
                        statuses.Append(aps.LookupAgencyPrincipalStatu.Description);
                        remarks.Append(aps.Remarks);
                    });

                    dest.Statuses = statuses.ToString();
                    //27/12/2018 empty string return after map
                    //dest.Remarks = remarks.ToString();
                });

            Mapper.CreateMap<AgencyBanker, AgencyBankerDisplayViewModel>()
                .ForMember(dest => dest.Bank, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupBank)))
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => Mapper.Map<LookupBankAccountType>(src.LookupBankAccountType)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => Mapper.Map<AddressDisplayViewModel>(src.Address)));

            Mapper.CreateMap<AgencyPrincipalHistory, AgencyPrincipalDisplayViewModel>()
               .ForMember(dest => dest.AgencyNumber, opt => opt.MapFrom(src => src.AgencyNumber))
               .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => src.Company.ID))
               .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Company.Code))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
               .ForMember(dest => dest.ItermediaryTypeDescription, opt => opt.MapFrom(src => src.LookupIntermediaryType.Description))
               //.ForMember(dest => dest.AgencyTypeDescription, opt => opt.MapFrom(src => src.LookupAgencyType.Description))
               .ForMember(dest => dest.AgencyType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupAgencyType)))
               .ForMember(dest => dest.IsBancaStaff, opt => opt.MapFrom(src => src.IsBancaStaff));

            Mapper.CreateMap<AgencyPrincipalDisplayViewModel, LookupItem>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.CompanyID))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CompanyName));

            Mapper.CreateMap<Address, AddressDisplayViewModel>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupState)));

            Mapper.CreateMap<AgencyPrincipalGuarantor, GuarantorDisplayViewModel>()
                 .ForMember(dest => dest.GuarantorType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGuarantorType)))
                 ;

            Mapper.CreateMap<RegistrationViewModel, AgencyDisplayViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AgencyType.Code == LookupConstants.AgencyType.Individual ? src.Agency.Name : src.CorporateNominee.Name))
                .ForMember(dest => dest.AuthorizedCapital, opt => opt.MapFrom(src => src.Agency.AuthorizedCapital))
                .ForMember(dest => dest.IsFamily, opt => opt.UseValue(false))
                .ForMember(dest => dest.IsGeneral, opt => opt.UseValue(true))
                .ForMember(dest => dest.BusinessRegistrationNumber, opt => opt.MapFrom(src => src.Agency.BusinessRegistrationNumber))
                .ForMember(dest => dest.PaidupCapital, opt => opt.MapFrom(src => src.Agency.PaidupCapital))
                .AfterMap((src, dest) =>
                {
                    var ab = src as IExistableAgencyBanker;
                    if (ab != null)
                    {
                        dest.AgencyBanker = Mapper.Map<AgencyBankerDisplayViewModel>(ab.AgencyBanker);
                    }
                });

            Mapper.CreateMap<CorporateNomineeViewModel, MemberDisplayViewModel>();
            Mapper.CreateMap<DirectorViewModel, MemberDisplayViewModel>()
                 .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>());
            Mapper.CreateMap<PartnerViewModel, MemberDisplayViewModel>()
                 .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>());
            Mapper.CreateMap<ShareholderViewModel, MemberDisplayViewModel>()
                 .ForMember(dest => dest.NewICNumber, opt => opt.ResolveUsing<ConditionalNewICResolver>())
                .ForMember(dest => dest.PassportNumber, opt => opt.ResolveUsing<ConditionalPassportNumberResolver>());
            Mapper.CreateMap<SpouseViewModel, MemberDisplayViewModel>();
            Mapper.CreateMap<QualificationViewModel, QualificationDisplayViewModel>();
            Mapper.CreateMap<GuarantorViewModel, GuarantorDisplayViewModel>();
            Mapper.CreateMap<AddressViewModel, AddressDisplayViewModel>();
            Mapper.CreateMap<Member, DirectorViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.ICType = Mapper.Map<LookupItem>(src.LookupICType);
                    dest.ICNumber = (src.LookupICType.Code == LookupConstants.ICTypes.NewIc) ? src.NewICNumber : src.PassportNumber;

                });
            Mapper.CreateMap<AgencyBankerViewModel, AgencyBankerDisplayViewModel>();
            Mapper.CreateMap<Member, ShareholderViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.ICType = Mapper.Map<LookupItem>(src.LookupICType);
                    dest.ICNumber = (src.LookupICType.Code == LookupConstants.ICTypes.NewIc) ? src.NewICNumber : src.PassportNumber;
                });
            Mapper.CreateMap<Member, PartnerViewModel>()
                 .AfterMap((src, dest) =>
                 {
                     dest.ICType = Mapper.Map<LookupItem>(src.LookupICType);
                     dest.ICNumber = (src.LookupICType.Code == LookupConstants.ICTypes.NewIc) ? src.NewICNumber : src.PassportNumber;
                 });

            Mapper.CreateMap<Member, AdditionalCorporateNomineeViewModel>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupGender)))
                .ForMember(dest => dest.Race, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupRace)))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReligion)))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupMaritalStatu)))
                .ForMember(dest => dest.ICType, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupICType)))
                 .AfterMap((src, dest) =>
                 {
                     dest.ICType = Mapper.Map<LookupItem>(src.LookupICType);
                     dest.ICNumber = (src.LookupICType.Code == LookupConstants.ICTypes.NewIc) ? src.NewICNumber : src.PassportNumber;
                 });

            Mapper.CreateMap<ReferredMember, ReferredMemberViewModel>()
                 .ForMember(dest => dest.CategoryDescription, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredCategory).Description))
                 .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => !src.CreatedDate.HasValue ? "" : Convert.ToDateTime(src.CreatedDate).ToString(GlobalConstants.DateFormat)));
        }

        void MapVariableSource()
        {
            Mapper.CreateMap<CorporateNomineeViewModel, ChangeNomineeVariableSource>();
        }

        string GetLookup(ILookupEntity lookupEntity)
        {
            if (lookupEntity == null) return null;
            return lookupEntity.Description;
        }

        bool IsAdd(RegistrationViewModel model)
        {
            return model.CurrentAction == Actions.Add;
        }

        List<TDest> GetAgencyPrincipal<TSource, TDest>(TSource source, Func<TSource, long, long, bool, long?, string, TDest> function, long typeId, bool isInclusion = false, long? memberID = null, string photoPath = null) where TSource : IExistableIntermediary
        {
            var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
            var list = new List<TDest>();
            var generalId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            list.Add(function(source, generalId, typeId, isInclusion, memberID, photoPath));
            return list;
        }
        List<TDest> GetAgencyPrincipalConflict<TSource, TDest>(TSource source, Func<TSource, long, bool, TDest> function, bool isInclusion = false) where TSource : IExistableIntermediary
        {
            var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
            var list = new List<TDest>();
            var generalId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            list.Add(function(source, generalId, isInclusion));
            return list;
        }

        void AssignAgencyPrincipal<TSource>(TSource source, Agency agency, long? memberID = null, string photoPath = null) where TSource : RegistrationViewModel
        {
            var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
            var principalCreator = ObjectContainer.Container.Resolve<AgencyPrincipalCreator>();
            var generalId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            agency.AgencyPrincipals.Add(principalCreator.GetPrincipal(source, generalId, source.AgencyType.ID, false, memberID, photoPath));
        }

        void AssignAgencyBankers<TSource>(TSource source, Agency agency) where TSource : IExistableAgencyBanker
        {
            if (source.AgencyBanker == null) return;
            if (source.AgencyBanker.Bank == null) return;
            var item = Mapper.Map<AgencyBanker>(source.AgencyBanker);
            item.Address = Mapper.Map<Address>(source.AgencyBanker.Address);
            agency.AgencyBankers.Add(item);
        }

        void AssignBoardMembers<TSource>(TSource source, Agency agency) where TSource : RegistrationViewModel, IExistableBoardMembers
        {
            var memberCreator = ObjectContainer.Container.Resolve<AgencyMemberCreator>();
            var cn = memberCreator.GetCorporateNominee(source);
            if (cn.Member.ID > 0)
            {
                int index = agency.AgencyMembers.ToList().FindIndex(m => m.MemberID == cn.MemberID);
                if (index >= 0)
                    agency.AgencyMembers.ElementAt(index).Member.IsBancaStaff = cn.Member.IsBancaStaff;
                else if (agency.AgencyMembers.Count > 0)
                    agency.AgencyMembers.FirstOrDefault().Member.IsBancaStaff = cn.Member.IsBancaStaff;
            }
            else
            {
                agency.AgencyMembers.Add(cn);
            }
            agency.AgencyMembers.AddRange(memberCreator.GetBoardMembers(cn.Member, source));

        }

        long? GetLookupID(LookupItem item)
        {
            if (item == null) return null;
            return item.ID;
        }
        LookupItem GetYN(string code)
        {
            var list = new List<LookupItem>{ new LookupItem { ID = 1, Code = "Y", Description = "Y" },
            new LookupItem { ID = 2, Code = "N", Description = "N" } };

            return list.FirstOrDefault(x => x.Code == code);

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
