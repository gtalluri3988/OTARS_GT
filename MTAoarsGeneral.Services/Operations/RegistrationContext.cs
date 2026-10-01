using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Services.Operations {

    class RegistrationContext {

        public RegistrationViewModel Model { get; set; }

        public Agency Agency { get; set; }
    }

}
