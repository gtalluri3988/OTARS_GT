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

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    
    [FluentRegisterable]
    public class QualificationViewModelValidator : AbstractValidator<QualificationViewModel> {

        public QualificationViewModelValidator(ILookupRepository lookupRepository, SpouseViewModelValidator spouseValidator) {
            RuleFor(x => x.EducationalQualification)
                .NotNull();
                
            RuleFor(x => x.SchoolName)
                //.NotNull()
                .Length(0, 100);
            RuleFor(x => x.Year)
                .NotNull();
                
                    
        }

    }// class

}// namespace
