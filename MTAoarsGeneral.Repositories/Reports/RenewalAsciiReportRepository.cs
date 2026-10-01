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

namespace MTAoarsGeneral.Repositories.Reports {
    
    public class RenewalAsciiReportRepository : IReportRepository{

        EntityContext context;
        public RenewalAsciiReportRepository(EntityContext context) {
            this.context = context;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            var fromDate = DateTime.Parse(parameters["FromDate"]);
            var toDate = DateTime.Parse(parameters["ToDate"]);
            var statusCode1 = parameters["StatusCode1"];
            var statusCode2 = parameters["StatusCode2"];
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.Ascii, context.GetRenewalAsciiReport(fromDate, toDate, statusCode1, statusCode2));
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("FromDate", DateTime.Parse(parameters["FromDate"]).ToShortDateString());
            output.Add("ToDate", DateTime.Parse(parameters["ToDate"]).ToShortDateString());
            return output;
        }

    }// class

}// namespace
