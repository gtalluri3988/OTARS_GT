using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Interfaces;
using System.Web;

namespace MTAoarsGeneral.Utilities.Mvc {
    public class SessionScopeDataProvider : IScopeDataProvider {

        public void Remove(string key) {
            if (HasRegistered(key)) { HttpContext.Current.Session.Remove(key); }
        }

        public void Register<T>(string key, T item) where T:class {
            HttpContext.Current.Session.Add(key, item);
        }

        public T Get<T>(string key) where T:class{
            var value = HttpContext.Current.Session[key];
            return (T)(value);
        }

        public bool HasRegistered(string key) {
            object o = Get<object>(key);
            return o != null;
        }

    }// class
}// namespace
