using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Script.Serialization;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Utilities.Mvc {

    public class AjaxResponse : JsonResult {
        private ModelStateDictionary modelState;
        private string message;
        private string redirect;
        private bool isRedirect;
        private string redirectKey;
        public AjaxResponse(ModelStateDictionary modelState, string message) {
            this.modelState = modelState;
            this.message = message;
        }
        public AjaxResponse(ModelStateDictionary modelState, string message, string redirect, string redirectKey) {
            this.modelState = modelState;
            this.message = message;
            this.redirect = redirect;
            this.redirectKey = redirectKey;
            if (!string.IsNullOrEmpty(redirect)) {
                isRedirect = true;
            }
        }

        //Generate Error process;

        private dynamic GetErrors() {
            
            if (modelState != null) {
                return modelState.GetErrors();
            } else {
                throw new ArgumentNullException("Invalid Model state");
            }
        }
        public override void ExecuteResult(ControllerContext context) {

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            JavaScriptSerializer js = new JavaScriptSerializer();
            response.Write(js.Serialize(new { Errors = GetErrors(), IsRedirect = isRedirect, Redirect = redirect, RedirectKey = redirectKey, Message = message }));
        }

    }

}
