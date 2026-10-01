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
using MTAoarsGeneral.ViewModels.Enquiries;

namespace MTAoarsGeneral.Validators.Enquiries.Fluents {
    [FluentRegisterable]
    public class SearchViewModelValidator : AbstractValidator<SearchViewModel> {

        public SearchViewModelValidator() {
           /* RuleFor(x => x.Company)
                .NotNull()
                .WithMessage("Please select Takaful Operator");

            RuleFor(x => x.ICNumber)
                .Must((svm, icnumber) => !(String.IsNullOrEmpty(svm.ICNumber) && String.IsNullOrEmpty(svm.RegistrationNumber)))
                .WithMessage("Please key in the required details");

            RuleFor(x => x.ICNumber)
                .Must((svm, icnumber) => !(!String.IsNullOrEmpty(svm.ICNumber) && !String.IsNullOrEmpty(svm.RegistrationNumber)))
                .WithMessage("Please choose 1 criteria only");*/

            
        }

    }// class

}// namespace
