using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Extensions;
using AutoMapper;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Shared;



namespace MTAoarsGeneral.Mappers.Operations {

    class DirectorDisplayResolver : ValueResolver<Agency, List<MemberDisplayViewModel>> {
        IMemberRepository memberRepository;

        public DirectorDisplayResolver(IMemberRepository memberRepository) {
            this.memberRepository = memberRepository;
        }

        protected override List<MemberDisplayViewModel> ResolveCore(Agency source) {
            var result = new List<MemberDisplayViewModel>();
            var members = source.AgencyMembers.Where(desig => desig.LookupDesignation.Code == LookupConstants.Designation.Director)
                .Select(p=>p.Member);
            foreach (var member in members) {
                result.Add(Mapper.Map<MemberDisplayViewModel>(member));
            }
            return result;
        }

    }

}
