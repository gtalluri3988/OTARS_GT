using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using System.Web.Routing;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.Utilities.Mvc
{

    public class MTAoarsGeneralAuthorizeAttribute : AuthorizeAttribute
    {


        public MTAoarsGeneralAuthorizeAttribute()
        {

        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            if (!dataProvider.HasRegistered(GlobalConstants.CurrentIdentity) || !HasPermitted(filterContext))
            {
                var routeValues = new RouteValueDictionary(new { controller = "Account", action = "Unauthorized" });
                filterContext.Result = new RedirectToRouteResult(routeValues);
            }
        }

        bool HasPermitted(AuthorizationContext filterContext)
        {
            var url = String.Format("~/{0}/{1}", filterContext.RouteData.Values["controller"],
                filterContext.RouteData.Values["action"]);
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (!IsExists(identity.AllMenus, url)) return true;
            if (IsExists(identity.Menus, url)) return true;
            return false;
        }

        bool IsExists(List<MenuItem> menus, string url)
        {
            foreach (var menu in menus)
            {
                if (url.Equals(menu.Url, StringComparison.InvariantCultureIgnoreCase)) return true;
                if (menu.Menus.Count > 0)
                {
                    if (IsExists(menu.Menus, url)) return true;
                }
            }
            return false;
        }

    }// class

}// namespace
