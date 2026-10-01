using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces {
    
    public interface IIbfimResultRepository : IRepository<IbfimResult> {

        IEnumerable<IbfimResult> Search(string icNumber);

        IbfimResult Search(int ID);

        void Update(IbfimResult source,out string message);
    }// interface

}// namespace
