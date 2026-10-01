using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Operations {
    
    public class InvoiceListViewModel {

        [Dependency]
        public List<InvoiceViewModel> Data { get; set; }

        public int TotalInvoice { get; set; }

    }// class


  

}// namespace
