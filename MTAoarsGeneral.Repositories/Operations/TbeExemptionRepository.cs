using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Config;

namespace MTAoarsGeneral.Repositories.Operations {

    public class TbeExemptionRepository : GenericRepository<TbeExemption>, ITbeExemptionRepository {

        public TbeExemptionRepository(EntityContext context)
            : base(context) {
              
        }

        public IEnumerable<TbeExemption> Search(string icNumber) {
            return Context.TbeExemptions.Where(p => p.ICNumber == icNumber).AsEnumerable();
        }


    }// class

}// namespace
