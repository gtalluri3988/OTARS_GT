using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Validators.Shared {
    public interface IContext {

        List<ValidationMessage> ValidationMessages { get; }

        bool IsSuccess { get; }
            
    }
}
