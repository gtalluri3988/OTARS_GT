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
using MTAoarsGeneral.Validators.Shared.Fluent;


namespace MTAoarsGeneral.Validators.Operations.Fluents {
    [FluentRegisterable]
    public class SoleProprietorshipRegistrationViewModelValidator : CorporateRegistrationViewModelValidator<SoleProprietorshipRegistrationViewModel> {

        public SoleProprietorshipRegistrationViewModelValidator(AgencyViewModelValidator agencyValidator, AddressViewModelValidator addressValidator):base(agencyValidator, addressValidator) {
           
        }

    }// class

}// namespace
