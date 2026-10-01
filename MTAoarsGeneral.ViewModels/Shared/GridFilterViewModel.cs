using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Shared {
  
    public class GridFilterViewModel {
        public string Field { get; set; }

        public string Operator { get; set; }

        public string Value { get; set; }

        public List<GridFilterViewModel> Filters { get; set; }

        public string Logic { get; set; }
    }

}
