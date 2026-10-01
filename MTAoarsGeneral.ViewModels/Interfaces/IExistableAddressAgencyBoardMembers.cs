using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;

namespace MTAoarsGeneral.ViewModels.Interfaces {
    public interface IExistableAddressAgencyBoardMembers : IExistableAgency, IExistableBoardMembers{

        AddressViewModel Address { get; set; }
    }
}
