    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class UpdateGuarantorVariableSource : INotificationVariableSource {

        public string AgencyName { get; set; }

        public string GuarantorTypeDescription { get; set; }

    }// class

}// namesapce
