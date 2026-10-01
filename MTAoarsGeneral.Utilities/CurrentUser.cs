using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.Utilities
{
    public class CurrentUser
    {
        public CurrentUser()
        {
        }

        public Identity GetCurrentIdentity()
        {
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            var identity = dataProvider.Get<Identity>(MTAoarsGeneral.Utilities.Constants.GlobalConstants.CurrentIdentity);
            return identity;
        }

        public bool IsLogin
        {
            get
            {
                var currentIdentity = GetCurrentIdentity();
                return (currentIdentity != null);
            }
        }
    }
}
