using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.Utilities.Mvc {

    public static class HtmlHelperObjectExtensions {

        public static Identity GetIdentity(this HtmlHelper helper) {
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            return identity;
        }

        public static bool IsMTA(this HtmlHelper helper) {
            return helper.GetIdentity().IsMTA;
        }

        public static bool IsISM(this HtmlHelper helper) {
            return helper.GetIdentity().IsISM;
        }

        public static bool IsISMOrMTA(this HtmlHelper helper) {
            return helper.GetIdentity().IsISMOrMTA;
        }
    }// class

}// namespace
