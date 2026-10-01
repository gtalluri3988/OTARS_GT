using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Enquiries {
    public class SearchViewModel {

        [Display(Name="Registration number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "NRIC / Passport Number")]
        public string ICNumber { get; set; }

        [Display(Name = "Takaful Operator")]
        public LookupItem Company { get; set; }

        public string CaptchaKey { get; set; }

        public string IPAddress { get; set; }

    }// class

    public class SearchResultViewModel {

        [Display(Name = "MTA Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "NRIC / Passport Number")]
        public string ICNumber { get; set; }

        [Display(Name = "Name of Corporate Nominee (if a corporate agency)")]
        public string CorporateNominee { get; set; }

        [Display(Name = "Name of Agent/Agency")]
        public string AgentName { get; set; }

        [Display(Name = "Principal")]
        public string Company { get; set; }

        [Display(Name = "Intermediary Type")]
        public string IntermediaryType { get; set; }

        [Display(Name = "Valid until")]
        public DateTime? ValidTo { get; set; }

        public string Photo { get; set; }

        public string Qr { get; set; }

        public byte? Rating { get; set; }

        [Display(Name = "MTA Awards")]
        public string MTAAwards { get; set; }

        [Display(Name = "Social Media Address ")]
        public string SocialMediaAddress { get; set; }

        [Display(Name = "Date 1st Join as Agent")]
        public string JoinMonthYear { get; set; }

        [Display(Name = "M2 Exam")]
        public LookupItem M2Exam { get; set; }

    }
}// namespace
