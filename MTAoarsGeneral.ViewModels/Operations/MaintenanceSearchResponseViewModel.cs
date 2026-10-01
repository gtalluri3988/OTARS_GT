using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class MaintenanceSearchResponseViewModel
    {

        public long AgencyID { get; set; }

        public long CompanyID { get; set; }

        public string IntermediaryTypeDescription { get; set; }

        public string AgencyTypeDescription { get; set; }

        public long AgencyPrincipalID { get; set; }

        public string AgencyNumber { get; set; }

        public string NomineeName { get; set; }

        public string NomineeNewICNumber { get; set; }

        public string CompanyName { get; set; }

        public long? ActionID { get; set; }

        public DateTime ScheduledOn { get; set; }



        public string ValidFromLabel
        {
            get;
            set;
        }

        public string ValidToLabel
        {
            get;
            set;
        }

        public string Remarks { get; set; }
    }
}
