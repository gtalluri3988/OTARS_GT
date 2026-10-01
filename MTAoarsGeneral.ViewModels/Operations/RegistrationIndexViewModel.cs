using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using System.ComponentModel.DataAnnotations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Administrative;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class RegistrationIndexViewModel : IMemberViewModel, IExistableNewIC, IExistableIntermediary, IExistableTbeDetails
    {
        public LookupItem AgencyType { get; set; }

        public string Name { get; set; }

        [Display(Name = "Business Registration Number")]
        public string BusinessRegistrationNumber { get; set; }

        [Display(Name = "IC Type")]
        public LookupItem ICType { get; set; }

        [Display(Name = "IC Number")]
        public string ICNumber { get; set; }

        [Display(Name = "Rank")]
        public LookupItem Level { get; set; }

        [Display(Name = "Family")]
        public bool IsFamily { get; set; }

        [Display(Name = "General")]
        public bool IsGeneral { get; set; }

        [Display(Name = "Takaful Entery Exam")]
        public LookupItem TbeCategory { get; set; }



        [Display(Name = "M2 Exam")]
        public LookupItem M2Exam { get; set; }

        [Display(Name = "Date of Exam (DD/MM/YYYY)")]
        public DateTime? DateOfExam { get; set; }
        [Display(Name = "Date 1st Join as Agent")]
        public int? NumberofYears { get; set; }

        public LookupItem JoinYear { get; set; }
        public LookupItem JoinMonth { get; set; }


        [Display(Name = "MTA Awards")]

        public List<LookupItem> MTAAwards { get; set; }
        [Display(Name = "Social Media Address ")]
        public string SocialMediaAddress { get; set; }

        [Display(Name = "Is Parttime")]
        public bool IsPartTime { get; set; }

        public bool IsBancaStaff { get; set; }

        public bool AllowConflict { get; set; }

        public long CompanyID { get; set; }

        public bool? AllowReferredMember { get; set; }

        public bool IsOld { get; set; }

        public bool IsIndividual
        {
            get
            {
                return Array.Exists<string>(new string[] { LookupConstants.AgencyType.Individual }, m => m == this.AgencyType.Code);
            }
        }

        //To display message
        public string ReferredMemberCategoriesDescription { get; set; }
        public string ReferredMemberDateCreated { get; set; }
        public List<ReferredResponseViewModel> ReferredMembers { get; set; }

        [Display(Name = "New Business Registration Number")]
        public string NewBusinessRegistrationNumber { get; set; }
    }

}
