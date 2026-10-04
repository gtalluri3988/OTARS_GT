using System;
using System.Collections.Generic;
using System.Linq;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Validators.Operations
{
    /// <summary>
    /// MemberRepository.GetPrincipals(ic) returns the principals of every agency the IC is a member of,
    /// including agencies where the IC is only a partner, director or shareholder. For the General
    /// matrix (Scenarios 8, 12, 14) those memberships are not registrations of the IC.
    /// A principal is registered to the IC when it is the IC's own Individual/Sole Proprietorship
    /// principal, or the IC is Corporate Nominee / Additional Corporate Nominee of the agency.
    /// </summary>
    public static class RegisteredPrincipalFilter
    {
        public static List<AgencyPrincipal> RegisteredTo(IEnumerable<AgencyPrincipal> principals, string icNumber)
        {
            return (principals ?? Enumerable.Empty<AgencyPrincipal>())
                .Where(p => IsRegisteredTo(p, icNumber))
                .ToList();
        }

        public static bool IsRegisteredTo(AgencyPrincipal principal, string icNumber)
        {
            if (principal == null || string.IsNullOrEmpty(icNumber)) return false;
            if (principal.Member != null && IsSameIC(principal.Member, icNumber)) return true;
            if (principal.Agency == null) return false;
            return principal.Agency.AgencyMembers.Any(am =>
                am.LookupDesignation != null
                && (am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                    || am.LookupDesignation.Code == LookupConstants.Designation.AdditionalCorporateNominee)
                && am.Member != null
                && IsSameIC(am.Member, icNumber));
        }

        static bool IsSameIC(Member member, string icNumber)
        {
            return member.NewICNumber == icNumber || member.PassportNumber == icNumber;
        }

    }// class
}// namespace
