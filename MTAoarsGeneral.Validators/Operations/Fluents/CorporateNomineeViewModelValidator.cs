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
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Validators.Operations.Fluents
{

    [FluentRegisterable]
    public class CorporateNomineeViewModelValidator : AbstractValidator<CorporateNomineeViewModel>
    {

        ILookupRepository lookupRepository;
        public CorporateNomineeViewModelValidator(ILookupRepository lookupRepository, SpouseViewModelValidator spouseValidator)
        {
            this.lookupRepository = lookupRepository;
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 75);
            RuleFor(x => x.OldICNumber)
                .Length(0, 25);
            RuleFor(x => x.BirthDate).NotNull();
            RuleFor(x => x.Race).NotNull();
            RuleFor(x => x.Religion).NotNull();
            RuleFor(x => x.MaritalStatus).NotNull();
            RuleFor(x => x.Gender).NotNull();
            /*RuleFor(x => x.Spouse)
               .SetValidator(spouseValidator)
               .When(IsMarried);*/
            RuleFor(x => x.Mobile)
                .Length(0, 20);
            //RuleFor(x => x.Email)
            //    .Length(0, 100)
            //     .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            //    .WithMessage("Invalid email address");
            //.EmailAddress();
            RuleFor(x => x.Fax)
                .Length(0, 20);
            RuleFor(x => x.Phone)
                .Length(0, 20);
            RuleFor(x => x.ICType).NotNull().WithMessage("IC Type is required");
            RuleFor(x => x.TbeCategory).NotNull().When(IsOld);
            RuleFor(x => x.NewICNumber)
               .Must(ValidateNewIC)
               .WithMessage("New IC number should be 12 digits and begin with date of birth (yyMMdd) format")
               .When(x =>
                   x.ICType != null &&
                   x.ICType.Code == LookupConstants.ICTypes.NewIc);
        }

        bool IsMarried(CorporateNomineeViewModel source)
        {
            if (source.MaritalStatus == null) return false;
            return source.MaritalStatus.Code == LookupConstants.MaritalStatus.Married;
        }
        bool ValidateNewIC(string icNumber)
        {
            return icNumber.IsNewIC();
        }
        bool IsOld(CorporateNomineeViewModel source)
        {
            if (source.IsOld)
                return false;
            return true;
        }
    }// class

}// namespace
