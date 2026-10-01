using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Operations {

    public class ALCMapper : IMapper {

        public void Map() {
            Mapper.CreateMap<ALCHeader, ALCHeaderViewModel>();
            Mapper.CreateMap<ALCMember, ALCMemberViewModel>();

            Mapper.CreateMap<ALCHeaderViewModel, ALCHeader>()
                .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => GetLookupID(src.Company)))
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                 .AfterMap((src, dest) => {
                     var member = Mapper.Map<ALCMember>(src.AgencyManager);
                     member.Designation = "Manager";
                     dest.ALCMembers.Add(member);
                     foreach (ALCMemberViewModel director in src.Directors) {
                         director.Designation = "Director";
                         dest.ALCMembers.Add(Mapper.Map<ALCMember>(director));
                     }
                     foreach (ALCMemberViewModel sh in src.Shareholders) {
                         sh.Designation = "Shareholder";
                         dest.ALCMembers.Add(Mapper.Map<ALCMember>(sh));
                     }
                     dest.Address = Mapper.Map<Address>(src.Address);
                 });
            Mapper.CreateMap<ALCMemberViewModel, ALCMember>();
            Mapper.CreateMap<ALCAddressViewModel, Address>();
                  
        }

        long? GetLookupID(LookupItem item) {
            if (item == null) return null;
            return item.ID;
        }

    }// class

}// namespace
