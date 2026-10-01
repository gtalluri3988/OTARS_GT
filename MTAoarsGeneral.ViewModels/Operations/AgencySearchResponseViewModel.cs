using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.ViewModels.Operations
{

    public class AgencySearchResponseViewModel
    {

        public long AgencyID { get; set; }

        public long CompanyID { get; set; }

        public long AgencyPrincipalID { get; set; }

        public long NomineeID { get; set; }

        public string AgencyNumber { get; set; }

        public string NomineeName { get; set; }

        public string NomineeNewICNumber { get; set; }

        public string CompanyName { get; set; }

        public long IntermediaryTypeID { get; set; }

        public string IntermediaryTypeDescription { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateTime? TerminationDate { get; set; }

        public string ValidFromLabel
        {
            get
            {
                return DateToString(ValidFrom);
            }
        }
        public string ValidToLabel
        {
            get
            {
                return DateToString(ValidTo);
            }
        }

        public string TerminationDateLabel
        {
            get
            {
                return DateToString(TerminationDate);
            }
        }
        public bool IsNotToRelease { get; set; }
        public string Remarks { get; set; }
        public bool IsReferredMember { get; set; }
        protected string DateToString(DateTime? date)
        {
            if (date == null) return null;
            return date.Value.ToString(GlobalConstants.DateFormat);
        }
        public bool IsResigned { get; set; }

        public Boolean? IsBancaStaff { get; set; }

        public String AgencyTypeDescription { get; set; }

        public string Designation { get; set; }

        public string ReferredCategory { get; set; }
        public DateTime? ReferredCreatedDate { get; set; }
        public string ReferredCreatedDateToText { get; set; }

        public long MemberID { get; set; } //To pull correct Member at enquiry details

        public string NewBusinessRegistrationNumber { get; set; }

        public string M2Exam { get; set; }
        public DateTime? M2ExamDate { get; set; }

        public string M2ExamDateToLabel
        {
            get
            {
                return DateToString(M2ExamDate);
            }
        }

    }// class

}// namespace
