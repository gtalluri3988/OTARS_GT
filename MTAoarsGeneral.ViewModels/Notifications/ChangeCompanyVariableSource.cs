    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class ChangeCompanyVariableSource : INotificationVariableSource {

        public string AgencyName { get; set; }

        public string OldCompanyName { get; set; }

        public string NewCompanyName { get; set; }

    }// class

}// namesapce
