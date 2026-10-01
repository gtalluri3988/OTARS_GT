using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Operations;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Validators.Operations
{
    public class ReinstateValidator : IValidator<RegistrationIndexViewModel>
    {

        AgencyRepository agencyRepository;
        IMemberRepository memberRepository;
        ILookupRepository lookupRepository;
        IScopeDataProvider dataProvider;

        public ReinstateValidator(AgencyRepository agencyRepository, IMemberRepository memberRepository, ILookupRepository lookupRepository, IScopeDataProvider dataProvider)
        {
            this.agencyRepository = agencyRepository;
            this.memberRepository = memberRepository;
            this.lookupRepository = lookupRepository;
            this.dataProvider = dataProvider;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationIndexViewModel item)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var agency = memberRepository.GetAgencies(item.ICNumber).FirstOrDefault();
            if (agency == null) yield break;
            var principal = agency.AgencyPrincipals.Where(p => p.CompanyID == currentIdentity.CompanyID).FirstOrDefault();
            if (principal != null) yield break;
            var intermediaryTypes = lookupRepository.GetAll<LookupIntermediaryType>();
            if (item.IsFamily)
            {
                var aph = agencyRepository.GetReinstateHistory(item.ICNumber, currentIdentity.CompanyID,
                    intermediaryTypes.First(p => p.Code == LookupConstants.IntermediaryType.Family).ID, item.AgencyType.ID);
                if (aph != null && item.IsGeneral)
                {
                    yield return new ValidationMessage("", "This is eligible for reinstation on Family. Reinstate should happen separately for Family and General");
                }


            }
            if (item.IsGeneral)
            {
                var aph = agencyRepository.GetReinstateHistory(item.ICNumber, currentIdentity.CompanyID,
                    intermediaryTypes.First(p => p.Code == LookupConstants.IntermediaryType.General).ID, item.AgencyType.ID);
                if (aph != null && item.IsFamily)
                {
                    yield return new ValidationMessage("", "This is eligible for reinstation on General. Reinstate should happen separately for Family and General");
                }
            }
        }

    }// class
}// namesapce
