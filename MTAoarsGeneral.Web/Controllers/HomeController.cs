using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Constants;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities;
using System.Security.Claims;
using MTAoarsGeneral.Utilities.Config;

namespace MTAoarsGeneral.Web.Controllers
{

    public class HomeController : BaseController
    {

        IHomeService homeService;
        IUserService userService;
        ConfigManager config;

        public HomeController(IHomeService homeService, IUserService userService, ConfigManager config)
        {
            this.homeService = homeService;
            this.userService = userService;
            this.config = config;
        }

        public ActionResult Authorize(string userName)
        {
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            var identity = userService.Get(userName);
            identity.UserLoginID = userService.SaveLoginDetail(identity.UserID, Request.UserHostAddress);
            dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Index()
        {
            var user = User as ClaimsPrincipal;
            var sub = user == null
                ? null
                : user.Claims.Where(c => c.Type.Equals("sub"))
                    .Select(c => c.Value)
                    .SingleOrDefault();

            // Finding 1: restore an ASP.NET session only after the OWIN cookie has passed
            // its idle and absolute-expiry validation. This keeps the existing SSO flow
            // working after a session recycle without allowing an expired cookie to revive it.
            if (!base.CurrentUser.IsLogin && user != null &&
                user.Identity.IsAuthenticated && !string.IsNullOrEmpty(sub))
            {
                var identity = userService.Get(sub);
                if (identity != null)
                {
                    var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
                    dataProvider.Remove(GlobalConstants.CurrentIdentity);
                    dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
                }
            }

            if (!base.CurrentUser.IsLogin)
            {
                return Redirect(config.SSOUrl);
            }

            if (user != null && user.Identity.IsAuthenticated)
            {
                if (sub == null)
                    return Redirect(config.SSOUrl);

            }

            var model = homeService.GetModel();
            return View(model);
        }

        public ActionResult Print()
        {
            return View();
        }

        public ActionResult Error(string responseCode)
        {
            logger.Info("Error Page...");
            TempData["ResponseCode"] = "";
            if (!string.IsNullOrEmpty(responseCode))
                TempData["ResponseCode"] = responseCode;
            if (base.CurrentUser.IsLogin)
                return View("Error", "~/Views/Shared/_Layout.cshtml", null);
            return View();
        }
    }// class
}
