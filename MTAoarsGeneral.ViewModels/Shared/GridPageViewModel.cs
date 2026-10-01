using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Shared {
    public class GridPageViewModel {

        public int Take { get; set; }

        public int Skip { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public List<GridSortViewModel> Sort { get; set; }

        public GridFilterViewModel Filter { get; set; }
    }

     
}
