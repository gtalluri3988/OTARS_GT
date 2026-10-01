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

namespace MTAoarsGeneral.Validators.Operations.Fluents
{

    public class AgencyViewModelValidator : AbstractValidator<AgencyViewModel>
    {

        public AgencyViewModelValidator(ILookupRepository lookupRepository, SpouseViewModelValidator spouseValidator)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50);
            RuleFor(x => x.BusinessRegistrationNumber)
                .NotNull()
                .Length(1, 15);
            When(x => x.ExcludeM2ExamValidation == false,
         () =>
         {
             RuleFor(x => x.M2Exam).NotNull();
             When(x => (x.M2Exam?.Code ?? "").ToUpper() != "EXEMPTED" && (x.M2Exam?.Code ?? "").ToUpper() != "NOTYETCOMPLETED",
        () =>
        {
            RuleFor(x => x.DateOfExam).NotNull();
        });
             RuleFor(x => x.DateOfExam).Must(
                                            (transaction, value) =>
                                            (transaction.DateOfExam == null && ((transaction.M2Exam?.Code ?? "").ToUpper() == "EXEMPTED" || (transaction.M2Exam?.Code ?? "").ToUpper() == "NOTYETCOMPLETED"))
                                           ).WithMessage("Do not enter a Date of Exam for M2 Exam with a status of Exempted or Not Yet Completed."); ;



         });
        }
    }// class

}// namespace
