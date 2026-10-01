using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Reports
{
    public class TBEExaminationViewModel
    {
        public string Search { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
