using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Extensions;


namespace MTAoarsGeneral.Utilities.Mvc {
    public class JsonModelBinder : IModelBinder {
        public object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext) {
            throw new NotImplementedException();
        }

        public void SetProperty(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor) {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var name = bindingContext.ModelName.GetModelProperty(propertyDescriptor.Name);
            try {
                if (bindingContext.ValueProvider.GetValue(name) == null) return;
                propertyDescriptor.SetValue(bindingContext.Model, js.Deserialize(bindingContext.ValueProvider.GetValue(name).AttemptedValue, propertyDescriptor.PropertyType));
            } catch (Exception) {
                bindingContext.ModelState.AddModelError("",
                    String.Format("Error or unaccepted value on {0}", bindingContext.ValueProvider.GetValue(name)));
            }
        }
    }
}
