using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Operations {

    public class CBCMapper : IMapper {

        public void Map() {
            Mapper.CreateMap<CBCHeader, CBCHeaderViewModel>()
                 .ForMember(dest => dest.Company, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.Company)))
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupCBCHeaderStatu)));

            Mapper.CreateMap<CBCDetail, CBCDetailViewModel>()
                .ForMember(dest => dest.IntermediaryTypeDescription, opt => opt.MapFrom(src => src.LookupIntermediaryType.Description))
                .AfterMap((src, dest) => {
                    var cn = src.Agency.AgencyMembers
                        .FirstOrDefault(am => am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
                    if (cn != null) {
                        var m = cn.Member;
                        dest.NomineeICNumber = m.LookupICType.Code == LookupConstants.ICTypes.NewIc? m.NewICNumber : m.PassportNumber;
                        dest.NomineeName = m.Name;
                    }
                    var ap = src.Agency.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == src.CBCHeader.CompanyID
                        && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General);
                    if (ap != null) {
                        dest.AgencyNumber = ap.AgencyNumber;
                    }
                });
        }       

    }// class

}// namespace
