using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Validators.Shared;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.Web.Controllers
{
    public class BaseController : Controller
    {
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected JsonResponseModel GetResponse()
        {
            var response = new JsonResponseModel();
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    if (!String.IsNullOrWhiteSpace(error.ErrorMessage))
                        response.AddError(key, error.ErrorMessage);
                }
            }
            response.Result = ModelState.IsValid;
            return response;
        }

        protected JsonResponseModel GetResponse(IContext context)
        {
            var response = new JsonResponseModel();
            foreach (var msg in context.ValidationMessages)
            {
                response.AddError(msg.ErrorKey, msg.ErrorMessage);
            }
            response.Result = context.IsSuccess;
            return response;
        }

        private MTAoarsGeneral.Utilities.CurrentUser currentUser { get; set; }
        public MTAoarsGeneral.Utilities.CurrentUser CurrentUser
        {
            get
            {
                if (currentUser == null)
                    currentUser = new MTAoarsGeneral.Utilities.CurrentUser();
                return currentUser;
            }
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    //filterContext.ExceptionHandled = true;

        //    //filterContext.HttpContext.Response.Redirect("/error.aspx", true);
        //    //filterContext.HttpContext.Response.End();

        //    if (IsLogin)
        //        filterContext.HttpContext.Response.Redirect("~/", true);
        //    filterContext.HttpContext.Response.Redirect("/error.html", true);

        //    //filterContext.Result = new ViewResult
        //    //{
        //    //    ViewName = "~/Shared/Error.cshtml"
        //    //};            
        //}
    }// class
}// namespace
