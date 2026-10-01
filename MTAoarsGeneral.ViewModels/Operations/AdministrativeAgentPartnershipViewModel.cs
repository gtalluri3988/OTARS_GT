using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class AdministrativeAgentPartnershipViewModel : AdministrativeAgentViewModel, IExistableAgencyBanker, IExistableAgency
    {

        public AdministrativeAgentPartnershipViewModel()
        {
            this.Partners = new List<PartnerViewModel>();
        }

        [Binder(typeof(JsonModelBinder))]
        public List<PartnerViewModel> Partners { get; set; }

        [Dependency]
        public AgencyBankerViewModel AgencyBanker { get; set; }

    }// class
}
