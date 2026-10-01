using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MTAoarsGeneral.Utilities.Mvc {

    public class DecimalModelBinder : DefaultModelBinder {

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null) return base.BindModel(controllerContext, bindingContext);
            if (String.IsNullOrWhiteSpace(valueProviderResult.AttemptedValue)) return null;
            return Convert.ToDecimal(valueProviderResult.AttemptedValue);
        }

    }// class

}// namespace
