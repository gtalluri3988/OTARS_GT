using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using System.Data.Objects.DataClasses;
using Torrent.Caching.Attribute;
namespace MTAoarsGeneral.Repositories.Interfaces
{

    public interface ILookupRepository
    {
        [Cache]
        IEnumerable<T> GetAll<T>() where T : EntityObject, ILookupEntity;

        [Cache]
        T Get<T>(string code) where T : EntityObject, ILookupEntity;

        [Cache]
        T Get<T>(long id) where T : EntityObject, ILookupEntity;
    }// interface

}// namespace
