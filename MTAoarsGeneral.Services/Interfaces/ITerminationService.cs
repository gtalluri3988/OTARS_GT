using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface ITerminationService {

        void Process(IEnumerable<TerminationSearchResponseViewModel> list);
        void Reinstate(IEnumerable<TerminationSearchResponseViewModel> list);
        void Renew(IEnumerable<TerminationSearchResponseViewModel> list);
        void AddIntoRenewalDetail(List<TerminationSearchResponseViewModel> terminationAgents);
    }
}
