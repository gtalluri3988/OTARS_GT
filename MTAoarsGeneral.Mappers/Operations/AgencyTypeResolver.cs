using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Mappers.Operations {

   class AgencyTypeResolver : ValueResolver<Agency, LookupItem> {

        ILookupRepository lookupRepository;
        IScopeDataProvider dataProvider;
        public AgencyTypeResolver(ILookupRepository lookupRepository, IScopeDataProvider dataProvider) {
            this.lookupRepository = lookupRepository;
            this.dataProvider = dataProvider;
        }

        protected override LookupItem ResolveCore(Agency source) {
            var companyId = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity).CompanyID;
            var ap = source.AgencyPrincipals.FirstOrDefault(p => p.CompanyID == companyId && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General);
            if (ap == null) return null;
            return Mapper.Map<LookupItem>(ap.LookupAgencyType);

        }

    }// class

}// namespace
