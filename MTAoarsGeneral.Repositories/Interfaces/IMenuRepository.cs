using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Repositories.Interfaces {
    public interface IMenuRepository : IRepository<Menu> {

        List<MenuItem> GetMenus();

        List<MenuItem> GetMenus(long roleId);

        List<long> GetRoleMenus(long roleId);

        void Save(long roleId, IEnumerable<long> menus);
    }
}
