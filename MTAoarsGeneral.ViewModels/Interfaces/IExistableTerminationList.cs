using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Notifications;

namespace MTAoarsGeneral.ViewModels.Interfaces {
    public interface IExistableTerminationList {

        List<TerminatedVariableSource> TerminationList { get; }

    }// interface
}// namespae
