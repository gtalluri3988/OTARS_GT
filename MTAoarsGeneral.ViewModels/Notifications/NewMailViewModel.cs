using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Notifications {
    public class NewMailViewModel {

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<long> CompanyIdentifiers { get; set; }

        public IEnumerable<LookupItem> Companies { get; set; }

    }// class
}// namespace
