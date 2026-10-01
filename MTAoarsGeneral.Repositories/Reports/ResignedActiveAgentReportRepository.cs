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

namespace MTAoarsGeneral.Repositories.Reports
{
    public class ResignedActiveAgentReportRepository : IReportRepository
    {
        EntityContext context;
        public ResignedActiveAgentReportRepository(EntityContext context)
        {
            this.context = context;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters)
        {
            var year = int.Parse(parameters["Year"]);
            var month = int.Parse(parameters["Month"]);
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            context.CommandTimeout = 10 * 60; //10min

            var items = context.GetResignedActiveAgentsReport(month, year).ToList();
            if (items.Count() > 0)
                items = items.OrderBy(i => i.AgentName).ToList();

            output.Add(LookupConstants.Reports.ResignedActiveAgents, items);
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters)
        {
            var output = new Dictionary<string, string>();
            output.Add("Year", parameters["Year"]);
            output.Add("Month", parameters["Month"]);
            return output;
        }
    }
}
