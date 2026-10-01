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
    public class UserViewModelValidator : AbstractValidator<UserMasterViewModel> {

        public UserViewModelValidator() {
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50);
            RuleFor(x => x.UserName)
                .NotNull()
                .Length(1, 100);
          
            
        }

    }// class

}// namespace
