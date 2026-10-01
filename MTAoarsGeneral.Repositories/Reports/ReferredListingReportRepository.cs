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
    
    public class ReferredListingReportRepository : IReportRepository{

        EntityContext context;
        ILookupRepository lookupRepository;
        IScopeDataProvider dataProvider;

        public ReferredListingReportRepository(EntityContext context, ILookupRepository lookupRepository, IScopeDataProvider dataProvider)
        {
            this.context = context;
            this.lookupRepository = lookupRepository;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            long? categoryId = null;
            DateTime? dateFrom = null, dateTo = null;
            string name = null, ic = null;
            int agentType = 0;
            if (parameters.ContainsKey("ApprovedFromDate") && !String.IsNullOrEmpty(parameters["ApprovedFromDate"]) ) dateFrom = DateTime.Parse(parameters["ApprovedFromDate"]);
            if (parameters.ContainsKey("ApprovedToDate") && !String.IsNullOrEmpty(parameters["ApprovedToDate"])) dateTo = DateTime.Parse(parameters["ApprovedToDate"]);
            if (parameters.ContainsKey("CategoryID")) categoryId = long.Parse(parameters["CategoryID"]);
            if (parameters.ContainsKey("TakafulName")) name = parameters["TakafulName"];
            if (parameters.ContainsKey("NewICNumber")) ic = parameters["NewICNumber"];
            if (parameters.ContainsKey("AgentType") && !int.TryParse(parameters["AgentType"], out agentType)) agentType = 0;

            //To cater same datetime (dateFrom, dateTo) in search
            if (dateFrom.HasValue && dateTo.HasValue && DateTime.Compare(Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo)) == 0)
            {
                dateTo = Convert.ToDateTime(dateFrom).AddDays(1).AddSeconds(-1);
            }

            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.Referred, context.GetReferredListingReport(categoryId, name, dateFrom, dateTo, identity.UserID, agentType, ic));
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            if (parameters.ContainsKey("ApprovedFromDate") && !String.IsNullOrEmpty(parameters["ApprovedFromDate"]))
                output.Add("ApprovedFromDate", DateTime.Parse(parameters["ApprovedFromDate"]).ToShortDateString());
            if (parameters.ContainsKey("ApprovedToDate") && !String.IsNullOrEmpty(parameters["ApprovedToDate"]))
                output.Add("ApprovedToDate", DateTime.Parse(parameters["ApprovedToDate"]).ToShortDateString());
            output.Add("CategoryID", parameters["CategoryID"]);
            output.Add("TakafulName", parameters["TakafulName"]);
            output.Add("NewICNumber", parameters["NewICNumber"]);
            output.Add("AgentType", parameters["AgentType"]);
            return output;
        }
    }// class

}// namespace
