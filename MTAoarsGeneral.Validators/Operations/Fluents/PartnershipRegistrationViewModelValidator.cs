using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using FluentValidation;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Validators.Shared.Fluent;

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    [FluentRegisterable()]
    public class PartnershipRegistrationViewModelValidator : CorporateRegistrationViewModelValidator<PartnershipRegistrationViewModel> {

        public PartnershipRegistrationViewModelValidator(AgencyViewModelValidator agencyValidator,
            AddressViewModelValidator addressValidator) : base(agencyValidator, addressValidator){
            RuleFor(x =>  x.Partners)
                .Must(list => (list != null && list.Count > 0))
                .WithMessage("Atleast one partner is required");
        }

    }// class
}// namespace
