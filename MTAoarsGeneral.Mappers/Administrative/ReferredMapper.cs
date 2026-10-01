using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Mappers.Administrative
{
    public class ReferredMapper : IMapper
    {
        public void Map()
        {
            Mapper.CreateMap<ReferredMember, ReferredResponseViewModel>()
                .ForMember(ViewModel => ViewModel.ICNumber,M => M.MapFrom(DM => DM.NewICNumber) );

            Mapper.CreateMap<ReferredDetail, ReferredResponseViewModel>();

            //Mapper.CreateMap<LookupReferredReason, ReferredReasonViewModel>();

            //Mapper.CreateMap<LookupReferredCategory, ReferredCategoryViewModel>();
                
        }
    }
}
