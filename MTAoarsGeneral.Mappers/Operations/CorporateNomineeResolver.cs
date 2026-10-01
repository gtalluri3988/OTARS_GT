using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Mappers.Operations {
    class CorporateNomineeResolver : ValueResolver<Agency, CorporateNomineeViewModel> {

        protected override CorporateNomineeViewModel ResolveCore(Agency source) {
            CorporateNomineeViewModel nominee = null;
            var agencyMembers = source.AgencyMembers.Where(i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee).ToList(); //Added [20190702] to cater issue (Wrong norminee name at Maintenance > Change Board Member)
            if (agencyMembers != null) {
                if (agencyMembers.Count > 0) {
                    var member = agencyMembers.First().Member;
                    nominee = Mapper.Map<CorporateNomineeViewModel>(member);
                    nominee.Qualification = new QualificationViewModel();
                    UpdateEducationalQualifications(nominee, member);
                    UpdateInsuranceQualifications(nominee, member);
                }
            }
            return nominee;
        }

        void UpdateEducationalQualifications(CorporateNomineeViewModel nominee, Member member) {
            if (member.MemberEducationalQualifications != null) {
                if (member.MemberEducationalQualifications.Count > 0) {
                    nominee.Qualification.EducationalQualification = Mapper.Map<LookupItem>(member.MemberEducationalQualifications.First().LookupEducationalQualification);
                    nominee.Qualification.SchoolName = member.MemberEducationalQualifications.First().SchoolName;
                    nominee.Qualification.Year = member.MemberEducationalQualifications.First().Year;
                }
            }
        }

        void UpdateInsuranceQualifications(CorporateNomineeViewModel nominee, Member member) {
            if (member.MemberInsuranceQualifications != null) {
                if (member.MemberInsuranceQualifications.Count > 0) {
                    nominee.Qualification.InsuranceQualification = Mapper.Map<LookupItem>(member.MemberInsuranceQualifications.First().LookupInsuranceQualification);

                }
            }
        }


    }// class
}// namespace
