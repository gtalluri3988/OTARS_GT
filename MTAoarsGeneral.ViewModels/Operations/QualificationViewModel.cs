using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class QualificationViewModel {

        [Display(Name="School Name")]
        public string SchoolName { get; set; }

        [Display(Name="Year")]
        public int? Year { get; set; }

        [Display(Name = "Educational Qualification")]
        public LookupItem EducationalQualification { get; set; }

        [Display(Name = "Insuarance Qualification")]
        public LookupItem InsuranceQualification{ get; set; }

    }
}
