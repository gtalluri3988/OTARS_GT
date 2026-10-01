using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Schedulers {
   public class SchedulerDataProvider : IScopeDataProvider {
        static Dictionary<string, object> list;
        static SchedulerDataProvider() {
            list = new Dictionary<string, object>();
        }

        public void Remove(string key) {
            if (HasRegistered(key)) { list.Remove(key); }
        }

        public void Register<T>(string key, T item) where T : class {
            list.Add(key, item);
        }

        public T Get<T>(string key) where T : class {
            var value =list[key];
            return (T)(value);
        }

        public bool HasRegistered(string key) {
            return list.ContainsKey(key);
        }
    }
}
