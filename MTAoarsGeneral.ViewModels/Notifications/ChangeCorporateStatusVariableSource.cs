    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {

    public class ChangeCorporateStatusVariableSource : INotificationVariableSource {

        public string AgencyName { get; set; }

        public string OldAgencyType { get; set; }

        public string NewAgencyType { get; set; }
    }// class

}// namesapce
