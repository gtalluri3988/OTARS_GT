using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Reports {
    public class ConsolidateReportViewModel {
        public string IsBancaStaff { get; set; }
        public string IntermediaryTypeID { get; set; }
        
        public string Status { get; set; }
        public string ReportFor { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public bool IsMTA { get; set; }
        public int CompanyID { get; set; }
        public List<LookupItem> StatusCodes { get; set; }
    }
}
