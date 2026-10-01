using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Repositories.Operations
{

    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {

        public MemberRepository(EntityContext context)
            : base(context)
        {
        }

        public bool IsMemberExistsInCompany(long memberId, long companyId)
        {
            return Context.Members.Where(
                 member => member.AgencyMembers.Any(
                     agencyMember => agencyMember.Agency.AgencyPrincipals.Any(
                         agencyPrincipal => agencyPrincipal.CompanyID == companyId)
                                             )
                         && member.ID == memberId).Count() > 0;

        }

        public bool IsMemberExistsInCompany(string icNumber, long companyId)
        {
            return Context.Members.Where(
                 member => member.AgencyMembers.Any(
                     agencyMember => agencyMember.Agency.AgencyPrincipals.Any(
                         agencyPrincipal => agencyPrincipal.CompanyID == companyId)
                                             )
                         &&
                         (member.NewICNumber == icNumber || member.PassportNumber == icNumber)
                         ).Count() > 0;

        }

        public bool IsMemberExistsInConflict(string icNumber)
        {
            return Context.Members.Where(
                 member => member.AgencyMembers.Any(
                     agencyMember => agencyMember.Agency.AgencyPrincipalConflicts.Any(conflict =>
                         conflict.IsActive &&
                            (
                                conflict.LookupConflictStatus.Code != LookupConstants.ConflictStatus.Closed
                                && conflict.LookupConflictStatus.Code != LookupConstants.ConflictStatus.Rejected
                                && conflict.LookupConflictStatus.Code != LookupConstants.ConflictStatus.AutoClose)
                            ))
                         &&
                         (member.NewICNumber == icNumber || member.PassportNumber == icNumber)
                         ).Count() > 0;

        }

        public bool IsMemberReferred(string icNumber)
        {
            //return Context.ReferredMembers.Count(p => p.NewICNumber == icNumber || p.OldICNumber == icNumber) > 0;
            return IsMemberReferred(icNumber, null);
        }
        public bool IsMemberReferred(string icNumber, long? companyID)
        {
            //if (companyID == null)
            //    return Context.ReferredMembers.Count(p => (p.NewICNumber == icNumber || p.OldICNumber == icNumber)) > 0;

            //var company = Context.Companies.Where(i => i.ID == companyID).FirstOrDefault();
            //if (company == null) return false;

            //return Context.ReferredMembers.Any(p => 
            //    (p.NewICNumber == icNumber || p.OldICNumber == icNumber) && 
            //    ((p.IsFamily && company.IsFamily) || (p.IsGeneral && company.IsGeneral))
            //);

            //[2021-04-02] To cater active referred member only
            return GetReferredMembers(icNumber, companyID).Where(i => i.IsActive).Any();
        }

        public bool IsMemberReferredAndNotAllowedForRegistration(string icNumber)
        {

            return Context.ReferredMembers
                .Where(p => p.AllowForRegistration == false)
                .Count(p => p.NewICNumber == icNumber || p.OldICNumber == icNumber) > 0;
        }

        public bool IsMemeberReasonNotAllowedForRegistration(string icNumber)
        {
            return Context.ReferredMembers.Where(rm => rm.OldICNumber == icNumber || rm.NewICNumber == icNumber).Any(p => p.LookupReferredReason.AllowForRegistration == false);
        }

        public bool IsMemeberReasonNotAllowedForRegistrationByIntermediary(string icNumber, bool isFamily, bool isGeneral)
        {
            return Context.ReferredMembers.Where(rm => (rm.OldICNumber == icNumber || rm.NewICNumber == icNumber) && rm.IsGeneral)
                .Any(p => p.LookupReferredReason.AllowForRegistration == false);
        }

        public Member Get(string icNumber)
        {
            return Context.Members.Where(member => member.NewICNumber == icNumber || member.PassportNumber == icNumber)
                .OrderByDescending(p => p.CreatedDate)
                .FirstOrDefault();
        }

        public IEnumerable<Member> GetMembers(string icNumber) /* [Issue on Takaful Exam: Exampted Agent Registered on or below 2008] */
        {
            return Context.Members.Where(member => member.NewICNumber == icNumber || member.PassportNumber == icNumber)
                .OrderByDescending(p => p.CreatedDate);
        }

        public IEnumerable<AgencyPrincipal> GetPrincipals(long memberId)
        {
            return Context.AgencyPrincipals.Where(
                    agencyPrincipal => agencyPrincipal.Agency.AgencyMembers.Any(agencyMember => agencyMember.MemberID == memberId)
                ).AsEnumerable();
        }

        public IEnumerable<AgencyPrincipal> GetPrincipals(string icNumber)
        {
            var agencyPrincipals = Context.AgencyPrincipals.Where(
                agencyPrincipal => agencyPrincipal.Agency.AgencyMembers.Any(
                    agencyMember => agencyMember.Member.NewICNumber == icNumber || agencyMember.Member.PassportNumber == icNumber)
            ).AsEnumerable();
            ExcludeInvalidMemberID(icNumber, ref agencyPrincipals);
            return agencyPrincipals;
        }

        private void ExcludeInvalidMemberID(string icNumber, ref IEnumerable<AgencyPrincipal> agencyPrincipals)
        {
            var members = GetMembers(icNumber);
            if (members.Count() > 0)
            {
                var memberIDs = members.Select(i => i.ID).ToList();
                var excludeAgencyPrincipals = agencyPrincipals.Where(i =>
                    i.MemberID.HasValue && i.MemberID > 0 && !(memberIDs.Contains((long)i.MemberID))
                );
                agencyPrincipals = agencyPrincipals.Except(excludeAgencyPrincipals);
            }
        }

        public IEnumerable<AgencyPrincipalHistory> GetPrincipalHistories(long memberId)
        {
            return Context.AgencyPrincipalHistories.Where(
                    aph => aph.Agency.AgencyMembers.Any(agencyMember => agencyMember.MemberID == memberId)
                ).AsEnumerable();
        }

        public IEnumerable<Agency> GetAgencies(string icNumber)
        {
            return Context.Agencies.Where(
                    agency => agency.AgencyMembers.Any(
                        agencyMember => agencyMember.Member.NewICNumber == icNumber || agencyMember.Member.PassportNumber == icNumber)
                ).AsEnumerable();
        }

        public IEnumerable<Agency> GetActiveAgencies(string icNumber)
        {
            return Context.Agencies.Where(
                    agency => agency.AgencyMembers.Any(
                        agencyMember => agencyMember.Member.NewICNumber == icNumber || agencyMember.Member.PassportNumber == icNumber)
                        && agency.AgencyPrincipals.Count > 0
                ).AsEnumerable();
        }

        public IEnumerable<Agency> GetCorporateNomineeActiveAgencies(string icNumber)
        {
            return Context.Agencies.Where(
                    agency => agency.AgencyMembers.Any(
                        agencyMember => (agencyMember.Member.NewICNumber == icNumber || agencyMember.Member.PassportNumber == icNumber)
                        && agencyMember.DesignationID == 1) //Change to only get Corporate Nominee Only
                        && agency.AgencyPrincipals.Count > 0
                ).AsEnumerable();
        }

        public Member GetCorporateNominee(long agencyId)
        {
            return Context.Members.Where(
                member => member.AgencyMembers.Any(
                    agencyMember => agencyMember.AgencyID == agencyId
                    && agencyMember.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee)
                    ).FirstOrDefault();
        }

        public ReferredMember GetReferredMember(string icNumber)
        {
            return Context.ReferredMembers.FirstOrDefault(p => p.NewICNumber == icNumber || p.OldICNumber == icNumber);
        }

        public IEnumerable<ReferredMember> GetReferredMembers(string icNumber)
        {
            if (string.IsNullOrWhiteSpace(icNumber))
                return new List<ReferredMember>();
            return Context.ReferredMembers.Where(p => p.NewICNumber == icNumber || p.OldICNumber == icNumber).AsEnumerable();
        }

        public IEnumerable<ReferredMember> GetReferredMembers(string icNumber, long? companyID)
        {
            if (string.IsNullOrWhiteSpace(icNumber))
                return new List<ReferredMember>();

            if (companyID == null)
                return Context.ReferredMembers.Where(p => (p.NewICNumber == icNumber || p.OldICNumber == icNumber)).AsEnumerable();

            var company = Context.Companies.FirstOrDefault(i => i.ID == companyID);
            if (company == null) return new List<ReferredMember>();

            return Context.ReferredMembers.Where(p =>
                (p.NewICNumber == icNumber || p.OldICNumber == icNumber) &&
                p.IsGeneral && company.IsGeneral
            );
        }

    }// class

}// namespace
