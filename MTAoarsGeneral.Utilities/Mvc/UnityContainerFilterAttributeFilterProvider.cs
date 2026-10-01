using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.IoC;

namespace MTAoarsGeneral.Utilities.Mvc {

    public class UnityContainerFilterAttributeFilterProvider : FilterAttributeFilterProvider {

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor);
            BuildUp(attributes);
            return attributes;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor);
            BuildUp(attributes);
            return attributes;
        }

        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
            var filters = base.GetFilters(controllerContext, actionDescriptor);
            BuildUp(filters);
            return filters;
        }

        void BuildUp(IEnumerable<object> list) {
            foreach (var item in list) {
                ObjectContainer.Container.BuildUp(item);
            }
        }
       
    }// class

}// namespace
