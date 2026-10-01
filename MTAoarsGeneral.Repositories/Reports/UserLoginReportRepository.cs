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
    
    public class UserLoginReportRepository : IReportRepository{

        EntityContext context;
        public UserLoginReportRepository(EntityContext context) {
            this.context = context;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            var fromDate = DateTime.Parse(parameters["FromDate"]);
            var toDate = DateTime.Parse(parameters["ToDate"]);
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.UserLogin, context.GetUserLoginReport(fromDate, toDate));
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("FromDate", parameters["FromDate"]);
            output.Add("ToDate", parameters["ToDate"]);
            return output;
        }

    }// class

}// namespace
