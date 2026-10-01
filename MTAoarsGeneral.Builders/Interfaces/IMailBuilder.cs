using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Notifications;

namespace MTAoarsGeneral.Builders.Interfaces {
    public interface IMailBuilder {

        NewMailViewModel GetNewMail();

    }
}
