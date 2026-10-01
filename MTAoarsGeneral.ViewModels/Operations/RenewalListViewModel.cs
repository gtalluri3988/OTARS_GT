using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class RenewalListViewModel {

        public long ID { get; set; }

        public long CompanyID { set; get; }
        
        public string CompanyName { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public long StatusID { get; set; }

        public string StatusDescription { get; set; }

        public bool IsMailSent { get; set; }

        public bool IsReminderSent { get; set; }
    }// class
}// namespace
