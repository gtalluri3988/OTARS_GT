using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class AgencyDisplayViewModel
    {

        public long ID { get; set; }

        public string Name { get; set; }

        public string BusinessRegistrationNumber { get; set; }

        public decimal? AuthorizedCapital { get; set; }

        public decimal? PaidupCapital { get; set; }

        public bool? IsStockExchangeListed { get; set; }

        public LookupItem AgencyType { get; set; }

        public bool IsFamily { get; set; }

        public bool IsGeneral { get; set; }

        public DateTime? DateAppointed { get; set; }

        public MemberDisplayViewModel Nominee { get; set; }

        public MemberDisplayViewModel Spouse { get; set; }

        public AddressDisplayViewModel Address { get; set; }

        public GuarantorDisplayViewModel Guarantor { get; set; }

        public AgencyBankerDisplayViewModel AgencyBanker { get; set; }

        public List<AgencyPrincipalDisplayViewModel> AgencyPrincipals
        {
            get;
            set;
        }

        public List<MemberDisplayViewModel> Partners { get; set; }
        public List<MemberDisplayViewModel> Directors { get; set; }
        public List<MemberDisplayViewModel> Shareholders { get; set; }
        public List<MemberDisplayViewModel> AdditionalCorporateNominees { get; set; }

        public bool ShouldDisplayAuthorizedCapital { get; set; }
        public bool IsReferred { get; set; }

        public string PhotoPath { get; set; }

        //To cater agency type (individual) to show norminee name 
        public bool IsIndividual { get; set; }

        //To display message
        //public string ReferredMemberCategoriesDescription { get; set; }
        //public string ReferredMemberDateCreated { get; set; }
        public string ICNumber { get; set; }
        public List<ReferredMemberViewModel> ReferredMembers { get; set; }

        public string NewBusinessRegistrationNumber { get; set; }

        // M2 Examination related properties
        public LookupItem M2Exam { get; set; }
        public DateTime? DateOfExam { get; set; }
        public LookupItem JoinMonth { get; set; }
        public LookupItem JoinYear { get; set; }
        public int? NumberofYears { get; set; }
        public List<LookupItem> MTAAwards { get; set; }
        public string SocialMediaAddress { get; set; }
    }

    public class ReferredMemberViewModel
    {
        public string CategoryDescription { get; set; }
        public string CreatedDate { get; set; }
    }

    public class MemberDisplayViewModel
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public LookupItem ICType { get; set; }

        public string NewICNumber { get; set; }

        public string OldICNumber { get; set; }

        public string PassportNumber { get; set; }

        public bool IsBancaStaff { get; set; }

        public bool IsCitizen { get; set; }

        public bool IsBumiputera { get; set; }

        public bool IsPartTime { get; set; }

        public DateTime? BirthDate { get; set; }

        public LookupItem Level { get; set; }

        public LookupItem Gender { get; set; }

        public LookupItem Race { get; set; }

        public LookupItem Religion { get; set; }

        public LookupItem MaritalStatus { get; set; }

        public decimal? ShareAmount { get; set; }

        public decimal? SharePercentage { get; set; }

        public DateTime? JoinedOn { get; set; }

        public QualificationDisplayViewModel Qualification { get; set; }

        public string NewBusinessRegistrationNumber { get; set; }
        //public long MFPC { get; set; }
        //public long FPAM { get; set; }


    }

    public class AgencyPrincipalDisplayViewModel
    {
        public long CompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string ItermediaryTypeDescription { get; set; }
        public string AgencyTypeDescription { get; set; }
        public string AgencyNumber { get; set; }
        public string Statuses { get; set; }
        public string Remarks { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? DateAppointed { get; set; }
        public LookupItem AgencyType { get; set; }
        public bool IsBancaStaff { get; set; }
        public string Member { get; set; }
        public string PhotoPath { get; set; }
    }

    public class AgencyBankerDisplayViewModel
    {
        public LookupItem Bank { get; set; }
        public LookupItem AccountType { get; set; }
        public string AccountNumber { get; set; }
        public AddressDisplayViewModel Address { get; set; }
    }

    public class AddressDisplayViewModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public LookupItem State { get; set; }
        public string PostalCode { get; set; }
    }

    public class QualificationDisplayViewModel
    {
        public LookupItem EducationalQualification { get; set; }
        public string SchoolName { get; set; }
        public int? Year { get; set; }
        public string InsuranceQualification { get; set; }
    }

    public class GuarantorDisplayViewModel
    {
        public LookupItem GuarantorType { get; set; }

        public string Details { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }

}
