using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Interfaces {
    public interface IExistableNewIC {

        LookupItem ICType { get; set; }

        string ICNumber { get; set; }

    }
}
