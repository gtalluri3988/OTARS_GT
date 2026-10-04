using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using System.Globalization;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Validators.Operations
{
    public class BoardMembersValidator : IValidator<IExistableBoardMembers>
    {

        ILookupRepository lookupRepository;
        IMemberRepository memberRepository;

        public BoardMembersValidator(ILookupRepository lookupRepository, IMemberRepository memberRepository)
        {
            this.lookupRepository = lookupRepository;
            this.memberRepository = memberRepository;
        }

        public IEnumerable<ValidationMessage> Validate(IExistableBoardMembers item)
        {


            var cnIcNumber = (item as RegistrationViewModel).CorporateNominee.PassportNumber;
            if ((item as RegistrationViewModel).CorporateNominee.ICType.Code == LookupConstants.ICTypes.NewIc)
            {
                cnIcNumber = (item as RegistrationViewModel).CorporateNominee.NewICNumber;
            }
            foreach (var message in CheckMember(item.Directors.Cast<IMemberViewModel>(), cnIcNumber, "Director"))
            {
                yield return message;
            }

            foreach (var message in CheckMember(item.Shareholders.Cast<IMemberViewModel>(), cnIcNumber, "Shareholder"))
            {
                yield return message;
            }

            foreach (var message in CheckMember(item.AdditionalCorporateNominees.Cast<IMemberViewModel>(), cnIcNumber, "Additional Corporate Nominee"))
            {
                yield return message;
            }

            /* var percentage = item.Shareholders.Sum(p => p.SharePercentage);
             if (percentage < 99 || percentage > 100) {
                 yield return new ValidationMessage("", "Shareholders total percentage should be 99 to 100");
             }*/
        }


        bool CanAllow(string cnIcNumber, string boardMemberIcNumber)
        {
            var agencies = memberRepository.GetActiveAgencies(boardMemberIcNumber);
            if (agencies.Count() == 0) return true;
            var count = agencies.SelectMany(p => p.AgencyMembers)
                .Where(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                && p.Member.NewICNumber == cnIcNumber || p.Member.PassportNumber == cnIcNumber).Count();
            return count > 0;
        }

        IEnumerable<ValidationMessage> CheckMember(IEnumerable<IMemberViewModel> members, string cnIcNumber, string designation)
        {
            foreach (var member in members)
            {
                if (String.IsNullOrEmpty(member.ICNumber) && member.ICType != null && !string.IsNullOrEmpty(member.ICType.Code))
                {
                    yield return new ValidationMessage("", "IC number is required for {0}", designation);
                    continue;
                }
                if (member.ICType.Code == LookupConstants.ICTypes.NewIc && !member.ICNumber.IsNewIC())
                {
                    yield return new ValidationMessage("", "{0}'s New IC number {1} should be 12 digits and begin with date of birth (yyMMdd) format", designation, member.ICNumber);
                    continue;
                }
                // The Corporate Nominee may also be a director/shareholder of his own company,
                // even if he is only a director/shareholder of another agency (matrix S14, S15)
                var isNomineeSelf = designation != "Additional Corporate Nominee" && member.ICNumber == cnIcNumber;
                if (!isNomineeSelf && !CanAllow(cnIcNumber, member.ICNumber))
                {
                    yield return new ValidationMessage("", "{0}'s New IC number {1} and Corporate nomine IC number are in different company", designation, member.ICNumber);
                    continue;
                }
                var count = members.Count(p => p.ICNumber == member.ICNumber);
                if (count > 1)
                {
                    yield return new ValidationMessage("", "{0} with IC Number {1} appears {2} times. Remove all except one", designation, member.ICNumber, count.ToString());
                }
            }
        }

    }// class
}// namespace
