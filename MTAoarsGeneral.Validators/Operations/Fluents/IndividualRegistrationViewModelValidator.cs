using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using FluentValidation;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Validators.Shared.Fluent;

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    [FluentRegisterable()]
    public class IndividualRegistrationViewModelValidator : AbstractValidator<IndividualRegistrationViewModel> {

        public IndividualRegistrationViewModelValidator(AddressViewModelValidator addressValidator) {
            RuleFor(x => x.Address)
                .SetValidator(addressValidator);
            RuleFor(x => x.CorporateNominee.Phone)
                .NotNull();
        }

    }// class
}// namespace
