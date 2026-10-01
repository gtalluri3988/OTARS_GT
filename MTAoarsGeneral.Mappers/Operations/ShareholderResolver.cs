using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Mappers.Operations
{
    class ShareholderResolver : ValueResolver<Agency, List<ShareholderViewModel>>
    {
        IMemberRepository memberRepository;
        IScopeDataProvider dataProvider;

        public ShareholderResolver(IMemberRepository memberRepository, IScopeDataProvider dataProvider)
        {
            this.memberRepository = memberRepository;
            this.dataProvider = dataProvider;
        }

        protected override List<ShareholderViewModel> ResolveCore(Agency source)
        {
            var result = new List<ShareholderViewModel>();
            var members = source.AgencyMembers.Where(desig => desig.LookupDesignation.Code == LookupConstants.Designation.Shareholder);
            var companyId = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity).CompanyID;
            var ap = source.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == companyId && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General);
            foreach (var member in members)
            {
                var shareHolder = Mapper.Map<ShareholderViewModel>(member.Member);
                    shareHolder.ShareAmount = member.AmountShareholding.HasValue?member.AmountShareholding.Value:default(decimal);
                    shareHolder.SharePercentage = member.ShareholdingPercentage.HasValue ? (decimal)member.ShareholdingPercentage.Value : default(decimal);
                    if (ap != null) {
                        shareHolder.AgencyType = Mapper.Map<LookupItem>(ap.LookupAgencyType);
                    }
                    result.Add(shareHolder);
            }
            return result;
        }

    }
}