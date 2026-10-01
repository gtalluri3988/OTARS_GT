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
    
    public class CPDReportRepository : IReportRepository{

        EntityContext context;
        IScopeDataProvider dataProvider;
        public CPDReportRepository(EntityContext context, IScopeDataProvider dataProvider) {
            this.context = context;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            var year = int.Parse(parameters["Year"]);
            bool? isBancaStaff = null;
            if (parameters["IsBancaStaff"] != "All") isBancaStaff = bool.Parse(parameters["IsBancaStaff"]);
            long? intermediaryTypeID = context.LookupIntermediaryTypes.Single(i => i.Code == LookupConstants.IntermediaryType.General).ID;
            bool? isAchieved = null;
            if (parameters["IsAchieved"] != "All") isAchieved = bool.Parse(parameters["IsAchieved"]);
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            long? companyId = null;
            if (!identity.IsISMOrMTA) {
                companyId = identity.CompanyID;
            } else {
                if (parameters.ContainsKey("CompanyID")) {
                    long companyValue;
                    if (long.TryParse(parameters["CompanyID"], out companyValue)) companyId = companyValue;
                }
            }
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.CPD, context.GetCPDReport(year, intermediaryTypeID, isBancaStaff, isAchieved, companyId));
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("Year", parameters["Year"]);
            return output;
        }

    }// class

}// namespace
