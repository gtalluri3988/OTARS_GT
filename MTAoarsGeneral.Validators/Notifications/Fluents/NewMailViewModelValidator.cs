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
using MTAoarsGeneral.ViewModels.Notifications;

namespace MTAoarsGeneral.Validators.Notifications.Fluents {
    
    [FluentRegisterable]
    public class NewMailViewModelValidator : AbstractValidator<NewMailViewModel> {

        
        public NewMailViewModelValidator() {
            RuleFor(x => x.CompanyIdentifiers)
                .Must((src, list) => (list != null && list.Count > 0))
                .WithMessage("Please select company");
            RuleFor(x => x.Subject).NotNull();
            RuleFor(x => x.Body).NotNull();
        }

    }// class

}// namespace
