using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Masters {
    
    public class TrainingViewModel {

        public long ID { get; set; }

        public long MemberID { get; set; }

        public long AgencyID { get; set; }

        public long? CourseID { get; set; }

        public long CompanyID { get; set; }

        public long IntermediaryTypeID { get; set; }

        public bool IsStructured { get; set; }

        public bool IsDialogue { get; set; }

        public string CourseDescription { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long? TrainingTypeID { get; set; }

        public float CreditHours { get; set; }

        public string OrganizedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public string BankName { get; set; }

        public long StatusID { get; set; }

        public string RecordVersion { get; set; }
    }// class

}// namespace
