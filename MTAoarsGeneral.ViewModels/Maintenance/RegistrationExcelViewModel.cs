using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.ViewModels.Maintenance
{
    public class RegistrationExcelViewModel : ExcelViewModel, IExcelViewModel
    {

        public RegistrationExcelViewModel(string rawData)
            : base(rawData)
        {
        }

        public string CompanyCode { get; set; }

        public string AgencyTypeCode { get; set; }

        public string AgencyName { get; set; }

        public string IntermediaryTypeCode { get; set; }

        public string BusinessRegistrationNumber { get; set; }

        public decimal? AuthorizedCapital { get; set; }

        public decimal? PaidupCapital { get; set; }

        public string AgencyAddress1 { get; set; }

        public string AgencyAddress2 { get; set; }

        public string AgencyCity { get; set; }

        public string AgencyStateCode { get; set; }

        public string AgencyPostalCode { get; set; }

        public string NomineeName { get; set; }

        public string NomineeRankCode { get; set; }

        public bool NomineeIsBanacaStaff { get; set; }

        public bool NomineeIsCitizen { get; set; }

        public bool NomineeIsBumiputera { get; set; }

        public string NomineeICTypeCode { get; set; }

        public string NomineeICNumber { get; set; }

        public string NomineeOldICNumber { get; set; }

        public DateTime? NomineeBirthDate { get; set; }

        public string NomineeGenderCode { get; set; }

        public string NomineeRaceCode { get; set; }

        public string NomineeReligionCode { get; set; }

        public string NomineeMaritalStatusCode { get; set; }

        public string NomineePhone { get; set; }

        public string NomineeMobile { get; set; }

        public string NomineeEmail { get; set; }

        public string NomineeFax { get; set; }

        public string NomineeTbeCategoryCode { get; set; }

        public bool NomineeIsPartTime { get; set; }

        public DateTime? NomineeJoinedOn { get; set; }

        public string NomineeSchoolName { get; set; }

        public int? NomineeYear { get; set; }

        public string NomineeEducationalQualificationCode { get; set; }

        public string SpouseName { get; set; }

        public bool SpouseIsCitizen { get; set; }

        public bool SpouseIsBumiputera { get; set; }

        public string SpouseNewICNumber { get; set; }

        public string SpouseOldICNumber { get; set; }

        public DateTime? SpouseBirthDate { get; set; }

        public string SpouseGenderCode { get; set; }

        public string SpouseRaceCode { get; set; }

        public string SpouseReligionCode { get; set; }

        public string SpouseMaritalStatusCode { get; set; }

        public string GuarantorTypeCode { get; set; }

        public string GuarantorDetails { get; set; }

        public decimal? GuarantorAmount { get; set; }

        public DateTime? GuarantorFromDate { get; set; }

        public DateTime? GuarantorToDate { get; set; }

        public string BankCode { get; set; }

        public string AccountTypeCode { get; set; }

        public string AccountNumber { get; set; }

        public string BankAddress1 { get; set; }

        public string BankAddress2 { get; set; }

        public string BankCity { get; set; }

        public string BankStateCode { get; set; }

        public string BankPostalCode { get; set; }

        public string Member1Name { get; set; }
        public string Member1ICType { get; set; }
        public string Member1ICNumber { get; set; }
        public string Member1OldICNumber { get; set; }
        public string Member1DesignationCode { get; set; }
        public string Member1ShareAmount { get; set; }
        public string Member1SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member1NewBusinessRegistrationNumber { get; set; }

        public string Member2Name { get; set; }
        public string Member2ICType { get; set; }
        public string Member2ICNumber { get; set; }
        public string Member2OldICNumber { get; set; }
        public string Member2DesignationCode { get; set; }
        public string Member2ShareAmount { get; set; }
        public string Member2SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member2NewBusinessRegistrationNumber { get; set; }

        public string Member3Name { get; set; }
        public string Member3ICType { get; set; }
        public string Member3ICNumber { get; set; }
        public string Member3OldICNumber { get; set; }
        public string Member3DesignationCode { get; set; }
        public string Member3ShareAmount { get; set; }
        public string Member3SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member3NewBusinessRegistrationNumber { get; set; }

        public string Member4Name { get; set; }
        public string Member4ICType { get; set; }
        public string Member4ICNumber { get; set; }
        public string Member4OldICNumber { get; set; }
        public string Member4DesignationCode { get; set; }
        public string Member4ShareAmount { get; set; }
        public string Member4SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member4NewBusinessRegistrationNumber { get; set; }

        public string Member5Name { get; set; }
        public string Member5ICType { get; set; }
        public string Member5ICNumber { get; set; }
        public string Member5OldICNumber { get; set; }
        public string Member5DesignationCode { get; set; }
        public string Member5ShareAmount { get; set; }
        public string Member5SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member5NewBusinessRegistrationNumber { get; set; }

        public string Member6Name { get; set; }
        public string Member6ICType { get; set; }
        public string Member6ICNumber { get; set; }
        public string Member6OldICNumber { get; set; }
        public string Member6DesignationCode { get; set; }
        public string Member6ShareAmount { get; set; }
        public string Member6SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member6NewBusinessRegistrationNumber { get; set; }

        public string Member7Name { get; set; }
        public string Member7ICType { get; set; }
        public string Member7ICNumber { get; set; }
        public string Member7OldICNumber { get; set; }
        public string Member7DesignationCode { get; set; }
        public string Member7ShareAmount { get; set; }
        public string Member7SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member7NewBusinessRegistrationNumber { get; set; }

        public string Member8Name { get; set; }
        public string Member8ICType { get; set; }
        public string Member8ICNumber { get; set; }
        public string Member8OldICNumber { get; set; }
        public string Member8DesignationCode { get; set; }
        public string Member8ShareAmount { get; set; }
        public string Member8SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member8NewBusinessRegistrationNumber { get; set; }

        public string Member9Name { get; set; }
        public string Member9ICType { get; set; }
        public string Member9ICNumber { get; set; }
        public string Member9OldICNumber { get; set; }
        public string Member9DesignationCode { get; set; }
        public string Member9ShareAmount { get; set; }
        public string Member9SharePercentage { get; set; }
        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string Member9NewBusinessRegistrationNumber { get; set; }

        [SkipExcel()]
        public bool IsOld { get; set; }

        [SkipExcelForAgencyType(LookupConstants.AgencyType.Individual)]
        public string NewBusinessRegistrationNumber { get; set; }

        public string M2Exam { get; set; }

        public string DateOfExam { get; set; }
        //public int? NumberofYears { get; set; }
        public string JoinYear { get; set; }
        public string JoinMonth { get; set; }
        public string MTAAwards { get; set; }
        public string SocialMediaAddress { get; set; }
    }// class
}// namespace
