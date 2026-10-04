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
    /// General Takaful agency type matrix (UAT Script - General, Scenarios 1-7 and 16).
    /// A Corporate Nominee of a non-Individual General agency in another TO may only register
    /// again as Individual, or as the same agency type with the same business registration
    /// number. Mixing agency types (e.g. Sole Proprietorship then Partnership) is not allowed.
    /// Same-TO registrations are left to MemberCompanyValidator / MemberIntermediaryValidator.
    /// </summary>
    public class GeneralAgencyTypeValidator : BaseValidator, IValidator<RegistrationIndexViewModel>
    {
        IMemberRepository memberRepository;

        public GeneralAgencyTypeValidator(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationIndexViewModel item)
        {
            if (item == null || !item.IsGeneral || item.AgencyType == null || string.IsNullOrWhiteSpace(item.ICNumber))
                yield break;

            // Individual registration is allowed whatever the existing agency type
            if (item.AgencyType.Code == LookupConstants.AgencyType.Individual)
                yield break;

            // Banca rules are handled by MemberIntermediaryValidator / BancaStaffValidator
            var member = memberRepository.Get(item.ICNumber);
            if (item.IsBancaStaff || (member != null && member.IsBancaStaff.GetValueOrDefault()))
                yield break;

            var nomineePrincipals = memberRepository.GetPrincipals(item.ICNumber)
                .Where(p => p != null
                    && p.CompanyID != item.CompanyID
                    && p.LookupIntermediaryType != null
                    && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General
                    && p.LookupAgencyType != null
                    && p.LookupAgencyType.Code != LookupConstants.AgencyType.Individual
                    && IsCorporateNominee(p.Agency, item.ICNumber))
                .ToList();
            if (nomineePrincipals.Count == 0)
                yield break;

            // Same conflict bypass as MemberIntermediaryValidator, so the conflict flow in
            // RegistrationService.Check still detects the case when AllowConflict is ticked
            var notReleasedAgents = nomineePrincipals.Any(p => p.AgencyPrincipalStatus.Count(aps =>
                aps.LookupAgencyPrincipalStatu.Code == LookupConstants.AgencyPrincipalStatus.NotReleased) > 0);
            if (item.AllowConflict && !notReleasedAgents)
                yield break;

            var differentType = nomineePrincipals.FirstOrDefault(p => p.LookupAgencyType.Code != item.AgencyType.Code);
            if (differentType != null)
            {
                logger.Warn("[RegistrationValidation][General][GeneralAgencyTypeValidator] BLOCKED: existing {0} principal in another TO, requested {1}.",
                    differentType.LookupAgencyType.Code, item.AgencyType.Code);
                yield return new ValidationMessage("", String.Format(
                    "The member with IC number \"{0}\" is already registered under General as Corporate Nominee of {1} Agency type with {2} (Agency Number \"{3}\"). Registration under a different Agency type ({4}) is not allowed",
                    item.ICNumber,
                    differentType.LookupAgencyType.Description,
                    differentType.Company == null ? "another company" : differentType.Company.Name,
                    differentType.AgencyNumber,
                    item.AgencyType.Description));
                yield break;
            }

            var requestedBrn = NormalizeBrn(item.BusinessRegistrationNumber);
            if (string.IsNullOrEmpty(requestedBrn))
                yield break;

            var differentEntity = nomineePrincipals.FirstOrDefault(p =>
                p.Agency != null && NormalizeBrn(p.Agency.BusinessRegistrationNumber) != requestedBrn);
            if (differentEntity != null)
            {
                logger.Warn("[RegistrationValidation][General][GeneralAgencyTypeValidator] BLOCKED: Corporate Nominee of another {0} entity in another TO.",
                    differentEntity.LookupAgencyType.Code);
                yield return new ValidationMessage("", String.Format(
                    "The member with IC number \"{0}\" is already registered under General as Corporate Nominee of {1} (business registration number {2}) with {3}. Only the same business registration number can be registered under {4} Agency type",
                    item.ICNumber,
                    differentEntity.Agency.Name,
                    differentEntity.Agency.BusinessRegistrationNumber,
                    differentEntity.Company == null ? "another company" : differentEntity.Company.Name,
                    item.AgencyType.Description));
            }
        }

        static bool IsCorporateNominee(Agency agency, string icNumber)
        {
            if (agency == null) return false;
            return agency.AgencyMembers.Any(am =>
                am.LookupDesignation != null
                && am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                && am.Member != null
                && (am.Member.NewICNumber == icNumber || am.Member.PassportNumber == icNumber));
        }

        static string NormalizeBrn(string brn)
        {
            return string.IsNullOrWhiteSpace(brn) ? string.Empty : brn.Trim().ToUpperInvariant();
        }

    }// class
}// namespace
