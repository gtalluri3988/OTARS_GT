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

    public class CPDRepository : GenericRepository<CPDDetail>, ICPDRepository {
        ConfigManager config;
        IScopeDataProvider dataProvider;
        public CPDRepository(EntityContext context, ConfigManager config, IScopeDataProvider dataProvider)
            : base(context) {
                this.config = config;
                this.dataProvider = dataProvider;
        }

        public bool IsProcessed(int year) {
            return Context.CPDDetails.Count(p => p.Year == year) > 0;
        }

        public void Process(int year) {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            Context.ProcessCPD(year, identity.UserID);
        }

        public IEnumerable<CPDDetail> Get(int year, string statusCode) {
            return Context.CPDDetails.Where(p => p.Year == year && p.LookupCPDStatus.Code == statusCode).ToList();
        }

    }// class

}// namespace
