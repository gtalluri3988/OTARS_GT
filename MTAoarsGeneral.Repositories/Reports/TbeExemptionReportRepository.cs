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

    public class TbeExemptionReportRepository : IReportRepository
    {

        EntityContext context;
        public TbeExemptionReportRepository(EntityContext context)
        {
            this.context = context;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters)
        {
            var fromDate = DateTime.Parse(parameters["FromDate"]);
            var toDate = DateTime.Parse(parameters["ToDate"]);
            var search = (parameters["Search"] ?? "").ToString();
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            context.CommandTimeout = 10 * 60; //10min

            var items = context.GetTbeExemptionReport(fromDate, toDate, search).ToList();

            output.Add(LookupConstants.Reports.TBEExemption, items);
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters)
        {
            var output = new Dictionary<string, string>();
            output.Add("FromDate", parameters["FromDate"]);
            output.Add("ToDate", parameters["ToDate"]);
            output.Add("Search", (parameters["Search"] ?? "").ToString());
            return output;
        }

    }// class

}// namespace
