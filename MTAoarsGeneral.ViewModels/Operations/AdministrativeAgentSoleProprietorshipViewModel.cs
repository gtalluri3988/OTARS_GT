using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Interfaces;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Operations
{

    public class AdministrativeAgentSoleProprietorshipViewModel : AdministrativeAgentViewModel, IExistableAgencyBanker, IExistableAgency
    {
        [Dependency]
        public AgencyBankerViewModel AgencyBanker { get; set; }

    }// class

}// namespace
