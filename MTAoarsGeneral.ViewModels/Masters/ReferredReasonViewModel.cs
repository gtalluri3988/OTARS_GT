using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;
namespace MTAoarsGeneral.ViewModels.Masters {
  public   class ReferredReasonViewModel {

        
        public string Code { get; set; }
        
        public string Description { get; set; }
        [Display(Name="Allow for Registration")]
        public bool AllowForRegistration { get; set; }
        [Display(Name="Active")]
        public bool IsActive { get; set; }
        public byte[] RecordVersion { get; set; }
        [Display(Name = "Referred Category")]
        public long? ReferredCategoryID { get; set; }
    }
}
