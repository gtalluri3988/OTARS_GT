using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class MaintenanceSearchViewModel
    {

        [Dependency]
        public AgencySearchRequestViewModel AgencySearch { get; set; }

        [Binder(typeof(JsonModelBinder))]
        [Dependency]
        public List<MaintenanceSearchResponseViewModel> Agents { get; set; }
    }
}
