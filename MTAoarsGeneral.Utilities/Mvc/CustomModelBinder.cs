using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.Attributes;
using System.ComponentModel;


namespace MTAoarsGeneral.Utilities.Mvc {
    public class CustomModelBinder : DefaultModelBinder {
        /*public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            BinderAttribute binderAttribute = (BinderAttribute)bindingContext.ModelType.GetCustomAttributes(true).SingleOrDefault(t => t.GetType() == typeof(BinderAttribute));
            if (binderAttribute == null) {
                return base.BindModel(controllerContext, bindingContext);
            } else {
                return binderAttribute.GetBinder().BindModel(controllerContext, bindingContext);
            }

        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor) {

            BinderAttribute binderAttribute = (BinderAttribute)propertyDescriptor.Attributes[typeof(BinderAttribute)];
            if (binderAttribute == null) {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            } else {
                binderAttribute.GetBinder().SetProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }*/

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext,
           PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder) {
            var propertyType = propertyDescriptor.PropertyType;
            if (propertyType.IsEnum) {
                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (null != providerValue) {
                    var value = providerValue.RawValue;
                    if (null != value) {
                        var valueType = value.GetType();
                        if (!valueType.IsEnum) {
                            return Enum.ToObject(propertyType, value);
                        }
                    }
                }
            }
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }


    }// class
}// namespace
