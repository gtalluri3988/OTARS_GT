    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class ChangeNomineeVariableSource : INotificationVariableSource {

        public string NewICNumber { get; set; }

        public string Name { get; set; }
    }// class

}// namesapce
