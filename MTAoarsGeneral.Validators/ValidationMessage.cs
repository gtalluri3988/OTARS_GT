using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Validators {
    
    public class ValidationMessage {

        public ValidationMessage(string errorKey, string errorMessage, params string[] args) {
            this.ErrorKey = errorKey;
            this.ErrorMessage = String.Format(errorMessage, args);
            
        }

        public string ErrorKey { get; set; }

        public string ErrorMessage { get; set; }

    }

}
