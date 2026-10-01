using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class ActivityViewModel
    {

         public string Code { get; set; }

        public string Description { get; set; }

        [Display(Name = "MTA Charges")]
        public string MTACharge { get; set; }

        [Display(Name = "ISM Charges")]
        public string ISMCharge { get; set; }

        [Display(Name = "Is Admin Activity")]
        public bool IsAdminActivity { get; set; }

        public byte[] RecordVersion { get; set; }
    }
}
