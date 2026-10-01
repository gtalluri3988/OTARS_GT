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



namespace MTAoarsGeneral.Mappers.Operations
{

    class ShareholderDisplayResolver : ValueResolver<Agency, List<MemberDisplayViewModel>>
    {
        IMemberRepository memberRepository;

        public ShareholderDisplayResolver(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        protected override List<MemberDisplayViewModel> ResolveCore(Agency source)
        {
            var result = new List<MemberDisplayViewModel>();
            var agencyMembers = source.AgencyMembers.Where(desig => desig.LookupDesignation.Code == LookupConstants.Designation.Shareholder);

            foreach (var am in agencyMembers)
            {
                var member = Mapper.Map<MemberDisplayViewModel>(am.Member);
                member.ShareAmount = am.AmountShareholding;
                member.SharePercentage = Convert.ToDecimal(am.ShareholdingPercentage);
                member.NewBusinessRegistrationNumber = am.NewBusinessRegistrationNumber;
                result.Add(member);
            }
            return result;
        }

    }

}
