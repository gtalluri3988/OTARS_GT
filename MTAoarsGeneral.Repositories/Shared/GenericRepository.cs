using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Repositories.Shared {
    public class GenericRepository<T> : IRepository<T> where T : EntityObject, IEntity {

        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public GenericRepository(EntityContext context) {
            this.Context = context;

            this.Set = context.CreateObjectSet<T>();

        }

        protected EntityContext Context {
            get;
            private set;
        }

        protected ObjectSet<T> Set {
            get;
            private set;
        }

        public T Get(long id) {
            return Set.Where(p => p.ID == id).FirstOrDefault();

        }

        public IEnumerable<T> GetAll() {
            return Set.AsEnumerable();
        }

        public IEnumerable<T> GetAll(int pageNo, int pageSize) {
            return Set
                .OrderBy(item => item.ID)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize);
        }

        public void Save(T item) {
            if (item.EntityState == EntityState.Modified) {
                Set.Attach(item);
            } else {
                Set.AddObject(item);
            }
        }

        public void Save(IEnumerable<T> list) {
            foreach (var item in list) Save(item);
        }
        public void Delete(T item) {
            Set.DeleteObject(item);
        }

        public void Attach(T item) {
            Set.Attach(item);
        }

        public void Delete(long id) {
            var item = Get(id);
            if (item == null) return;
            Delete(item);

        }

        public void SaveChanges(SaveOptions options = SaveOptions.None) {
            Context.SaveChanges(options);
            Context.AcceptAllChanges();
        }

        public bool IsSlowQuery
        {
            set
            {
                this.Context.CommandTimeout = 300; //5 mins
            }
        }
    }
}
