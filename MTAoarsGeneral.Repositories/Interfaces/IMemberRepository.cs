using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces
{
    public interface IMemberRepository : IRepository<Member>
    {

        bool IsMemberExistsInCompany(long memberId, long companyId);

        bool IsMemberExistsInCompany(string icNumber, long companyId);

        bool IsMemberExistsInConflict(string icNumber);

        bool IsMemberReferred(string icNumber);
        bool IsMemberReferred(string icNumber, long? companyId);

        bool IsMemeberReasonNotAllowedForRegistration(string icNumber);
        bool IsMemeberReasonNotAllowedForRegistrationByIntermediary(string icNumber, bool isFamily, bool isGeneral);

        bool IsMemberReferredAndNotAllowedForRegistration(string icNumber);

        Member Get(string icNumber);

        IEnumerable<Member> GetMembers(string icNumber); /* [Issue on Takaful Exam: Exampted Agent Registered on or below 2008] */

        IEnumerable<AgencyPrincipal> GetPrincipals(long memberId);

        IEnumerable<AgencyPrincipal> GetPrincipals(string icNumber);

        IEnumerable<AgencyPrincipalHistory> GetPrincipalHistories(long memberId);

        IEnumerable<Agency> GetAgencies(string icNumber);

        IEnumerable<Agency> GetActiveAgencies(string icNumber);

        IEnumerable<Agency> GetCorporateNomineeActiveAgencies(string icNumber);

        Member GetCorporateNominee(long agencyId);

        ReferredMember GetReferredMember(string icNumber);
        IEnumerable<ReferredMember> GetReferredMembers(string icNumber);
        IEnumerable<ReferredMember> GetReferredMembers(string icNumber, long? companyID);
    }
}
