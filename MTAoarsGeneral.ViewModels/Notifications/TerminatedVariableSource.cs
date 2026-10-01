using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class TerminatedVariableSource : INotificationVariableSource {

        public string AgencyNumber { get; set; }

        public string AgencyName { get; set; }

        public string IntermediaryTypeDescription { get; set; }
        
    }// class

}// namesapce
