using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports {
    public class CPDReportViewModel {

        public int Year { get; set; }

        public long? IntermediaryTypeID { get; set; }

        public string IsBancaStaff { get; set; }

        public string IsAchieved { get; set; }

        public long? CompanyID { get; set; }


    }
}
