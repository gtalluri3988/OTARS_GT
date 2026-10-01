using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class CompanyViewModel
    {

         public string Code { get; set; }

         public string Name { get; set; }
                 
         public string Abbreviation { get; set; }
                 
         public string BusinessRegistrationNumber { get; set; }

        [Display(Name = "Is Active")]
         public bool IsActive { get; set; }

        [Display(Name = "Is Dropdown")]
        public bool IsDropDown { get; set; }

        public byte[] RecordVersion { get; set; }

        public int AddressID { get { return 1; } }

        [Display(Name = "Is General")]
        public bool IsGeneral { get; set; }

        [Display(Name = "Is Family")]
        public bool IsFamily { get; set; }
    }
}
