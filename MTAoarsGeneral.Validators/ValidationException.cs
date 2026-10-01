using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Validators {
    public class ValidationException : Exception {

        public ValidationException(IEnumerable<ValidationMessage> validationMessages) {
            this.ValidationMessages = validationMessages;
        }

        public IEnumerable<ValidationMessage> ValidationMessages {
            get;
            private set;
        }

    }
}
