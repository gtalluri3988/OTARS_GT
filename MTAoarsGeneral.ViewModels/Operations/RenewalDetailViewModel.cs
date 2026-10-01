using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class RenewalDetailViewModel
    {

        public RenewalDetailViewModel()
        {
            RenewalStatus = new LookupItem();
        }

        public long ID { get; set; }

        public long AgencyPrincipalID { get; set; }

        public long AgencyID { get; set; }

        public string AgencyTypeDescription { get; set; }

        public string AgencyName { get; set; }

        public string AgencyNumber { get; set; }

        public long StatusID { get; set; }
        public long NotToRelease { get; set; }
        public string NotToReleaseDescription
        {
            get
            {
                return NotToRelease == 1 ? "Yes" : "No";
            }
        }


        public string StatusDescription { get; set; }

        public long IntermediaryTypeID { get; set; }

        public string IntermediaryTypeDescription { get; set; }

        public bool IsBancaStaff { get; set; }

        public string Remarks { get; set; }

        public LookupItem RenewalStatus
        {
            get;
            set;
        }

        string description;
        public string Description
        {
            set { description = value; }
            get
            {
                if (String.IsNullOrEmpty(description)) return String.Empty;
                return description;
            }
        }

        public bool IsSubmitted { get; set; }

        public bool IsProcesses { get; set; }

        // CR Number 20190719-01 (Add IC No#/BR No#, address)
        // -----------------------------------------------------------------
        public string ICNoOrBusinessRegistrationNo { get; set; }
        public string Address { get; set; }

    }// class
}// namespace
