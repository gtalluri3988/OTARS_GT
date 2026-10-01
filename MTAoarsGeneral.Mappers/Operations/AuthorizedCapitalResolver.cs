using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Mappers.Operations {
    class AuthorizedCapitalResolver :ValueResolver<Agency, bool>
    {
        
        IScopeDataProvider dataProvider;
        public AuthorizedCapitalResolver(IScopeDataProvider dataProvider)
        {           
            this.dataProvider = dataProvider;
        }
        protected override bool ResolveCore(Agency source)
        {
           
            var companyId = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity).CompanyID;
            var ap = source.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == companyId && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General);
            if (ap == null) return false;
            var code = ap.LookupAgencyType.Code;
            if (code == LookupConstants.AgencyType.PrivateLimitedCompany || code == LookupConstants.AgencyType.PublicLimitedCompany
                || code == LookupConstants.AgencyType.Cooperative || code == LookupConstants.AgencyType.GovernmentAgency) {
                    return true;                 
            }
            return false;  
        }
    }
}
