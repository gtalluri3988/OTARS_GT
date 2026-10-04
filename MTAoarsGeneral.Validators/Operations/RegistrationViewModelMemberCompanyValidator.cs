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

    public class RegistrationViewModelMemberCompanyValidator : IValidator<RegistrationViewModel>
    {
        IMemberRepository memberRepository;
        IScopeDataProvider dataProvider;

        public RegistrationViewModelMemberCompanyValidator(IMemberRepository memberRepository, IScopeDataProvider dataProvider)
        {
            this.memberRepository = memberRepository;
            this.dataProvider = dataProvider;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationViewModel item)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var principals = memberRepository.GetPrincipals(item.CorporateNominee.NewICNumber);

            var generalPrincipals = principals.Where(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).ToList();
            var familyPrincipals = principals.Where(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.Family).ToList();

            var individualPrincipals = principals.Where(p => AgencyManager.IsIndividual(p.LookupAgencyType.Code)).ToList();
            var corperatePrincipals = principals.Where(p => !AgencyManager.IsIndividual(p.LookupAgencyType.Code)).ToList();

            var corperateGeneralPrincipals = generalPrincipals.Where(p => !AgencyManager.IsIndividual(p.LookupAgencyType.Code)).ToList();
            var corperateFamilyPrincipals = familyPrincipals.Where(p => !AgencyManager.IsIndividual(p.LookupAgencyType.Code)).ToList();

            var individualGeneralPrincipals = generalPrincipals.Where(p => AgencyManager.IsIndividual(p.LookupAgencyType.Code)).ToList();
            var individualFamilyPrincipals = familyPrincipals.Where(p => AgencyManager.IsIndividual(p.LookupAgencyType.Code)).ToList();


            // Partner/director/shareholder memberships are not General registrations (matrix S14)
            var registeredGeneralPrincipals = RegisteredPrincipalFilter.RegisteredTo(generalPrincipals, item.CorporateNominee.NewICNumber);
            if (item.Agency.IsGeneral && registeredGeneralPrincipals.Exists(p => p.CompanyID == item.CompanyID && p.LookupAgencyType.Code == item.AgencyType.Code))
                yield return new ValidationMessage("", "The member is already exists under General intermediary type in the same company");

            if (item.Agency.IsFamily && familyPrincipals.Exists(p => p.CompanyID == item.CompanyID))
                yield return new ValidationMessage("", "The member is already exists under Family intermediary type in the same company");

            //[Corporate General] Agent in TO-A wish to register as [Individual Family/General] in TO-B is allowed
            // if (corperateGeneralPrincipals.Count > 0 && !individualPrincipals.Exists(p => p.CompanyID == item.CompanyID))
            //    yield break;

            if (corperateGeneralPrincipals.Count > 0 && individualPrincipals.Exists(p => p.CompanyID == item.CompanyID))
                yield break;

            //[Corporate Family] Agent in TO-A wish to register as [Individual Family] Agent in TO-B or vice versa are not allowed
            if (item.Agency.IsFamily && AgencyManager.IsIndividual(item.AgencyType.Code) &&
                corperateFamilyPrincipals.Where(p => p.CompanyID != item.CompanyID).Count() > 0)
            {
                yield return new ValidationMessage("", "The member is already exists under Family intermediary type in the other company as Corporate");
            }
            if (item.Agency.IsFamily && !AgencyManager.IsIndividual(item.AgencyType.Code) &&
                individualFamilyPrincipals.Where(p => p.CompanyID != item.CompanyID).Count() > 0)
            {
                yield return new ValidationMessage("", "The member is already exists under Family intermediary type in the other company as Individual");
            }
        }

    }// class
}
