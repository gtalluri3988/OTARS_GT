using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Utilities.Mvc {
    [Serializable]
    public class Identity {

        public Identity() {
            AllMenus = new List<MenuItem>();
            Menus = new List<MenuItem>();
        }

        public long UserID { get; set; }

        public long UserLoginID { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public long CompanyID { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public IEnumerable<LookupItem> Roles { get; set; }

        public List<MenuItem> Menus { get; private set; }

        public List<MenuItem> AllMenus { get; private set; }

        public bool IsMTA {
            get { return CompanyCode == GlobalConstants.MTACompanyCode; }
        }

        public bool IsISM {
            get { return (CompanyCode == GlobalConstants.ISMCompanyCode || CompanyCode == GlobalConstants.ICICompanyCode); }
        }

        public bool IsISMOrMTA {
            get { return IsMTA || IsISM; }
        }

        public bool IsGeneral { get; set; }
        public bool IsFamily { get; set; }

        public bool IsAdmin
        {
            get; set;
        }
    }

   
}
