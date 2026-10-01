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
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;



namespace MTAoarsGeneral.Mappers.Operations {

    class GuarantorDisplayResolver : ValueResolver<Agency, GuarantorDisplayViewModel> {
        IMemberRepository memberRepository;
        IScopeDataProvider dataProvider;

        public GuarantorDisplayResolver(IMemberRepository memberRepository, IScopeDataProvider dataProvider) {
            this.memberRepository = memberRepository;
            this.dataProvider = dataProvider;
        }

        protected override GuarantorDisplayViewModel ResolveCore(Agency source) {
            var companyId = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity).CompanyID;
            var ap = source.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == companyId && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General);
            if(ap == null) return null;
            var guarantor = ap.AgencyPrincipalGuarantors.FirstOrDefault();
            if(guarantor == null) return null;
            return Mapper.Map<GuarantorDisplayViewModel>(guarantor);
        }

    }

}
