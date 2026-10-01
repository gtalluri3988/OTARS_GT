using System.Collections.Generic;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations {
    public class TerminationSearchViewModel {

        [Dependency]
        public AgencySearchRequestViewModel AgencySearch { get; set; }
        
        [Binder(typeof(JsonModelBinder))]
        [Dependency]
        public List<TerminationSearchResponseViewModel> TerminationAgents { get; set; }

        public IEnumerable<LookupItem> TerminationActions { get; set; }
    }
}
