using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class InvoiceVariableSource : INotificationVariableSource {

        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Amount { get; set; }
        
    }// class

}// namesapce
