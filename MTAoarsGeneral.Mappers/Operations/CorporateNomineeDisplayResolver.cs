using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Extensions;
using AutoMapper;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;



namespace MTAoarsGeneral.Mappers.Operations {

    class CorporateNomineeDisplayResolver : ValueResolver<Agency, MemberDisplayViewModel> {
        IMemberRepository memberRepository;

        public CorporateNomineeDisplayResolver(IMemberRepository memberRepository) {
            this.memberRepository = memberRepository;
        }

        protected override MemberDisplayViewModel ResolveCore(Agency source) {
            var am = source.AgencyMembers.First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
            var member = am.Member;
            var result = Mapper.Map<MemberDisplayViewModel>(member);
            result.IsPartTime = am.IsPartTime.GetValueOrDefault();
            result.Qualification = new QualificationDisplayViewModel();
            var  qualification = member.MemberEducationalQualifications.FirstOrDefault();
            if (qualification != null) {
                result.Qualification.EducationalQualification = Mapper.Map<LookupItem>(qualification.LookupEducationalQualification);
                result.Qualification.SchoolName = qualification.SchoolName;
                result.Qualification.Year = qualification.Year;
            }

            var ins = member.MemberInsuranceQualifications.FirstOrDefault();    
            if (ins != null) {
                result.Qualification.InsuranceQualification = ins.LookupInsuranceQualification.Description;
            }
            return result;
        }

    }

}
