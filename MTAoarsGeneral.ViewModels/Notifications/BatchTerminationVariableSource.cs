using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Attributes;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class BatchTerminationVariableSource : INotificationVariableSource, IExistableTerminationList {

        public BatchTerminationVariableSource() {
            TerminationList = new List<TerminatedVariableSource>();
        }

        [ValueProvider(typeof(TerminationListValueProvider))]
        public List<TerminatedVariableSource> TerminationList { get; private set; }
        
    }// class

}// namesapce
