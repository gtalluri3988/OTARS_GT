using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Validators.Shared;

namespace MTAoarsGeneral.Services.Shared {
    public class BuilderContext : IContext {


        public BuilderContext() {
            ValidationMessages = new List<ValidationMessage>();
        }

        public List<ValidationMessage> ValidationMessages {
            get;
            private set;
        }

        public bool IsSuccess {
            get {
                return ValidationMessages.Count == 0;
            }
        }

    }// class
}// namespace
