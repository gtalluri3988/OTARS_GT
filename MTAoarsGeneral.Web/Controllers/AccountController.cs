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
using Microsoft.Owin;
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

            // Finding 1: this only restores the server session after OWIN has accepted
            // the cookie, which is limited by the 15-minute idle and 60-minute absolute
            // expiry checks configured in Startup. It preserves the established SSO flow
            // when ASP.NET session state is recycled.
            //if (this.User.Identity.IsAuthenticated && identity == null)
            //{
            //    var claimsIdentity = User.Identity as ClaimsIdentity;
            //    var sub = claimsIdentity == null
            //        ? null
            //        : claimsIdentity.Claims.FirstOrDefault(i => i.Type == "sub");

            //    if (sub != null && !string.IsNullOrEmpty(sub.Value))
            //    {
            //        identity = userService.Get(sub.Value);
            //        if (identity != null)
            //        {
            //            dataProvider.Remove(GlobalConstants.CurrentIdentity);
            //            dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
            //        }
            //    }
            //}
            if (this.User.Identity.IsAuthenticated && identity == null)
            {
                var sub = (User.Identity as ClaimsIdentity).Claims.FirstOrDefault(i => i.Type == "sub").Value;
                identity = userService.Get(sub);
                dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
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
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            // Finding 5: invalidate every authentication/session layer before redirecting away from the app.
            HttpContext.GetOwinContext().Authentication.SignOut("Cookies");
            FormsAuthentication.SignOut();
            dataProvider.Remove(GlobalConstants.CurrentIdentity);

            dataProvider.Register<Identity>(GlobalConstants.CurrentIdentity, null);


            if (identity != null)
                userService.SaveLogoutDetail(identity.UserLoginID);
            Session.Clear();
            Session.Abandon();
            // Finding 5: expire the browser's session identifier so it cannot be replayed after logout.
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty)
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = Request.IsSecureConnection
            });
            //return View("LogOn", new LogOnViewModel());
            //return Redirect("/home.aspx");
            return Redirect(config.SSOUrl);
        }

        public ActionResult Unauthorized()
        {
            // No logged-in user means the session has expired (or never existed):
            // clear every session layer and send the user back to SSO to re-authenticate.
            if (!dataProvider.HasRegistered(GlobalConstants.CurrentIdentity))
            {
                SessionTimeoutManager.SignOut(System.Web.HttpContext.Current);
                return Redirect(config.SSOUrl);
            }
            return View();
        }

        // Called by Scripts/SessionTimeout.js when the idle or absolute timeout is reached in the browser.
        public ActionResult SessionExpired()
        {
            SessionTimeoutManager.SignOut(System.Web.HttpContext.Current);
            return Redirect(config.SSOUrl);
        }

        // Called by Scripts/SessionTimeout.js when the user chooses to continue working;
        // the request itself refreshes the server-side idle timer.
        public JsonResult KeepAlive()
        {
            return Json(new { Result = dataProvider.HasRegistered(GlobalConstants.CurrentIdentity) }, JsonRequestBehavior.AllowGet);
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
