using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Attributes;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class CBCProcessVariableSource : INotificationVariableSource, IExistableTerminationList {

        public CBCProcessVariableSource() {
            TerminationList = new List<TerminatedVariableSource>();
            SuspensionList = new List<TerminatedVariableSource>();
        }

        [ValueProvider(typeof(TerminationListValueProvider))]
        public List<TerminatedVariableSource> TerminationList { get; private set; }

        [ValueProvider(typeof(RenewedListValueProvider))]
        public List<TerminatedVariableSource> SuspensionList { get; private set; }
        
    }// class

}// namesapce
