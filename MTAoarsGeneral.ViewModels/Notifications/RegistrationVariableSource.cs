    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class RegistrationVariableSource : INotificationVariableSource {

        public string AgencyName { get; set; }

        public string CorporateNomineeName { get; set; }

        public string CorporateNomineeNewICNumber { get; set; }

        public string CorporateNomineeOldICNumber { get; set; }

        public string CompanyName { get; set; }

        public string IntermediaryTypes { get; set; }

        public string AgencyNumbers { get; set; }
    }// class

}// namesapce
