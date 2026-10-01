using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Builders.Shared;
using MTAoarsGeneral.Validators;

namespace MTAoarsGeneral.Builders.Operations {
    public class CBCBuilder : BaseBuilder, ICBCBuilder {

        IObjectCreator viewModelCreator;
        ILookupRepository lookupRepository;
        IScopeDataProvider dataProvider;
        IAgencyBuilder agencyBuilder;
        long detailGenerated;
        long generalIntermediaryTypeId;

        public CBCBuilder(IObjectCreator viewModelCreator, ILookupRepository lookupRepository, IScopeDataProvider dataProvider, IAgencyBuilder agencyBuilder) {
            this.viewModelCreator = viewModelCreator;
            this.lookupRepository = lookupRepository;
            this.dataProvider = dataProvider;
            this.agencyBuilder = agencyBuilder;
            this.agencyBuilder.CurrentContext = this.CurrentContext;
            detailGenerated = lookupRepository.Get<LookupCBCDetailStatus>(LookupConstants.CBCDetailStatus.Generated).ID;
            generalIntermediaryTypeId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
        }

        public CBCStartViewModel GetStartViewModel() {
            var model = viewModelCreator.Create<CBCStartViewModel>();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (identity.IsMTA == false) {
                model.Company = new LookupItem { ID = identity.CompanyID, Code = identity.CompanyCode, Description = identity.CompanyName };
            }
            return model;
        }
        
        public CBCDetailViewModel Search(string agencyNumber, long companyId) {
            var model = agencyBuilder.Search<CBCDetailViewModel>(agencyNumber, companyId);
            if (model == null) return null;
            if (model.IntermediaryTypeID != generalIntermediaryTypeId) {
                CurrentContext.ValidationMessages.Add(new ValidationMessage("", "CBC is applied only for general insurance agents"));
                return null;
            }
            model.StatusID = detailGenerated;
            return model;
        }

    }// class
}// namespace
