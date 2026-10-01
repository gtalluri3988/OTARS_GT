using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class SpouseViewModel  {

        public string Name { get; set; }

        [Display(Name = "Citizen")]
        public bool IsCitizen { get; set; }

        [Display(Name = "Bumiputera")]
        public bool IsBumiputera { get; set; }

        [Display(Name = "New IC Number")]
        public string NewICNumber { get; set; }

        [Display(Name = "Old IC Number")]
        public string OldICNumber { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        public LookupItem Gender { get; set; }

        public LookupItem Race { get; set; }

        public LookupItem Religion { get; set; }

        [Display(Name = "Marital Status")]
        public LookupItem MaritalStatus { get; set; }

        
    }
}
