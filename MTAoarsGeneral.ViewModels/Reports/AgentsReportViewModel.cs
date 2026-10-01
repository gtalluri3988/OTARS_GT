using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports {
    public class AgentsReportViewModel {

        public string ReportFor { get; set; }

        public DateTime? FromDate{ get; set; }

        public DateTime? ToDate { get; set; }

        public string IntermediaryTypeID { get; set; }

        public string IsBancaStaff { get; set; }

    }
}
