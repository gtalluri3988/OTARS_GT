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
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.ViewModels.Maintenance;

namespace MTAoarsGeneral.Validators.Operations.Fluents
{
    [FluentRegisterable]
    public class TbeExemptionViewModelValidator : AbstractValidator<TbeExemptionViewModel>
    {

        public TbeExemptionViewModelValidator()
        {
            RuleFor(x => x.ICNumber)
              .NotNull()
              .Must(ValidateNewIC)
              .WithMessage("New IC number should be 12 digits and begin with date of birth (yyMMdd) format");

            RuleFor(x => x.Name)
                .NotNull();
            RuleFor(x => x.DateDocumentReceived)
                .NotNull();

        }

        bool ValidateNewIC(string icNumber)
        {
            return icNumber.IsNewIC();
        }

    }// class

}// namespace
