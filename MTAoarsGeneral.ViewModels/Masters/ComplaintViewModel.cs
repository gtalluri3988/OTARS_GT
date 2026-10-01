using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class ComplaintViewModel {

        public long ID { get; set; }

        public long AgencyID { get; set; }

        public long CompanyID { get; set; }

        public long IntermediaryTypeID { get; set; }

        public string Description { get; set; }

        public string Action { get; set; }

        public DateTime? ActionDate { get; set; }

        public byte[] RecordVersion { get; set; }

    }// class
}// namespace
