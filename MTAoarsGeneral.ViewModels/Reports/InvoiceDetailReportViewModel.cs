using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports
{
    public class InvoiceDetailReportViewModel{           

        public int Year { get; set; }

        public int Month { get; set; }

        public long CompanyID { get; set; }

        public string ReportFor { get; set; }
    }
}
