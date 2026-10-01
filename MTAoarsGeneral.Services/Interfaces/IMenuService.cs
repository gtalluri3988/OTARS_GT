using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Accounts;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IMenuService {

        List<MenuItem> GetMenus();

        List<MenuItem> GetMenus(long roleId);

        void Save(long roleId, IEnumerable<long> menus);

        List<long> GetRoleMenus(long roleId);
    }
}
