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

namespace MTAoarsGeneral.Repositories.Reports
{
    public class AdministrativeAuditTrailReportRepository : IReportRepository
    {
        EntityContext context;
        public AdministrativeAuditTrailReportRepository(EntityContext context)
        {
            this.context = context;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters)
        {
            var fromDate = DateTime.Parse(parameters["FromDate"]);
            var toDate = DateTime.Parse(parameters["ToDate"]);
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.AdministrativeAuditTrail, context.GetAdministrativeAuditTrailReport(fromDate, toDate));
            return output;
        }

        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters)
        {
            var output = new Dictionary<string, string>();
            output.Add("FromDate", DateTime.Parse(parameters["FromDate"]).ToShortDateString());
            output.Add("ToDate", DateTime.Parse(parameters["ToDate"]).ToShortDateString());
            return output;
        }
    }
}
