using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Operations;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Validators.Operations {
    public class ConflictValidator : IValidator<RegistrationConflictViewModel> {


        public IEnumerable<ValidationMessage> Validate(RegistrationConflictViewModel item) {
            if (item.SourceCompanyID <= 0) {
                yield return new ValidationMessage("", "Please select Source Company");
            }
           if(item.ConflictAttachments.Count < 2){
               yield return new ValidationMessage("", "Please attach Agent Movement Form and Termination Letter");
           }
        }

    }// class
}// namesapce
