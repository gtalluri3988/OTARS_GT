using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MTAoarsGeneral.ViewModels.Accounts;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Config;
using System.Security.Claims;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Web.Controllers
{
    public class AccountController : BaseController
    {

        IUserService userService;
        IScopeDataProvider dataProvider;
        IMenuService menuService;
        ConfigManager config;

        public AccountController(IUserService userService, IScopeDataProvider dataProvider, IMenuService menuService, ConfigManager config)
        {
            this.userService = userService;
            this.dataProvider = dataProvider;
            this.menuService = menuService;
            this.config = config;
        }
        // [Authorize]
        public ActionResult LogOn()
        {

            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            //if (identity == null) { return Redirect(config.SSOUrl); }
            //else { return RedirectToAction("Index", "Home"); }

            if (this.User.Identity.IsAuthenticated && identity == null)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var sub = claimsIdentity == null
                    ? null
                    : claimsIdentity.Claims.FirstOrDefault(i => i.Type == "sub");

                if (sub != null && !string.IsNullOrEmpty(sub.Value))
                {
                    identity = userService.Get(sub.Value);
                    if (identity != null)
                    {
                        dataProvider.Remove(GlobalConstants.CurrentIdentity);
                        dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
                    }
                }
            }

            if (identity != null)
                return RedirectToAction("Index", "Home");

            return Redirect(config.SSOUrl);
            // return View(new LogOnViewModel());
        }

        [HttpPost]
        public JsonResult LogOnSubmit(LogOnViewModel model)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);

            if (model.CaptchaKey == null || model.CaptchaKey.Equals(Session[GlobalConstants.CaptchaKey]) == false)
            {
                output.AddError("Security key does not match");
                output.Result = false;
                return Json(output);
            }

            if (!userService.Authenticate(model))
            {
                output.AddError("User name or password does not match");
                output.Result = false;
                return Json(output);
            }
            var identity = userService.Get(model.UserName);
            dataProvider.Remove(GlobalConstants.CurrentIdentity);
            dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
            FormsAuthentication.SetAuthCookie(model.UserName, false);
            output.RedirectUrl = Url.Action("Index", "Home");
            return Json(output);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            dataProvider.Remove(GlobalConstants.CurrentIdentity);

            if (identity != null)
                userService.SaveLogoutDetail(identity.UserLoginID);
            Session.Clear();
            Session.Abandon();
            //return View("LogOn", new LogOnViewModel());
            //return Redirect("/home.aspx");
            return Redirect(config.SSOUrl);
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult CaptchaImage()
        {
            var imageBuilder = new XCaptcha.ImageBuilder();
            var random = new XCaptcha.RandomTextGenerator();
            var result = imageBuilder.Create(random.Create(5, true));
            Session.Add(GlobalConstants.CaptchaKey, result.Solution);
            return new FileContentResult(result.Image, result.ContentType);

        }


    }// class
}// namespace
