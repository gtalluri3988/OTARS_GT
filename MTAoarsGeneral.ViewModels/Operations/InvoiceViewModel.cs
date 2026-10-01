using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations
{

    public class InvoiceSearchViewModel
    {
        public long CompanyID;
        public int Year;
        public int Month;
        public int InvoiceID;
        public List<InvoiceViewModel> Invoices;
        public List<LookupItem> InvoiceStatuses;
    }

    public class InvoiceViewModel
    {

        public long ID { get; set; }

        public string Number { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal Amount { get; set; }

        public string CompanyName { get; set; }

        public DateTime Date { get; set; }

        public bool IsPaid { get; set; }

        public string Remarks { get; set; }

        public long StatusID { get; set; }

        public LookupItem InvoiceStatus { get; set; }
        public string DisplayDate
        {
            get
            {
                return Date.ToString(GlobalConstants.DateFormat);
            }
        }
    }// class


    public class InvoiceDetailViewModel
    {
        public long ID { get; set; }

        public long JournalID { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string Uri { get; set; }

        public string AgencyNumber { get; set; }

        public string AgencyName { get; set; }

        public string IntermediaryTypeDescription { get; set; }

        public bool? IsBancaStaff { get; set; }

        public DateTime? JournalCreatedDate { get; set; }
    }

    public class InvoiceDetailExportModel
    {
        [DisplayName("Takaful Operator Name")]
        public string CompanyName { get; set; }

        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }

        public string Description { get; set; }

        [DisplayName("Intermediary Type")]
        public string IntermediaryTypeDescription { get; set; }

        [DisplayName("Corporate Status")]
        public string AgencyTypeDescription { get; set; }

        [DisplayName("Banca")]
        public string IsBancaStaff { get; set; }

        [DisplayName("Ageny Name")]
        public string AgencyName { get; set; }

        [DisplayName("Ageny Number")]
        public string AgencyNumber { get; set; }

        [DisplayName("B/R Number")]
        public string BusinessRegistrationNumber { get; set; }

        [DisplayName("New B/R Number")]
        public string NewBusinessRegistrationNumber { get; set; }

        [DisplayName("Nominee")]
        public string NomineeName { get; set; }

        [DisplayName("Nominee NRIC Number")]
        public string NomineeICNumber { get; set; }

        [DisplayName("Valid From")]
        public DateTime? ValidFrom { get; set; }

        [DisplayName("Valid To")]
        public DateTime? ValidTo { get; set; }

        public decimal Amount { get; set; }
    }


}// namespace
