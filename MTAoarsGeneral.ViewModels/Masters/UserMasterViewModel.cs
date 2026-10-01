using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class UserMasterViewModel {

        public long? CompanyID { get; set; }

        public long? RoleID { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string SecretQuestion { get; set; }

        public string SecretAnswer { get; set; }

        public byte[] RecordVersion { get; set; }
    }
}
