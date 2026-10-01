using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Builders.Interfaces;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Validators.Operations.Fluents;

namespace MTAoarsGeneral.Builders.Operations
{
    public class PartnerShipRegistrationCreator: RegistrationCreator, IRegsitrationCreator
    {

        IUnityContainer container;
        public PartnerShipRegistrationCreator(ILookupRepository lookupRepository, IUnityContainer container)
            : base(lookupRepository,container)
        {
            this.container = container;
        }


       
        public override void Process(RegistrationViewModel model)
        {
            base.Process(model);
        }

        public RegistrationViewModel Create(List<string> parameters)
        {
            base.Parameters = parameters;
            var model = new PartnershipRegistrationViewModel();
            Process(model);            
            return model;
        }
     
    }
}

