using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MTAoarsGeneral.Utilities.Mvc {

    public class DelegateErrorAttribute : HandleErrorAttribute {

        Dictionary<Type, Action<ExceptionContext>> delegates;

        public DelegateErrorAttribute() {
            delegates = new Dictionary<Type, Action<ExceptionContext>>();
        }

        public void Register(Type exceptionType, Action<ExceptionContext> action) {
            if (delegates.ContainsKey(exceptionType)) {
                delegates[exceptionType] += action;
            } else {
                delegates.Add(exceptionType, action);
            }
        }
        
        public override void OnException(ExceptionContext filterContext) {
            var exceptionType = filterContext.Exception.GetType();
            if (delegates.ContainsKey(exceptionType)) {
                delegates[exceptionType](filterContext);
            } else {
                base.OnException(filterContext);
            }
        }

    }// class

}// namespace
