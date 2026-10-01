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
using MTAoarsGeneral.Utilities.Mvc;
using System.Globalization;
using System.Data.Objects.DataClasses;

namespace MTAoarsGeneral.Mappers.Operations
{
    public class AdministrativeMapper : IMapper
    {
        ILookupRepository lookupRepository;
        public AdministrativeMapper(ILookupRepository lookupRepository)
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
            Mapper.CreateMap<Agency, AgencyViewModel>()
                 .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => GetM2Exams(src.M2Exam)))
                 .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.DateOfExam))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => new LookupItem { ID = Convert.ToInt16(src.JoinYear), Code = src.JoinYear }))
                  .AfterMap((src, dest) =>
                  {
                      dest.MTAAwards = GetLookupList<LookupMTAAward>(src.MTAAwards);
                      dest.JoinMonth = GetAllMonth(src.JoinMonth);

                  });

            Mapper.CreateMap<AgencyViewModel, Agency>()
                 .ForMember(dest => dest.M2Exam, opt => opt.MapFrom(src => src.M2Exam.Code))
                 .ForMember(dest => dest.JoinMonth, opt => opt.MapFrom(src => src.JoinMonth.Code))
                 .ForMember(dest => dest.JoinYear, opt => opt.MapFrom(src => src.JoinYear.Code))
                 .ForMember(dest => dest.MTAAwards, opt => opt.MapFrom(src => string.Join(";", src.MTAAwards.Select(x => x.Code))))

                 .ForMember(dest => dest.DateOfExam, opt => opt.MapFrom(src => src.DateOfExam));



            Mapper.CreateMap<SearchAgencyResult, AdministrativeSearchAgentResponseViewModel>()
                .ForMember(dest => dest.ValidFromLabel, opt => opt.MapFrom(src => src.ValidFrom.Value.ToString(GlobalConstants.DateFormat)))
                .ForMember(dest => dest.ValidToLabel, opt => opt.MapFrom(src => src.ValidTo.Value.ToString(GlobalConstants.DateFormat)))
                .ForMember(dest => dest.ScheduledOn, opt => opt.UseValue(DateTime.Now))
                .AfterMap((src, dest) =>
                {
                    var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
                    var action = lookupRepository.Get<LookupTerminationAction>(LookupConstants.TerminationAction.Terminate);
                    dest.ActionID = action.ID;
                });

            Mapper.CreateMap<Agency, AdministrativeAgentCorporateViewModel>()
                .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
                .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>())
                .ForMember(dest => dest.Directors, opt => opt.ResolveUsing<DirectorResolver>())
                .ForMember(dest => dest.AdditionalCorporateNominees, opt => opt.ResolveUsing<AdditionalCorporateNomineeResolver>())
                .ForMember(dest => dest.Shareholders, opt => opt.ResolveUsing<ShareholderResolver>()).AfterMap((src, dest) =>
                {
                    dest.Agency = Mapper.Map<AgencyViewModel>(src);
                    //var banker = src.AgencyBankers.FirstOrDefault();
                    //if (banker != null)
                    //{
                    //    dest.AgencyBanker = Mapper.Map<AgencyBankerViewModel>(banker);
                    //}
                });

            Mapper.CreateMap<Agency, AdministrativeAgentIndividualViewModel>()
                .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
                .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>()).AfterMap((src, dest) =>
                {
                    dest.Agency = Mapper.Map<AgencyViewModel>(src);
                });

            Mapper.CreateMap<Agency, AdministrativeAgentPartnershipViewModel>()
                .ForMember(dest => dest.CorporateNominee, opt => opt.ResolveUsing<CorporateNomineeResolver>())
                .ForMember(dest => dest.AgencyType, opt => opt.ResolveUsing<AgencyTypeResolver>())
                .ForMember(dest => dest.Guarantor, opt => opt.ResolveUsing<GuarantorResolver>())
                .AfterMap((src, dest) =>
                {
                    var members = src.AgencyMembers.Where(desig => desig.LookupDesignation.Code == LookupConstants.Designation.Partner).Select(p => p.Member);
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

            Mapper.CreateMap<Agency, AdministrativeAgentSoleProprietorshipViewModel>()
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

        public LookupItem GetYN(string code)
        {
            var list = new List<LookupItem>{ new LookupItem { ID = 1, Code = "Y", Description = "Y" },
            new LookupItem { ID = 2, Code = "N", Description = "N" } };

            return list.FirstOrDefault(x => x.Code == code);

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


    }// class
}
