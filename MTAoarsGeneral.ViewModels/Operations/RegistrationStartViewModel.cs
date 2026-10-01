using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Operations {
    
    public class RegistrationStartViewModel {

        [Display(Name="Company")]
        public LookupItem Company { get; set; }

        [Display(Name = "Registration Type")]
        public LookupItem AgencyType { get; set; }

    }// class

}// namespace
