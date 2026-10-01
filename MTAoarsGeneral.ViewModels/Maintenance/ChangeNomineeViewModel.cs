using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Maintenance {
    public class ChangeNomineeViewModel: IExistableTbeDetails {

        public long PrincipalID { get; set; }

        public RegistrationViewModel RegistrationModel { get; set; }

        public string ICNumber { 
            get { 
                var code = RegistrationModel.CorporateNominee.ICType.Code;
                if(code == LookupConstants.ICTypes.NewIc)return RegistrationModel.CorporateNominee.NewICNumber;
                return RegistrationModel.CorporateNominee.PassportNumber;
            }
        }

        public bool IsFamily => false;

        public bool IsGeneral => true;

        public LookupItem TbeCategory {
            get {
                return RegistrationModel.CorporateNominee.TbeCategory;
            }
        }
        public bool IsOld { get; set; }

    }// class
}// namespace
