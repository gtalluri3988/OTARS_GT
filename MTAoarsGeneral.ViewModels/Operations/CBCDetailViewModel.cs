using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations {
    
    public class CBCDetailViewModel {

        public long ID { get; set; }

        public long IntermediaryTypeID { get; set;}

        public long AgencyID { get; set;}

        public long StatusID { get; set;}

        public string IntermediaryTypeDescription { get; set; }

        public string AgencyNumber { get; set; }

        public string NomineeName { get; set; }

        public string NomineeICNumber { get; set; }
    }// class

}// namespace


