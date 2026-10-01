using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MTAoarsGeneral.Utilities.Extensions {
    public static class ModelStateExtensions {

        public static dynamic GetErrors(this ModelStateDictionary modelState) {
            dynamic errors = (from error in modelState
                              where error.Value.Errors.Count > 0
                              select new { error.Key, Messages = (from mes in error.Value.Errors select mes.ErrorMessage).ToArray() }
                                 ).ToList();
            return errors;
        }

        public static string GetErrorsJson(this ModelStateDictionary modelState) {
            var js = new JavaScriptSerializer();
            return js.Serialize(new { Errors = modelState.GetErrors() });
        }

    }
}
