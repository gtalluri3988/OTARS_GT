using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Repositories.Accounts {
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository {

        public MenuRepository(EntityContext context)
            : base(context) {
        }

        public List<MenuItem> GetMenus() {
            var output = new List<MenuItem>();
            var menus = Context.Menus.Where(p => p.ParentID == null);
            PopulateChildren(output, menus);
            return output;
        }

        public List<MenuItem> GetMenus(long roleId) {
            var output = new List<MenuItem>();
            var menus = Context.Menus.Where(p => p.ParentID == null && p.RoleMenus.Any(rm => rm.RoleID == roleId));
            PopulateChildren(output, menus, roleId);
            return output;
        }

        public List<long> GetRoleMenus(long roleId) {
            return Context.RoleMenus.Where(p => p.RoleID == roleId).Select(p => p.MenuID.Value).ToList();
        }

        public void Save(long roleId, IEnumerable<long> menus) {
            Context.RoleMenus.Where(p => p.RoleID == roleId).ToList().ForEach(p => Context.RoleMenus.DeleteObject(p));
            if (menus != null) {
                foreach (var menuId in menus) {
                    var rm = new RoleMenu { MenuID = menuId, RoleID = roleId, IsActive = true };
                    Context.RoleMenus.AddObject(rm);
                }
            }
            Context.SaveChanges();
        }

        void PopulateChildren(List<MenuItem> children, IEnumerable<Menu> menus, long? roleId = null) {
            foreach (var menu in menus) {
                var item = GetMenuItem(menu);
                children.Add(item);
                IEnumerable<Menu> childItems = menu.Children;
                if (roleId != null) childItems = childItems.Where(p => p.RoleMenus.Any(rm => rm.RoleID == roleId));
                childItems = childItems.ToList();
                if (childItems.Count() > 0) PopulateChildren(item.Menus, childItems);
            }
        }

        MenuItem GetMenuItem(Menu menu) {
            return new MenuItem {
                ID = menu.ID, Caption = menu.Caption, Description = menu.Description, Url = menu.Url
            };
        }

    }// class
}// namespace
