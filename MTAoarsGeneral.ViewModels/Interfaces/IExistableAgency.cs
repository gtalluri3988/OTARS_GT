using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Interfaces {
    public interface IExistableAgency {

        AgencyViewModel Agency { get; set; }

        LookupItem AgencyType { get; }

    }
}
