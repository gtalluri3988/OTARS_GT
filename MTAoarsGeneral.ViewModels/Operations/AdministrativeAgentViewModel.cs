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


namespace MTAoarsGeneral.ViewModels.Operations
{
    public abstract class AdministrativeAgentViewModel : IExistableGuarantor, IExistableCompanyGuarantor
    {

        public AdministrativeAgentViewModel()
        {
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
        public bool IsTerminated { get; set; } //Use for administrative section
    }// class
}
