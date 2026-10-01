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

    class SpouseDisplayResolver : ValueResolver<Agency, MemberDisplayViewModel> {
        IMemberRepository memberRepository;

        public SpouseDisplayResolver(IMemberRepository memberRepository) {
            this.memberRepository = memberRepository;
        }

        protected override MemberDisplayViewModel ResolveCore(Agency source) {
            var member = memberRepository.GetCorporateNominee(source.ID);
            if (member.Spouses.Count == 0) return null;
            return Mapper.Map<MemberDisplayViewModel>(member.Spouses.First());
        }

    }

}
