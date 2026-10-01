using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using AutoMapper;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Operations
{
    class AdditionalCorporateNomineeResolver : ValueResolver<Agency, List<AdditionalCorporateNomineeViewModel>>
    {
        IMemberRepository memberRepository;

        public AdditionalCorporateNomineeResolver(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        protected override List<AdditionalCorporateNomineeViewModel> ResolveCore(Agency source)
        {
            var result = new List<AdditionalCorporateNomineeViewModel>();
            var members = source.AgencyMembers.Where(desig => desig.LookupDesignation.Code == LookupConstants.Designation.AdditionalCorporateNominee)
                .Select(p => p.Member);
            foreach (var member in members)
            {
                var acn = Mapper.Map<AdditionalCorporateNomineeViewModel>(member);
                //acn.AgencyType = Mapper.Map<LookupItem>(source.LookupAgencyType);
                result.Add(acn);
                
            }
            return result;
        }

    }
}
