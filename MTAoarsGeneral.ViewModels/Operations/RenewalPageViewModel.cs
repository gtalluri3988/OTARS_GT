using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class RenewalPageViewModel {
        [Dependency]
        public List<RenewalDetailViewModel> Data { get; set; }

        public int TotalRecords { get; set; }
    }
}
