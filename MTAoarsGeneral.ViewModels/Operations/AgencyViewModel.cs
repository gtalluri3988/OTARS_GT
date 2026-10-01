using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class AgencyViewModel : IExistableIntermediary
    {

        [Display(Name = "Agency Name")]
        public string Name { get; set; }

        public bool IsFamily { get; set; }

        public bool IsGeneral { get; set; }

        [Display(Name = "Business Registration Number")]
        public string BusinessRegistrationNumber { get; set; }

        [Display(Name = "Authorized Capital")]
        public decimal? AuthorizedCapital { get; set; }

        [Display(Name = "Paidup Capital")]
        public decimal? PaidupCapital { get; set; }

        public bool? IsStockExchangeListed { get; set; }

        public bool? ShouldDisplayAuthorizedCapital { get; set; }

        public LookupItem Rating { get; set; }

        //public IEnumerable<LookupItem> TerminationActions { get; set; }

        public long StatusID { get; set; }

        [Display(Name = "New Business Registration Number")]
        public string NewBusinessRegistrationNumber { get; set; }

        [Display(Name = "M2 Exam")]
        public LookupItem M2Exam { get; set; }

        [Display(Name = "Date of Exam (DD/MM/YYYY)")]
        public DateTime? DateOfExam { get; set; }


        public LookupItem JoinYear { get; set; }
        public LookupItem JoinMonth { get; set; }

        [Display(Name = "Date 1st Join as Agent")]
        public int? NumberofYears { get; set; }

        [Display(Name = "MTA Awards")]
        public List<LookupItem> MTAAwards { get; set; }
        [Display(Name = "Social Media Address ")]
        public string SocialMediaAddress { get; set; }
        public bool ExcludeM2ExamValidation { get; set; }
    }// class
}// namespace
