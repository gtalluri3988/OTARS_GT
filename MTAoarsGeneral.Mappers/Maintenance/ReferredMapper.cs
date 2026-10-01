using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Maintenance
{

    public class ReferredMapper : IMapper
    {

        public void Map()
        {

            Mapper.CreateMap<ReferredDetailViewModel, ReferredDetail>()
                .ForMember(dest => dest.ReasonID, opt => opt.MapFrom(src => src.Reason.ID))
                 .ForMember(dest => dest.ActionId, opt => opt.MapFrom(src => src.ActionTaken.ID))
                .ForMember(dest => dest.PoliceReportLodgedId, opt => opt.MapFrom(src => src.PoliceReportLogged.ID))

                .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.Category.ID));


            Mapper.CreateMap<ReferredAttachmentViewModel, ReferredAttachment>();

            Mapper.CreateMap<ReferredDetail, ReferredDetailViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredDetailStatu)))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredReason)))
                  .ForMember(dest => dest.ActionTaken, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredActionTaken)))
                 .ForMember(dest => dest.PoliceReportLogged, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredPoliceReportLodged)))

                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredCategory)));

            Mapper.CreateMap<ReferredAttachment, ReferredAttachmentViewModel>();

            Mapper.CreateMap<ReferredHeader, ReferredHeaderViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupReferredHeaderStatus)));

            Mapper.CreateMap<ReferredDetail, ReferredMember>()
                .ForMember(dest => dest.LookupReferredReason, opt => opt.Ignore())
                .ForMember(dest => dest.LookupReferredReasonReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupReferredCategory, opt => opt.Ignore())
                .ForMember(dest => dest.LookupReferredCategoryReference, opt => opt.Ignore())
                  .ForMember(dest => dest.LookupReferredActionTaken, opt => opt.Ignore())
                .ForMember(dest => dest.LookupReferredActionTakenReference, opt => opt.Ignore())
                .ForMember(dest => dest.LookupReferredPoliceReportLodged, opt => opt.Ignore())
                .ForMember(dest => dest.LookupReferredPoliceReportLodgedReference, opt => opt.Ignore())

                .ForMember(dest => dest.EntityKey, opt => opt.Ignore())
                .ForMember(dest => dest.EntityState, opt => opt.Ignore())
                .ForMember(dest => dest.RecordVersion, opt => opt.Ignore())
                .ForMember(dest => dest.NewICNumber, opt => opt.MapFrom(src => src.ICNumber))
                .ForMember(dest => dest.IsApproved, opt => opt.UseValue(true));
        }

    }// class

}// namespace
