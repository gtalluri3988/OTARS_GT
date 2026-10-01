using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports {
   public class TerminationSummaryReportViewModel {
       public string ReportFor { get; set; }
       public DateTime? FromDate { get; set; }
       public DateTime? ToDate { get; set; }
    }
}
