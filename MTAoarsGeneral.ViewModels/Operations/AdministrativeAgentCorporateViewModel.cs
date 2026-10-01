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

    public class AdministrativeAgentCorporateViewModel : AdministrativeAgentViewModel, IExistableAgencyBanker, IExistableBoardMembers, IExistableAgency, IExistableAddressAgencyBoardMembers
    {
        public AdministrativeAgentCorporateViewModel()
        {
            Directors = new List<DirectorViewModel>();
            Shareholders = new List<ShareholderViewModel>();
            AdditionalCorporateNominees = new List<AdditionalCorporateNomineeViewModel>();
        }

        [Binder(typeof(JsonModelBinder))]
        public List<DirectorViewModel> Directors { get; set; }


        [Binder(typeof(JsonModelBinder))]
        public List<ShareholderViewModel> Shareholders { get; set; }

        public List<AdditionalCorporateNomineeViewModel> AdditionalCorporateNominees { get; set; }

        [Dependency]
        public AgencyBankerViewModel AgencyBanker { get; set; }

    }// class
}
