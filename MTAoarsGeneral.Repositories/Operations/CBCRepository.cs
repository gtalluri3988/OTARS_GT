using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Repositories.Operations {

    public class CBCRepository : GenericRepository<CBCHeader>, ICBCRepository {
        ConfigManager config;
        IScopeDataProvider dataProvider;
        public CBCRepository(EntityContext context, ConfigManager config, IScopeDataProvider dataProvider)
            : base(context) {
                this.config = config;
                this.dataProvider = dataProvider;
        }

        public CBCHeader Get(int year, int quarter) {
            var item = Context.CBCHeaders.FirstOrDefault(p => p.Year == year && p.Quarter == quarter);
            return item;
        }

        public IEnumerable<CBCPivotDetail> GetPivot(int year, int quarter) {
            return Context.GetCBCPivot(year, quarter).ToList();
        }

        public IEnumerable<CBCDetail> GetDetails(int year, int quarter, long agencyId) {
            return Context.CBCDetails.Where(p => p.CBCHeader.Year == year && p.CBCHeader.Quarter == quarter
                && p.AgencyID == agencyId).ToList();
        }

        public IEnumerable<CBCDetail> GetDetails(int quarterValue) {
            return Context.CBCDetails.Where(p => (p.CBCHeader.Year* 4 + p.CBCHeader.Quarter) == quarterValue).ToList();

        }

    }// class

}// namespace
