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

namespace MTAoarsGeneral.Repositories.Reports {
    
    public class ALCReportRepository : IReportRepository{

        EntityContext context;
        IScopeDataProvider dataProvider;
        public ALCReportRepository(EntityContext context, IScopeDataProvider dataProvider) {
            this.context = context;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            var fromDate = GetDate(parameters, "FromDate");
            var toDate = GetDate(parameters, "ToDate");
            var status = parameters["ReportFor"];
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.ALC, context.GetALCReport(fromDate, toDate, status));
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("ReportFor", parameters["ReportFor"]);
            return output;
        }

        DateTime? GetDate(Dictionary<string, string> parameters, string key) {
            if (!parameters.ContainsKey("FromDate")) return null;
            DateTime dateTime;
            if (!DateTime.TryParse(parameters[key], out dateTime)) return null;
            return dateTime;
        }

    }// class

}// namespace
