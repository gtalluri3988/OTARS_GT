using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.IoC;

namespace MTAoarsGeneral.Utilities.Mvc {
    public class UnityContainerResolver : IDependencyResolver{
        
        public object GetService(Type serviceType) {
            if (!ObjectContainer.Container.IsRegistered(serviceType)) return null;
            return ObjectContainer.Container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return ObjectContainer.Container.ResolveAll(serviceType);
        }
        // namespace
    }// class
}// namespace
