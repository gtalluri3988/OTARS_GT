using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using System.IO;

namespace MTAoarsGeneral.Validators.Operations
{

    public class MemberIntermediaryValidator : BaseValidator, IValidator<RegistrationIndexViewModel>
    {
        IMemberRepository memberRepository;
        IScopeDataProvider dataProvider;

        public MemberIntermediaryValidator(IMemberRepository memberRepository, IScopeDataProvider dataProvider)
        {
            this.memberRepository = memberRepository;
            this.dataProvider = dataProvider;
        }

        // Retained for the General-only inclusion validation path.
        public static IEnumerable<ValidationMessage> ValidateNonBancaGeneralPrincipals(
            IEnumerable<AgencyPrincipal> principals, string icNumber, bool isIndividual)
        {
            var generalPrincipals = (principals ?? Enumerable.Empty<AgencyPrincipal>())
                .Where(p => p != null && p.LookupIntermediaryType != null &&
                    p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General)
                .ToList();
            if (generalPrincipals.Count >= 2)
            {
                var companies = String.Join(", ", generalPrincipals.Select(p =>
                    p.Company == null ? "unknown company" : p.Company.Name));
                yield return new ValidationMessage("", String.Format(
                    "The member with IC number \"{0}\" is already registered under General with the following companies {1}",
                    icNumber, companies));
            }
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationIndexViewModel item)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);

            var principals = memberRepository.GetPrincipals(item.ICNumber).ToList();
            if (principals.Count == 0)
            {
                logger.Info(string.Format("No Agency Found for IC No# : {0}", item.ICNumber));
                yield break;
            }

            var member = memberRepository.Get(item.ICNumber);
            bool isBancaStaff = member != null && member.IsBancaStaff.GetValueOrDefault();

            var familyPrincipals = principals.Where(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.Family &&
                ((p.IsIndividual && (p.Member == null || p.Member.NewICNumber == item.ICNumber)) || !p.IsIndividual)).ToList();
            var familyPrincipal = familyPrincipals.FirstOrDefault(p => p.CompanyID == item.CompanyID)
                ?? familyPrincipals.FirstOrDefault();

            if (familyPrincipal != null && item.IsFamily &&
                (familyPrincipal.CompanyID == item.CompanyID ||
                    !item.AllowConflict ||
                    familyPrincipal.AgencyPrincipalStatus.Count(p => p.LookupAgencyPrincipalStatu.Code == LookupConstants.AgencyPrincipalStatus.NotReleased) > 0))
            {
                yield return new ValidationMessage("", String.Format("The member with IC number \"{0}\" is already registered under Family with {1}", item.ICNumber, familyPrincipal.Company.Name));
            }

            if (isBancaStaff)
            {
                var principal = principals.Where(p => p.IsIndividual != item.IsIndividual).FirstOrDefault();
                if (principal != null)
                {
                    yield return new ValidationMessage("", String.Format("The Banca member with IC number \"{0}\" is already registered under {1} with {2}", item.ICNumber, principal.LookupIntermediaryType.Description, principal.Company.Name));
                }
                yield break;
            }

            var generalPrincipals = principals.Where(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).ToList();
            var notReleasedAgents = generalPrincipals.Any(p => p.AgencyPrincipalStatus.Count(aps => aps.LookupAgencyPrincipalStatu.Code == LookupConstants.AgencyPrincipalStatus.NotReleased) > 0);
            if (generalPrincipals.Count >= 2 && item.IsGeneral && (!item.AllowConflict || notReleasedAgents))
            {
                var sb = new StringBuilder();
                generalPrincipals.ForEach(p => sb.Append(String.Format("{0}, ", p.Company.Name)));
                yield return new ValidationMessage("", String.Format("The member with IC number \"{0}\" is already registered under General with the following companies {1}", item.ICNumber, sb));
            }

            var sameCompanyGeneralPrinciapal = generalPrincipals.FirstOrDefault(x => x.CompanyID == item.CompanyID);
            if (sameCompanyGeneralPrinciapal != null && item.IsGeneral && (!item.AllowConflict || notReleasedAgents))
            {
                var sb = new StringBuilder();
                //  generalPrincipals.ForEach(p => sb.Append(String.Format("{0}, ", p.Company.Name)));
                yield return new ValidationMessage("", String.Format("The member with IC number \"{0}\" is already registered under General with the following company {1} under {2} Agency type", item.ICNumber, sameCompanyGeneralPrinciapal.Company.Name, sameCompanyGeneralPrinciapal.LookupAgencyType.Description));
            }

            //if (item.IsGeneral && generalPrincipals.Any(m => m.CompanyID == item.CompanyID) && !(item.IsIndividual && !principal.IsIndividual)))
            //{
            //    var sbc = new StringBuilder();
            //    generalPrincipals.ForEach(p => sbc.Append(String.Format("{0},", p.Company.Name)));
            //    yield return new ValidationMessage("", String.Format("The member with IC number \"{0}\" is already registered under General with the following companies {1}", item.ICNumber, sbc));
            //}
            //if ((item.IsFamily && principal != null && item.IsIndividual && !principal.IsIndividual && item.CompanyID != principal.CompanyID))
            //{
            //    yield return new ValidationMessage("", String.Format("The member with IC number \"{0}\" is already registered under Family with {1}", item.ICNumber, principal.Company.Name));
            //}
        }

    }// class
}// namespace
