using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using MTAoarsGeneral.ViewModels.Operations;
using System.Globalization;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    [FluentRegisterable]
    public class RegistrationStartViewModelValidator : AbstractValidator<RegistrationStartViewModel> {

        public RegistrationStartViewModelValidator(ILookupRepository lookupRepository) {
            RuleFor(x => x.Company).NotNull().WithMessage("Company is required");
            RuleFor(x => x.AgencyType).NotNull().WithMessage("Please select registration type");
        }

        

    }// class

}// namespace
