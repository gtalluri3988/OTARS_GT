using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Mvc;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Interfaces {
    public interface IExistableBoardMembers {

      List<DirectorViewModel> Directors { get; set; }
      List<ShareholderViewModel> Shareholders { get; set; }
      List<AdditionalCorporateNomineeViewModel> AdditionalCorporateNominees { get; set; }
      IEnumerable<LookupItem> ICTypes { get; set; }

    }
}
