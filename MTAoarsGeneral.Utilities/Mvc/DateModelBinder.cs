using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;
using System.Web.Script.Serialization;

namespace MTAoarsGeneral.Utilities.Mvc {
    
    public class DateModelBinder : DefaultModelBinder{

        private class DateValue {
            public DateTime Value { get; set; }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            DateTime date;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;
            if(DateTime.TryParseExact(
                value,
                "ddd MMM d HH:mm:ss UTCzzzzz yyyy",
                null,
                DateTimeStyles.None,
                out date)) {
                    return date;
            }

            if (DateTime.TryParseExact(value,
                "dd/MM/yyyy",
                null,
                DateTimeStyles.None, out date)) {
                    return date;
            }

            if (value != null && value.Contains(")")) {
                var input = String.Format("{{Value:\"\\/{0}\\/\"}}", value.Replace("/", ""));
                return new JavaScriptSerializer().Deserialize<DateValue>(input).Value.ToLocalTime();
               
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }// class

}// namespace

