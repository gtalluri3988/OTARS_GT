using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Maintenance {
    
    public class AdminActivityDetailViewModel {
        public long AgencyID { get; set; }

        public string AgencyNumber { get; set; }

        public string IntermediaryTypeDescription { get; set; }

        public string NomineeName { get; set; }

        public string NomineeICNumber { get; set; }

        public bool IsChargeable { get; set; }

    }// class

}// namespace
