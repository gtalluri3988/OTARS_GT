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
using System.Globalization;

namespace MTAoarsGeneral.Repositories.Reports {
    
    public class TerminatedAgentsStatisticsReportRepository : IReportRepository{

        EntityContext context;
        ILookupRepository lookupRepository;
        IScopeDataProvider dataProvider;

        public TerminatedAgentsStatisticsReportRepository(EntityContext context, ILookupRepository lookupRepository, IScopeDataProvider dataProvider) {
            this.context = context;
            this.lookupRepository = lookupRepository;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            long? companyId = null;
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (!identity.IsISMOrMTA) companyId = identity.CompanyID;
            var date = DateTime.Parse(parameters["Date"]);
            var category = parameters["RegistrationCategory"];
            var intermediaryTypeId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            var list = new List<Task>();
            var tasks = new Task[] {
                 Task.Factory.StartNew(() => output.Add("BancaOnePrincipal",
                    GetNewContext().GetTerminatedAgentsStatisticsReport(intermediaryTypeId, true, date, 0, 1, category, companyId))),
                Task.Factory.StartNew(() => output.Add("BancaMultiPrincipal",
                    GetNewContext().GetTerminatedAgentsStatisticsReport(intermediaryTypeId, true, date, 2, 5, category, companyId))),
                Task.Factory.StartNew(() => output.Add("NonBancaOnePrincipal",
                    GetNewContext().GetTerminatedAgentsStatisticsReport(intermediaryTypeId, false, date, 0, 1, category, companyId))),
                Task.Factory.StartNew(() => output.Add("NonBancaMultiPrincipal",
                    GetNewContext().GetTerminatedAgentsStatisticsReport(intermediaryTypeId, false, date, 2, 5, category, companyId)))
               
            };

            Task.WaitAll(tasks);
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("Date", parameters["Date"]);
            return output;
        }

        EntityContext GetNewContext() {
            return ObjectContainer.Container.Resolve<EntityContext>();
        }

       

    }// class

}// namespace
