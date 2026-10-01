using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace MTAoarsGeneral.Utilities.Extensions {
    public static class CollectionExtension {

        public static void AddRange<TEntity>(this EntityCollection<TEntity> entities, IEnumerable<TEntity> items) where TEntity:class {
            foreach (var entity in items) {
                entities.Add(entity);
            }
        }
        public static void RemoveRange<TEntity>(this EntityCollection<TEntity> entities, IEnumerable<TEntity> items) where TEntity : class
        {
            var list = entities.ToList();
            for (int i = list.Count - 1; i >= 0; i--) {
                entities.Remove(list[i]);
            }
            /*foreach (var entity in items)
            {
                entities.Remove(entity);
            }*/
        }

    }
}
