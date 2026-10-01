using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;

namespace MTAoarsGeneral.Utilities.Interfaces
{
   public interface IModelBinder
    {

        object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext);
        void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,PropertyDescriptor propertyDescriptor);
    }

    
}
