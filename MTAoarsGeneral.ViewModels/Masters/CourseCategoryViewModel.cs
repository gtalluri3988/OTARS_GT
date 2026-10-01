using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class CourseCategoryViewModel
    {

         public string Code { get; set; }

         public string Description { get; set; }
             
 
        [Display(Name = "Is Technical")]
         public bool IsTechnical { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }


        public byte[] RecordVersion { get; set; }
    }
}
