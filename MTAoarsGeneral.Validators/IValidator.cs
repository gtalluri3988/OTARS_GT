using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Validators {
    public interface IValidator<in T> {

        IEnumerable<ValidationMessage> Validate(T item);

    }// interface
}// namespace
