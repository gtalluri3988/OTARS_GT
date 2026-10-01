using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Shared {
    
    public class AddressViewModel {

        [Display(Name="Address1")]
        public string Address1 { get; set; }

        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public LookupItem State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

    }// class

}// namespace
