using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Accounts;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IUserService {

        bool Authenticate(LogOnViewModel model);

        Identity Get(long userId);

        Identity Get(string userName);

        long SaveLoginDetail(long userId, string hostIP);

        void SaveLogoutDetail(long userLoginId);
    }
}
