using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class ReferredMemberViewModel
    {

         public string OldICNumber { get; set; }

         public string NewICNumber { get; set; }

         public string Name { get; set; }

         public long ReasonID { get; set; }

         public long CategoryID { get; set; }

         public bool AllowForRegistration { get; set; }
             
         [Display(Name = "Is Active")]
         public bool IsActive { get; set; }

         public bool IsApproved { get; set; }

        public byte[] RecordVersion { get; set; }

        [Display(Name = "Is General")]
        public bool IsGeneral { get; set; }

        [Display(Name = "Is Family")]
        public bool IsFamily { get; set; }

        public string AgencyNumber { get; set; }
        public long? ActionId { get; set; }

        public long? PoliceReportLodgedId { get; set; }
        public DateTime? DateOfOffence { get; set; }

        public DateTime? DateOfInvestigationInitiated { get; set; }
        public DateTime? DateOfInvestigationCompleted { get; set; }
        public DateTime? DateActionTaken { get; set; }


        //public LookupItem ActionTaken { get; set; }

        //public LookupItem PoliceReportLogged { get; set; }

    }
}
