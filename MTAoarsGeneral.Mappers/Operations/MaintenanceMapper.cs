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

namespace MTAoarsGeneral.Mappers.Operations {
    public class MaintenanceMapper : IMapper {

        public void Map() {

            Mapper.CreateMap<SearchAgencyResult, MaintenanceSearchResponseViewModel>()
                .ForMember(dest => dest.ValidFromLabel, opt => opt.MapFrom(src => src.ValidFrom.Value.ToString(GlobalConstants.DateFormat)))
                .ForMember(dest => dest.ValidToLabel, opt => opt.MapFrom(src => src.ValidTo.Value.ToString(GlobalConstants.DateFormat)))
                .ForMember(dest => dest.ScheduledOn, opt => opt.UseValue(DateTime.Now))
                .AfterMap((src, dest) => {
                    var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
                    var action = lookupRepository.Get<LookupTerminationAction>(LookupConstants.TerminationAction.Terminate);
                    dest.ActionID = action.ID;
                });
        }


    }// class
}// namespace
