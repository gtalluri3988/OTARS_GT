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
    public class CorporateNomineeViewModel {

        [Display(Name="Name")]
        public string Name { get; set; }

        [Display(Name = "Rank")]
        public LookupItem Level { get; set; }

        [Display(Name = "Banca")]
        public bool IsBancaStaff { get; set; }

        [Display(Name = "Citizen")]
        public bool IsCitizen { get; set; }

        [Display(Name = "Bumiputera")]
        public bool IsBumiputera { get; set; }

        [Display(Name = "New IC Number")]
        public string NewICNumber { get; set; }

        [Display(Name = "Old IC Number")]
        public string OldICNumber { get; set; }

        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }

        [Display(Name = "IC Type")]
        public LookupItem ICType { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

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

        [Display(Name="Takaful Basic Exam")]
        public LookupItem TbeCategory { get; set; }

        public bool IsPartTime { get; set; }

        [Dependency]
        public QualificationViewModel Qualification { get; set; }

        [Dependency]
        public SpouseViewModel Spouse { get; set; }

        /*[Display(Name = "Date Appointed")]
        public DateTime? JoinedOn { get; set; }*/

       
        [Dependency]
        [Binder(typeof(JsonModelBinder))]
        public List<MemberExperienceViewModel> Experiences { get; set; }

        public bool IsOld { get; set; }

        [Display(Name = "Remark")]
        public string Remarks { get; set; } //Use for administrative
      
    }
}
