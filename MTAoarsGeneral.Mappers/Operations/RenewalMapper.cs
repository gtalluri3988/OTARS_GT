using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using System.Data;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Administrative;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Mappers.Operations {
    
    public class RenewalMapper : IMapper {

        IAgencyRepository agencyRepository;
        public RenewalMapper(IAgencyRepository agencyRepository)
        {
            this.agencyRepository = agencyRepository;
        }

        public void Map() {
            MapRenewalList();
            MapRenewalHeader();

            Mapper.CreateMap<RenewalHeader, RenewalHearderViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.LookupRenewalHeaderStatus.Code));

            
        }

        void MapRenewalList() {
            Mapper.CreateMap<RenewalHeader, RenewalListViewModel>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => src.LookupRenewalHeaderStatus.Description))
                
                ;
        }

        void MapRenewalHeader() {

            Mapper.CreateMap<RenewalHeader, RenewalHeaderViewModel>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => src.LookupRenewalHeaderStatus.Description));

            Mapper.CreateMap<RenewalDetail, RenewalDetailViewModel>()
                .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Agency.Name))
                .ForMember(dest => dest.AgencyNumber, opt => opt.MapFrom(src => src.Agency.AgencyPrincipals.Single(ap=>ap.CompanyID == src.RenewalHeader.CompanyID && ap.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).AgencyNumber))
                .ForMember(dest => dest.AgencyPrincipalID, opt => opt.MapFrom(src => src.Agency.AgencyPrincipals.Single(ap => ap.CompanyID == src.RenewalHeader.CompanyID && ap.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).ID))
                .ForMember(dest => dest.IsBancaStaff , opt => opt.MapFrom(src => src.Agency.AgencyMembers
                                                                             .First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee)
                                                                             .Member.IsBancaStaff
                                                                         )
                                                          )
                .ForMember(dest => dest.AgencyID, opt => opt.MapFrom(src => src.AgencyID))
                .ForMember(dest => dest.IntermediaryTypeDescription, opt => opt.MapFrom(src => src.LookupIntermediaryType.Description))
                .ForMember(dest => dest.AgencyTypeDescription, opt => opt.MapFrom(src => src.Agency.AgencyPrincipals.Single(ap => ap.CompanyID == src.RenewalHeader.CompanyID && ap.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).LookupAgencyType.Description))
                .ForMember(dest => dest.RenewalStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupRenewalDetailStatus)))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => src.LookupRenewalDetailStatus.Description))
              ;

            Mapper.CreateMap<RenewalViewDetail, RenewalDetailViewModel>()
                .ForMember(dest => dest.RenewalStatus, opt => opt.MapFrom(src => new LookupItem { ID = src.StatusID, Description = src.StatusDescription}))
                .ForMember(dest => dest.Address, opt => opt.ResolveUsing<AddressResolver>())
                .AfterMap((src, dest) =>
                {
                    var agencyPrincipal = agencyRepository.GetAgencyPrincipal(src.AgencyNumber);           
                    if (agencyPrincipal.LookupAgencyType.Code == LookupConstants.AgencyType.Individual)
                    {
                        var agencyMember = agencyPrincipal.Agency.AgencyMembers.FirstOrDefault(i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
                        if (agencyMember != null)
                            dest.ICNoOrBusinessRegistrationNo = agencyMember.Member.NewICNumber ?? agencyMember.Member.OldICNumber;
                    }
                    else
                        dest.ICNoOrBusinessRegistrationNo = agencyPrincipal.Agency.BusinessRegistrationNumber;
                });
            

            Mapper.CreateMap<RenewalDetailViewModel, RenewalDetail>()
                .ForMember(dest => dest.EntityState, opt => opt.UseValue(EntityState.Modified));
        }


    }// class

}// namespace

