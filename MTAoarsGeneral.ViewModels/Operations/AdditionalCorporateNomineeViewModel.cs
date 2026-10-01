using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Attributes;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class AdditionalCorporateNomineeViewModel : IMemberViewModel {

        public string Name { get; set; }

        [Display(Name = "Old IC Number")]
        public string OldICNumber { get; set; }

        public LookupItem ICType { get; set; }

        [Display(Name = "New IC Number")]
        public string ICNumber { get; set; }

        public LookupItem AgencyType { get; set; }

        [Display(Name = "Citizen")]
        public bool IsCitizen { get; set; }

        [Display(Name = "Bumiputera")]
        public bool IsBumiputera { get; set; }

        [Display(Name = "Gender")]
        public LookupItem Gender { get; set; }

        public LookupItem Race { get; set; }

        [Display(Name = "Religion")]
        public LookupItem Religion { get; set; }

        [Display(Name = "Marital Status")]
        public LookupItem MaritalStatus { get; set; }

        [Display(Name = "Contact Number")]
        public string Phone { get; set; }

        [Display(Name = "Handphone")]
        public string Mobile { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [Display(Name = "Family")]
        public bool IsFamily { get; set; }

        [Display(Name = "General")]
        public bool IsGeneral { get; set; }

    }
}
