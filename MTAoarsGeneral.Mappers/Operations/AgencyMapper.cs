using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Maintenance;

namespace MTAoarsGeneral.Mappers.Operations {

    public class AgencyMapper : IMapper {

        public void Map() {
            //Mapper.CreateMap<SearchAgencyResult, AgencySearchResponseViewModel>();
            Mapper.CreateMap<SearchAgencyResult, AgencySearchResponseViewModel>()
                .ForMember(dest => dest.ReferredCreatedDateToText, opt => opt.MapFrom(src => src.ReferredCreatedDate.HasValue ? Convert.ToDateTime(src.ReferredCreatedDate).ToString("dd/MM/yyyy") : ""));

            Mapper.CreateMap<SearchAgencyArchiveResult, AgencySearchResponseViewModel>(); /* [License Split] -Added */ 
            Mapper.CreateMap<MiiResult,TBEEnquiryResponseViewModel>();
            Mapper.CreateMap<IbfimResult,TBEEnquiryResponseViewModel>();
            Mapper.CreateMap<Agency, PartnershipRegistrationViewModel>();
            Mapper.CreateMap<SearchAgencyResult, AdminActivityDetailViewModel>()
                .ForMember(dest => dest.NomineeICNumber, opt => opt.MapFrom(src => src.NomineeNewICNumber));

            Mapper.CreateMap<AsciiReportDetail, AdminActivityDetailViewModel>()
                .ForMember(dest => dest.NomineeName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<SearchAgencyResult, CBCDetailViewModel>()
                .ForMember(dest => dest.NomineeICNumber, opt => opt.MapFrom(src => src.NomineeNewICNumber));

            Mapper.CreateMap<PhotoHistory, PhotoUploadEnquiryResponseViewModel>()
                .ForMember(dest => dest.AgencyNumber, opt => opt.MapFrom(src => src.AgencyNumber))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedDate));
        }       

    }// class

}// namespace
