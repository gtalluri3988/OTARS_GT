using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Data.EntityClient;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using System.Threading.Tasks;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.Repositories.Reports {
    
    public class RenewalStatisticsReportRepository : IReportRepository{

        EntityContext context;
        ILookupRepository lookupRepository;
        IScopeDataProvider dataProvider;

        public RenewalStatisticsReportRepository(EntityContext context, ILookupRepository lookupRepository, IScopeDataProvider dataProvider) {
            this.context = context;
            this.lookupRepository = lookupRepository;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            long? companyId = null;
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (!identity.IsISMOrMTA) companyId = identity.CompanyID;
            var year = int.Parse(parameters["Year"]);
            var quarter = int.Parse(parameters["Quarter"]);
            var category = parameters["RegistrationCategory"];
            var intermediaryTypeId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            var list = new List<Task>();
            var tasks = new Task[] {
                 Task.Factory.StartNew(() => output.Add("BancaOnePrincipal",
                    GetNewContext().GetRenewalStatisticsReport(intermediaryTypeId, true, year, quarter, 0, 1, category, companyId))),
                Task.Factory.StartNew(() => output.Add("BancaMultiPrincipal",
                    GetNewContext().GetRenewalStatisticsReport(intermediaryTypeId, true, year, quarter, 2, 5, category, companyId))),
                Task.Factory.StartNew(() => output.Add("NonBancaOnePrincipal",
                    GetNewContext().GetRenewalStatisticsReport(intermediaryTypeId, false, year, quarter, 0, 1, category, companyId))),
                Task.Factory.StartNew(() => output.Add("NonBancaMultiPrincipal",
                    GetNewContext().GetRenewalStatisticsReport(intermediaryTypeId, false, year, quarter, 2, 5, category, companyId)))
                
            };

            Task.WaitAll(tasks);
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("Year", parameters["Year"]);
            output.Add("Quarter", parameters["Quarter"]);
            return output;
        }

        EntityContext GetNewContext() {
            return ObjectContainer.Container.Resolve<EntityContext>();
        }

    }// class

}// namespace
