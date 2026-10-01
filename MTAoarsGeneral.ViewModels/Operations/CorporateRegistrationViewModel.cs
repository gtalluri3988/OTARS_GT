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

namespace MTAoarsGeneral.ViewModels.Operations {

    public class CorporateRegistrationViewModel : RegistrationViewModel, IExistableAgencyBanker, IExistableBoardMembers, IExistableAgency, IExistableAddressAgencyBoardMembers {


        public CorporateRegistrationViewModel() {
            Directors = new MemberList<DirectorViewModel>(this);
            Shareholders = new MemberList<ShareholderViewModel>(this);
            AdditionalCorporateNominees = new MemberList<AdditionalCorporateNomineeViewModel>(this);
        }
        
        [Binder(typeof(JsonModelBinder))]
        public List<DirectorViewModel> Directors { get; set; }

        
        [Binder(typeof(JsonModelBinder))]
        public List<ShareholderViewModel> Shareholders { get; set; }

        public List<AdditionalCorporateNomineeViewModel> AdditionalCorporateNominees { get; set; }

        [Dependency]
        public AgencyBankerViewModel AgencyBanker { get; set; }
              
    }// class

}// namespace
