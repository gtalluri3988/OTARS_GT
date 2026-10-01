using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.ViewModels.Shared;
using Microsoft.Practices.Unity;


namespace MTAoarsGeneral.ViewModels.Operations {
    public class ALCHeaderViewModel {

        public ALCHeaderViewModel() {
            Address = new ALCAddressViewModel();
            AgencyManager = new ALCMemberViewModel();
            AgencyManager.Designation = "Manager";
            Directors = new List<ALCMemberViewModel>();
            Shareholders = new List<ALCMemberViewModel>();
            IntermediaryType = "Family";
        }

        public long ID { get; set; }

        public long? AddressID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name of ALC")]
        public string Name { get; set; }

        public string RegistrationNumber { get; set; }

        [Dependency]
        public ALCAddressViewModel Address { get; set; }

        public string BusinessRegistrationNumber { get; set; }

        [Required(ErrorMessage="Intermediary Type is required")]
        public string IntermediaryType { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Takaful Operator")]
        public LookupItem Company { get; set; }

        [Display(Name = "Paidup Capital")]
        public decimal? PaidupCapital { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Valid From is required")]
        [Display(Name = "Valid From")]
        public DateTime? ValidFrom { get; set; }

        [Required(ErrorMessage = "Valid To is required")]
        [Display(Name = "Valid To")]
        public DateTime? ValidTo { get; set; }

        [Display(Name = "Date Terminated")]
        public DateTime? DateTerminated { get; set; }

        [Display(Name = "Date Appointed")]
        public DateTime? DateAppointed { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Comments { get; set; }

        public DateTime? CreatedDate { get; set; }

        public ALCMemberViewModel AgencyManager { get; set; }

        public List<ALCMemberViewModel> Directors { get; set; }

        public List<ALCMemberViewModel> Shareholders { get; set; }

    }

    public class ALCMemberViewModel {

        public long ID { get; set; }

        public long HeaderID { get; set; }

        [Required(ErrorMessage = "Member name is required")]
        public string Name { get; set; }

        [Display(Name = "MTA Agency Registration Number")]
        public string RegistrationNumber { get; set; }

        public string Designation { get; set; }

        [Required(ErrorMessage = "Member IC is required")]
        [Display(Name = "IC Number")]
        public string ICNumber { get; set; }
    }

    public class ALCAddressViewModel {

        
        public long ID { get; set; }

        [Display(Name = "Address1")]
        public string Address1 { get; set; }

        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public LookupItem State { get; set; }

        public long? StateID { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

    }// class
}
