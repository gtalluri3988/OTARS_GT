using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using System.Data.Objects.DataClasses;
using Torrent.Caching.Attribute;

namespace MTAoarsGeneral.Repositories.Shared
{
    public class LookupRepository : ILookupRepository
    {

        public LookupRepository(EntityContext context)
        {
            this.Context = context;
        }

        protected EntityContext Context
        {
            get;
            private set;
        }


        public IEnumerable<T> GetAll<T>() where T : EntityObject, ILookupEntity
        {
            var set = Context.CreateObjectSet<T>();
            return set.Where(p => p.IsActive).AsEnumerable();
        }

        public T Get<T>(string code) where T : EntityObject, ILookupEntity
        {
            var list = GetAll<T>();
            return list.Where(p => p.Code == code && p.IsActive).FirstOrDefault();
        }

        public T Get<T>(long id) where T : EntityObject, ILookupEntity
        {
            var list = GetAll<T>();
            return list.Where(p => p.ID == id && p.IsActive).FirstOrDefault();
        }



    }// class
}// namespace
