using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using FluentValidation;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Masters;

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    [FluentRegisterable]
    public class CBCStartViewModelValidator : AbstractValidator<CBCStartViewModel> {

        public CBCStartViewModelValidator() {
            RuleFor(x => x.Year)
                .NotNull().WithMessage("Please select Year");

            RuleFor(x => x.Quarter)
                .NotNull().WithMessage("Please select Quarter");

        }

    }// class

}// namespace
