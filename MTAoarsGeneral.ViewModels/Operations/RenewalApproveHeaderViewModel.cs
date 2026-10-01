using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class RenewalApproveHeaderViewModel {
        
        public LookupItem Company { get; set; }

        public LookupItem Year { get; set; }

        public LookupItem Quarter { get; set; }

    }// class
}// namespace
