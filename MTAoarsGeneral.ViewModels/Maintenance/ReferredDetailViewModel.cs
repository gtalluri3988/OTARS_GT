using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Maintenance {
    
    public class ReferredDetailViewModel {

        public ReferredDetailViewModel() {
            IsActive = true;
            Attachments = new List<ReferredAttachmentViewModel>();
        }
        public long ID { get; set; }

        public long HeaderID { get; set; }

        public string Name { get; set; }

        public string ICNumber { get; set; }

        public string OldICNumber { get; set; }

        public LookupItem Reason { get; set; }

        public string Comments { get; set; }

        public string ApproverComments { get; set; }

        public bool IsActive { get; set; }

        public bool AllowForRegistration { get; set; }

        public LookupItem Status { get; set; }

        public List<ReferredAttachmentViewModel> Attachments { get; private set; }

        public LookupItem Category { get; set; }

        public string TOName { get; set; }

        public bool IsGeneral { get; set; }

        public bool IsFamily { get; set; }

        public string AgencyNumber { get; set; }

        public Identity Identity { get; set; }

        public LookupItem ActionTaken { get; set; }

        public LookupItem PoliceReportLogged { get; set; }


        public DateTime? DateOfOffence { get; set; }

        public DateTime? DateOfInvestigationInitiated { get; set; }
        public DateTime? DateOfInvestigationCompleted { get; set; }
        public DateTime? DateActionTaken { get; set; }



    }// class

}// namspace
