using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Operations
{
    class GuarantorResolver : ValueResolver<Agency, GuarantorViewModel>
    {
        IMemberRepository memberRepository;
        IScopeDataProvider dataProvider;
        public GuarantorResolver(IMemberRepository memberRepository, IScopeDataProvider dataProvider)
        {
            this.memberRepository = memberRepository;
            this.dataProvider = dataProvider;
        }
        protected override GuarantorViewModel ResolveCore(Agency source)
        {
            GuarantorViewModel guarantorModel = new GuarantorViewModel();
            var companyId = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity).CompanyID;
            var ap = source.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == companyId && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General);
            if (ap == null) return null;
            var guarantor = ap.AgencyPrincipalGuarantors.FirstOrDefault();
            if (guarantor == null) return null;
            guarantorModel.GuarantorType =  Mapper.Map<LookupItem>(guarantor.LookupGuarantorType);
            Mapper.Map(guarantor, guarantorModel);
            return guarantorModel;
        }
    }
}
