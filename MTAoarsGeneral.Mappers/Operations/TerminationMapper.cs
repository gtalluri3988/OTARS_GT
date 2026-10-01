using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Repositories.Interfaces;


namespace MTAoarsGeneral.Mappers.Operations {
    public class TerminationMapper : IMapper {

        public void Map() {
            Mapper.CreateMap<AgencyPrincipal, AgencyPrincipalHistory>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.AgencyReference, opt => opt.Ignore())
                .ForMember(dest => dest.Agency, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupIntermediaryTypeReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupAgencyTypeReference, opt => opt.Ignore())
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.UseValue(true))
                .ForMember(dest => dest.TerminationDate, opt => opt.UseValue(DateTime.Now))
                .ForMember(dest => dest.MemberReference, opt => opt.Ignore())
                .AfterMap<AgencyPrincipalGuarantorHistoryMappingAction>()
                .AfterMap<AgencyPrincipalStatusHistoryMappingAction>();

            Mapper.CreateMap<AgencyPrincipalStatus, AgencyPrincipalStatusHistory>()
                .ForMember(dest => dest.LookupAgencyPrincipalStatuReference, opt => opt.Ignore())
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest=>dest.ID, opt => opt.Ignore());
            Mapper.CreateMap<AgencyPrincipalGuarantor, AgencyPrincipalGuarantorHistory>()
                .ForMember(dest => dest.LookupGuarantorTypeReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupGuarantorType, opt => opt.Ignore())
                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.ID, opt => opt.Ignore());

            Mapper.CreateMap<SearchAgencyResult, TerminationSearchResponseViewModel>()
                .ForMember(dest => dest.ValidFromLabel, opt => opt.MapFrom(src => src.ValidFrom.Value.ToString(GlobalConstants.DateFormat)))
                .ForMember(dest => dest.ValidToLabel, opt => opt.MapFrom(src => src.ValidTo.Value.ToString(GlobalConstants.DateFormat)))
                //.ForMember(dest => dest.ScheduledOn, opt => opt.UseValue(DateTime.Now))
                .AfterMap((src, dest) => {
                    var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
                    var action = lookupRepository.Get<LookupTerminationAction>(LookupConstants.TerminationAction.Terminate);
                    dest.ActionID = action.ID;
                });

            Mapper.CreateMap<TerminationSearchResponseViewModel, TerminationSchedule>()
                .ForMember(dest => dest.StatusID, opt => opt.ResolveUsing<TerminationScheduleStatusResolver>());
        }

    }// class
}// namespace
