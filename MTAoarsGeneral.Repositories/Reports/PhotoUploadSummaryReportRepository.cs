using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using System;
using System.Collections.Generic;

namespace MTAoarsGeneral.Repositories.Reports
{
    public class PhotoUploadSummaryReportRepository : IReportRepository
    {
        EntityContext context;
        public PhotoUploadSummaryReportRepository(EntityContext context)
        {
            this.context = context;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters)
        {
            var fromDate = DateTime.Parse(parameters["FromDate"]);
            var toDate = DateTime.Parse(parameters["ToDate"]);
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.PhotoUploadSummary, context.GetPhotoUploadSummaryReport(fromDate, toDate));
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
