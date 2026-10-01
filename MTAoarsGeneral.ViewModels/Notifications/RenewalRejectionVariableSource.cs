using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class RenewalRejectionVariableSource : INotificationVariableSource {

        public int Total { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

    }// class

}// namesapce
