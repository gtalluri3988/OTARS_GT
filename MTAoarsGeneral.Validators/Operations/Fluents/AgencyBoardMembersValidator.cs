using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using FluentValidation;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Validators.Shared.Fluent;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    public class AgencyBoardMembersValidator<T> : CorporateRegistrationViewModelValidator<T> 
        where T:RegistrationViewModel, IExistableBoardMembers
    {

        public AgencyBoardMembersValidator(AgencyViewModelValidator agencyValidator, 
            AddressViewModelValidator addressValidator) : base(agencyValidator, addressValidator) {
            RuleFor(x => x.Address)
               .SetValidator(addressValidator);
           RuleFor(x => x.Agency).SetValidator(agencyValidator);
           RuleFor(x => x.Directors.Count)
               .GreaterThan(0)
               .WithMessage("Atleast one director is required");
           RuleFor(x => x.Shareholders.Count)
               .GreaterThan(0)
               .WithMessage("Atleast one shareholder is required");
           RuleFor(x => x.Agency.AuthorizedCapital)
             .NotNull();
           RuleFor(x => x.Agency.PaidupCapital).NotNull();
           RuleFor(x => x.Agency.AuthorizedCapital)
               .GreaterThan(y => y.Agency.PaidupCapital.Value)
               .WithMessage("Authorized capital should be greater than paid up capital");


           RuleFor(x => x.Agency.PaidupCapital).GreaterThanOrEqualTo(50000).WithMessage("Paid up capital should not be less than 50000");
                
        }

    }// class
}// namespace
