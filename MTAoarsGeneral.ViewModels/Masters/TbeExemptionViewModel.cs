using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Masters
{
    public class TbeExemptionViewModel
    {
        [Display(Name = "New IC Number")]
        public string ICNumber { get; set; }

        [Display(Name = "Old/Police/Army/Passport Number")]
        public string OldICNumber { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Is Citizen")]
        public bool? IsCitizen { get; set; }

        [Display(Name = "Gender")]
        public long? GenderID { get; set; }

        public long? RaceID { get; set; }

        [Display(Name = "Religion")]
        public long? ReligionID { get; set; }

        [Display(Name = "Marital Status")]
        public long? MaritalStatusID { get; set; }

        [Display(Name = "Address1")]
        public string Address1 { get; set; }

        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Display(Name = "State")]
        public long? StateID { get; set; }

        [Display(Name = "PostCode")]
        public string PostCode { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Takaful Exam")]
        public long? TakafulExemptedExamID { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public byte[] RecordVersion { get; set; }

        [Display(Name = "Exemption For")]
        public long? TakafulExemptionForID { get; set; }

        [Display(Name = "Date Document Received")]
        public DateTime? DateDocumentReceived { get; set; }

        [Display(Name = "Date Approved")]
        public DateTime? DateApproved { get; set; }

        [Display(Name = "MTA Invoice Number")]
        public string MTAInvoiceNumber { get; set; }



    }
}
