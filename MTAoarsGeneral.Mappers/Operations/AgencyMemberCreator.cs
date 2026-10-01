using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using AutoMapper;

namespace MTAoarsGeneral.Mappers.Operations
{
    public class AgencyMemberCreator
    {

        ILookupRepository lookupRepository;
        IMemberRepository memberRepository;

        public AgencyMemberCreator(ILookupRepository lookupRepository, IMemberRepository memberRepository)
        {
            this.lookupRepository = lookupRepository;
            this.memberRepository = memberRepository;
        }

        public AgencyMember GetCorporateNominee(RegistrationViewModel source)
        {
            String ic = source.CorporateNominee.ICType.Code == LookupConstants.ICTypes.NewIc ? source.CorporateNominee.NewICNumber : source.CorporateNominee.PassportNumber;
            var cn = memberRepository.Get(ic);
            var member = cn == null ? Mapper.Map<CorporateNomineeViewModel, Member>(source.CorporateNominee) : Mapper.Map(source.CorporateNominee, cn);
            //member.MemberEducationalQualifications.Add(GetEducationQualification(source.CorporateNominee.Qualification));
            // var insuranceQualification = GetInsuranceQualification(source.CorporateNominee);
            // if (insuranceQualification != null) member.MemberInsuranceQualifications.Add(insuranceQualification);
            var agencyMember = new AgencyMember { Member = member, Agency = member.AgencyMembers.OrderByDescending(m => m.AgencyID).First().Agency };
            agencyMember.DesignationID = lookupRepository.Get<LookupDesignation>(LookupConstants.Designation.CorporateNominee).ID;
            agencyMember.IsPartTime = source.CorporateNominee.IsPartTime;
            return agencyMember;
        }

        public AgencyMember GetCorporateNomineeAsPartner(Member corporateNominee)
        {
            var output = new AgencyMember { Member = corporateNominee };
            output.DesignationID = lookupRepository.Get<LookupDesignation>(LookupConstants.Designation.Partner).ID;
            return output;
        }

        public IEnumerable<AgencyMember> GetPartners(PartnershipRegistrationViewModel source)
        {
            foreach (var item in source.Partners)
            {
                var agencyMember = new AgencyMember();
                var dbMember = memberRepository.Get(item.ICNumber);
                if (dbMember != null) agencyMember.MemberID = dbMember.ID;
                else agencyMember.Member = Mapper.Map<Member>(item);
                agencyMember.DesignationID = lookupRepository.Get<LookupDesignation>(LookupConstants.Designation.Partner).ID;
                yield return agencyMember;
            }
        }

        public IEnumerable<AgencyMember> GetBoardMembers(Member corporateNominee, IExistableBoardMembers source)
        {
            var list = new List<AgencyMember>();
            foreach (var item in source.Directors)
            {
                var member = Mapper.Map<Member>(item);
                var agencyMember = GetAgencyMember(list, member, corporateNominee);
                agencyMember.DesignationID = lookupRepository.Get<LookupDesignation>(LookupConstants.Designation.Director).ID;
                list.Add(agencyMember);
            }
            foreach (var item in source.Shareholders)
            {
                var member = Mapper.Map<Member>(item);
                var agencyMember = GetAgencyMember(list, member, corporateNominee);
                agencyMember.AmountShareholding = item.ShareAmount;
                agencyMember.ShareholdingPercentage = Convert.ToDouble(item.SharePercentage);
                agencyMember.NewBusinessRegistrationNumber = item.NewBusinessRegistrationNumber;
                agencyMember.DesignationID = lookupRepository.Get<LookupDesignation>(LookupConstants.Designation.Shareholder).ID;
                list.Add(agencyMember);
            }
            foreach (var item in source.AdditionalCorporateNominees)
            {
                var member = Mapper.Map<Member>(item);
                var agencyMember = GetAgencyMember(list, member, corporateNominee);
                agencyMember.DesignationID = lookupRepository.Get<LookupDesignation>(LookupConstants.Designation.AdditionalCorporateNominee).ID;
                list.Add(agencyMember);
            }
            return list;
        }


        private bool IsEqual(Member m1, Member m2)
        {
            if (m1 == null || m2 == null)
                return false;
            var icType = lookupRepository.Get<LookupICType>(m1.ICTypeID.Value);
            if (icType != null)
            {
                if (m1.ICTypeID == m2.ICTypeID && m1.NewICNumber == m2.NewICNumber && icType.Code == LookupConstants.ICTypes.NewIc) return true;
                if (m1.ICTypeID == m2.ICTypeID && m1.PassportNumber == m2.PassportNumber && icType.Code == LookupConstants.ICTypes.PoliceArmyPassport) return true;
            }
            return false;
        }

        private AgencyMember GetAgencyMember(List<AgencyMember> list, Member member, Member corporateNominee)
        {
            var dbMember = memberRepository.Get(Coalesce(member.NewICNumber, member.PassportNumber));
            if (dbMember != null) return new AgencyMember { MemberID = dbMember.ID };
            foreach (var ag in list)
            {
                if (IsEqual(ag.Member, member)) return new AgencyMember { Member = ag.Member };
            }
            if (IsEqual(member, corporateNominee)) return new AgencyMember { Member = corporateNominee };
            return new AgencyMember { Member = member };
        }

        private string Coalesce(params string[] arguments)
        {
            foreach (var item in arguments)
            {
                if (String.IsNullOrWhiteSpace(item) == false) return item;
            }
            return null;
        }

    }// class
}// namespace
