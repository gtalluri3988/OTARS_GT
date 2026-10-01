using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Attributes;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class RenewalHeaderViewModel {

        public RenewalHeaderViewModel() {
            Details = new List<RenewalDetailViewModel>();
        }

        public long ID { get; set; }

        public long CompanyID { get; set; }
        
        public string CompanyName { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public long StatusID { get; set; }

        public string StatusDescription { get; set; }

        public bool IsMailSent { get; set; }

        public bool IsReminderSent { get; set; }

        [Binder(typeof(JsonModelBinder))]
        public List<RenewalDetailViewModel> Details { get;  set; }

        public IEnumerable<LookupItem> RenewalDetailStatuses { get; set; }
    }// class
}// namespace
