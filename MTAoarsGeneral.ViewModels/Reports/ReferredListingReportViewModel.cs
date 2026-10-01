using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports
{
    public class ReferredListingReportViewModel{

        public DateTime? ApprovedFromDate { get; set; }

        public DateTime? ApprovedToDate { get; set; }

        public long CategoryID { get; set; }

        public string TakafulName { get; set; }

        public int AgentType { get; set; }

        public string NewICNumber { get; set; }
    }
}
