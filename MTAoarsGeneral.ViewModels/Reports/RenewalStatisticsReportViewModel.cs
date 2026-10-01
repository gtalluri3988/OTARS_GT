using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports {
    public class RenewalStatisticsReportViewModel {

        public string Caption { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public string IntermediaryTypeCode { get; set; }

        public string RegistrationCategory { get; set; }

    }
}
