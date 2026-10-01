using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Validators {
    public interface IValidationProvider {
        IEnumerable<IValidator<T>> GetValidators<T>(T item);
    }
}
