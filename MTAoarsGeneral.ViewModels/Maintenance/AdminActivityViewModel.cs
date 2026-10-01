using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Maintenance {
    
    public class AdminActivityViewModel {

        public AdminActivityViewModel() {
            Details = new List<AdminActivityDetailViewModel>();
        }

        public LookupItem Activity { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime SearchFromDate { get; set; }

        public DateTime SearchToDate { get; set; }

        public LookupItem Company { get; set; }

        public string AgencyNumber { get; set; }

        public List<AdminActivityDetailViewModel> Details { get; set; }

        public List<AdminActivityDetailViewModel> GetDetails() {
            return Details;
        }
    }// class

}// namespace
