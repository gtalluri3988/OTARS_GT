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
using MTAoarsGeneral.Utilities.Extensions;
using System.Text.RegularExpressions;

namespace MTAoarsGeneral.Validators.Operations.Fluents
{
    [FluentRegisterable]
    public class RegistrationIndexViewModelValidator : AbstractValidator<RegistrationIndexViewModel>
    {
        public RegistrationIndexViewModelValidator(ILookupRepository lookupRepository)
        {
            RuleFor(x => x.ICType).NotNull().WithMessage("IC Type is required");
            RuleFor(x => x.TbeCategory).NotNull().WithMessage("Takaful Entery Exam is required"); ;
            RuleFor(x => x.M2Exam).NotNull().WithMessage("M2 Exam is required"); ;
            RuleFor(x => x.DateOfExam).NotNull().When(x => (x.M2Exam?.Code ?? "").ToUpper() != "EXEMPTED" && (x.M2Exam?.Code ?? "").ToUpper() != "NOTYETCOMPLETED").WithMessage("Date of Exam is required");
            RuleFor(x => x.Level).NotNull().When(x => x.IsFamily);
            RuleFor(x => x.BusinessRegistrationNumber).NotNull().When(x =>
                string.IsNullOrWhiteSpace(x.NewBusinessRegistrationNumber) && x.AgencyType != null & !x.IsIndividual
            );
            RuleFor(x => x.DateOfExam).Must(
                                             (transaction, value) =>
                                             (transaction.DateOfExam == null && ((transaction.M2Exam?.Code ?? "").ToUpper() == "EXEMPTED" || (transaction.M2Exam?.Code ?? "").ToUpper() == "NOTYETCOMPLETED"))
                                            ).WithMessage("Do not enter a Date of Exam for M2 Exam with a status of Exempted or Not Yet Completed."); ;
            RuleFor(x => x.ICNumber)
               .Must(ValidateNewIC)
               .WithMessage("New IC number should be 12 digits and begin with date of birth (yyMMdd) format")
               .When(x =>
                   x.ICType != null &&
                   x.ICType.Code == LookupConstants.ICTypes.NewIc);

            RuleFor(x => x.NewBusinessRegistrationNumber)
                .Must(ValidateNewBusinessRegistrationNumber)
                .WithMessage("New business registration number should be 12 digits and begin with (yyyy) format")
                .When(x => !string.IsNullOrWhiteSpace(x.NewBusinessRegistrationNumber));
        }

        bool ValidateNewIC(string icNumber)
        {
            return icNumber.IsNewIC();
        }

        bool ValidateNewBusinessRegistrationNumber(string newBusinessRegistration)
        {
            return newBusinessRegistration.IsNewBusinessRegistrationNumber();
        }

    }// class

}// namespace
