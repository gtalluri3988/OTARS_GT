using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Notifications {
    public class MailListViewModel {
        [Dependency]
        public List<MailViewModel> Data { get; set; }

        public int TotalMail { get; set; }

    }
}
