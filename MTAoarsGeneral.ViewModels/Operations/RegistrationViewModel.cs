using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Attributes;


namespace MTAoarsGeneral.ViewModels.Operations {
    public abstract class RegistrationViewModel : IExistableGuarantor, IExistableCompanyGuarantor, IExistableTbeDetails {

        public RegistrationViewModel() {
            AdditionalCorporateNominee = new AdditionalCorporateNomineeViewModel();
        }

        public LookupItem AgencyType { get; set; }

        public long CompanyID { get; set; }

        public AgencyViewModel Agency { get; set; }

        [Dependency]
        public AddressViewModel Address { get; set; }

        [Dependency]
        public CorporateNomineeViewModel CorporateNominee { get; set; }

        [Dependency]
        public AdditionalCorporateNomineeViewModel AdditionalCorporateNominee { get; set; }

        [Dependency]
        public GuarantorViewModel Guarantor { get; set; }

        public DateTime? DateAppointed { get; set; }

        public Actions CurrentAction { get; set; }

        
        public IEnumerable<LookupItem> ICTypes { get; set; }
        
        [IgnoreValidate]
        public string PhotoPath { get; set; }
        
        public string AgencyNumber { get; set; } //Use for administrative section
        public DateTime? DateTerminated { get; set; } //Use for administrative section
        public string AgencyPrincipalKey { get; set; } //Use for administrative section (Agency Principal ID)

        public string ICNumber
        {
            get
            {
                if (CorporateNominee == null) return null;
                if (CorporateNominee.ICType != null && CorporateNominee.ICType.Code == LookupConstants.ICTypes.NewIc)
                    return CorporateNominee.NewICNumber;
                return string.IsNullOrEmpty(CorporateNominee.PassportNumber) ? CorporateNominee.NewICNumber : CorporateNominee.PassportNumber;
            }
        }

        public bool IsFamily
        {
            get { return Agency != null && Agency.IsFamily; }
        }

        public bool IsGeneral
        {
            get { return Agency != null && Agency.IsGeneral; }
        }

        public LookupItem TbeCategory
        {
            get { return CorporateNominee == null ? null : CorporateNominee.TbeCategory; }
        }

        public bool IsOld
        {
            get { return CorporateNominee != null && CorporateNominee.IsOld; }
            set
            {
                if (CorporateNominee != null)
                    CorporateNominee.IsOld = value;
            }
        }

    }// class
}// namespace
