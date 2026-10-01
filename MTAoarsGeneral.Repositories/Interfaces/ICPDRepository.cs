using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;


namespace MTAoarsGeneral.Repositories.Interfaces {
    public interface ICPDRepository : IRepository<CPDDetail> {

        bool IsProcessed(int year);

        void Process(int year);

        IEnumerable<CPDDetail> Get(int year, string statusCode);
    }
}
