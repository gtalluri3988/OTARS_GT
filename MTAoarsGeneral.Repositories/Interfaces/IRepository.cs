using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces {
    public interface IRepository<T> where T : EntityObject, IEntity {
        T Get(long id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(int pageNo, int pageSize);
        void Save(T item);
        void Save(IEnumerable<T> list);
        void Delete(T item);
        void Attach(T item);
        void Delete(long id);
        void SaveChanges(SaveOptions options = SaveOptions.None);
    }
}
