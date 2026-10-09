using System;
using System.Collections.Generic;
using System.Linq;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Validators.Operations
{
    /// <summary>
    /// General matrix Scenarios 10 and 11: an entity (same BRN or new BRN) already registered under
    /// General can only be registered again, at the same or another TO, by its own Corporate Nominee.
    /// A partner/director/shareholder of that entity cannot register it as a new Corporate Nominee.
    /// BRNs are compared ignoring case, spaces, '-', '/' and '.', and BRN and new BRN are cross-checked.
    /// </summary>
    public class RegisteredEntityNomineeValidator : BaseValidator, IValidator<RegistrationIndexViewModel>
    {
        IAgencyRepository agencyRepository;

        public RegisteredEntityNomineeValidator(IAgencyRepository agencyRepository)
        {
            this.agencyRepository = agencyRepository;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationIndexViewModel item)
        {
            if (item == null || !item.IsGeneral || item.AgencyType == null || string.IsNullOrWhiteSpace(item.ICNumber))
                yield break;
            if (item.AgencyType.Code == LookupConstants.AgencyType.Individual)
                yield break;

            var registered = agencyRepository.SelectByRegistrationNumber(item.BusinessRegistrationNumber, item.NewBusinessRegistrationNumber)
                .Select(a => new { Agency = a, Principals = GeneralCorporatePrincipals(a), Nominees = CorporateNominees(a) })
                .Where(r => r.Principals.Count > 0 && r.Nominees.Count > 0)
                .ToList();
            if (registered.Count == 0)
                yield break;

            // Legacy data can hold the same entity twice under different nominees (BRN typed in different
            // formats). The nominee of any of those agencies is still allowed, as before this rule.
            if (registered.Any(r => r.Nominees.Any(m => m.NewICNumber == item.ICNumber || m.PassportNumber == item.ICNumber)))
                yield break;

            var match = registered.FirstOrDefault(r => r.Principals.Any(p => p.CompanyID == item.CompanyID)) ?? registered.First();
            var agency = match.Agency;
            var principal = match.Principals.FirstOrDefault(p => p.CompanyID == item.CompanyID) ?? match.Principals.First();
            logger.Warn("[RegistrationValidation][General][RegisteredEntityNomineeValidator] BLOCKED: entity AgencyID={0} already registered with another Corporate Nominee.", agency.ID);
            yield return new ValidationMessage("",
                "Business registration number {0} is already registered under General as {1} (Agency Number \"{2}\") with {3} under another Corporate Nominee. Only the registered Corporate Nominee can register this {4}",
                string.IsNullOrWhiteSpace(agency.BusinessRegistrationNumber) ? agency.NewBusinessRegistrationNumber : agency.BusinessRegistrationNumber,
                agency.Name,
                principal.AgencyNumber,
                principal.Company == null ? "another company" : principal.Company.Name,
                item.AgencyType.Description);
        }

        static List<AgencyPrincipal> GeneralCorporatePrincipals(Agency agency)
        {
            return agency.AgencyPrincipals.Where(p =>
                p.LookupIntermediaryType != null
                && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General
                && p.LookupAgencyType != null
                && p.LookupAgencyType.Code != LookupConstants.AgencyType.Individual).ToList();
        }

        static List<Member> CorporateNominees(Agency agency)
        {
            return agency.AgencyMembers.Where(am =>
                am.LookupDesignation != null
                && am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                && am.Member != null).Select(am => am.Member).ToList();
        }

    }// class
}// namespace
