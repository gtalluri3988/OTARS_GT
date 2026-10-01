using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Managers;

namespace MTAoarsGeneral.Validators.Operations
{
    public class MemberCompanyValidator : IValidator<RegistrationIndexViewModel>
    {
        IMemberRepository memberRepository;
        IScopeDataProvider dataProvider;

        public MemberCompanyValidator(IMemberRepository memberRepository, IScopeDataProvider dataProvider)
        {
            this.memberRepository = memberRepository;
            this.dataProvider = dataProvider;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationIndexViewModel item)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var principals = memberRepository.GetPrincipals(item.ICNumber);


            if (item.IsGeneral)
            {
                if (principals.Count(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General
                    && p.CompanyID == currentIdentity.CompanyID && p.LookupAgencyType.Code == item.AgencyType.Code) > 0)
                {
                    yield return new ValidationMessage("", "The member is already exists under General intermediary type in the same company");
                }
            }
        }

    }// class
}// namespace
