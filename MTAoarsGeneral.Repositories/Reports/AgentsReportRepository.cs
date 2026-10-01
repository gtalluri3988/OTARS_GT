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
    
    public class AgentsReportRepository : IReportRepository{

        EntityContext context;
        IScopeDataProvider dataProvider;
        public AgentsReportRepository(EntityContext context, IScopeDataProvider dataProvider) {
            this.context = context;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters)
        {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            bool? isBancaStaff = null;
            if (parameters.ContainsKey("IsBancaStaff"))
            {
                if (parameters["IsBancaStaff"] != "All") isBancaStaff = bool.Parse(parameters["IsBancaStaff"]);
            }
            long? intermediaryTypeID = context.LookupIntermediaryTypes.Single(i => i.Code == LookupConstants.IntermediaryType.General).ID;

            //if (!intermediaryTypeID.HasValue)
            //{
            //    if (parameters.ContainsKey("IntermediaryTypeID"))
            //    {
            //        if (!String.IsNullOrEmpty(parameters["IntermediaryTypeID"]))
            //        {
            //            intermediaryTypeID = long.Parse(parameters["IntermediaryTypeID"]);
            //        }
            //        else
            //        {
            //            if (!identity.IsISMOrMTA)
            //            {
            //                intermediaryTypeID = (identity.IsFamily) ?
            //                    context.LookupIntermediaryTypes.Single(i => i.Code == LookupConstants.IntermediaryType.Family).ID :
            //                    context.LookupIntermediaryTypes.Single(i => i.Code == LookupConstants.IntermediaryType.General).ID;
            //            }
            //        }
            //    }
            //}
            
            DateTime? fromDate=null ;

            if (parameters.ContainsKey("FromDate")) {
                DateTime dateTime;
                DateTime.TryParse(parameters["FromDate"], out dateTime);
                fromDate = dateTime;
                if (dateTime == DateTime.MinValue)
                    fromDate = null;
            }
            DateTime? toDate =null;
            if (parameters.ContainsKey("ToDate")) 
            {
                DateTime dateTime;
                DateTime.TryParse(parameters["ToDate"], out dateTime);
                toDate = dateTime;
                if (dateTime == DateTime.MinValue)
                    toDate = null;
            }
            
            long? companyId = null;
            if(!identity.IsISMOrMTA) companyId = identity.CompanyID;
            if (identity.IsISMOrMTA) {
                if (parameters.ContainsKey("CompanyID")) {
                    long companyValue;
                    long.TryParse(parameters["CompanyID"], out companyValue);
                    companyId = companyValue;
                    var company = context.Companies.FirstOrDefault(c => c.ID == companyId.Value);
                    if (company != null)
                        parameters.Add("TOName", company.Name);
                }
            }
            int? statusID=null;
            parameters.Add("DateHeader", string.Empty);
            parameters.Add("StatusHeader", string.Empty);
            parameters.Add("SummaryStatusHeader", string.Empty);
          
            if(!parameters.ContainsKey("TOName"))
            parameters.Add("TOName", identity.IsISMOrMTA ? "All" : identity.CompanyName);
            var code = string.Empty;
            if (parameters.ContainsKey("ActionID")) {
                statusID = int.Parse(parameters["ActionID"]);
                var action = context.LookupTerminationActions.FirstOrDefault(a => a.ID == statusID);
                code = action.Code;
            }

            if (parameters.ContainsKey("StatusCode")) {
                code = parameters["StatusCode"];      
            }
            parameters["DateHeader"] = "Date " + GetHeader(code);
            parameters["StatusHeader"] = GetHeader(code);
            parameters["SummaryStatusHeader"] = GetSummaryStatusHeader(code); 
            int? year=null;
            if(parameters.ContainsKey("Year")) 
                if(!string.IsNullOrEmpty(parameters["Year"])) year=int.Parse(parameters["Year"]);
            int? quarter=null;
            if (parameters.ContainsKey("Quarter"))
                if (!string.IsNullOrEmpty(parameters["Quarter"])) quarter = int.Parse(parameters["Quarter"]);

            parameters.Add("SearchValue",GetSearchBy(parameters));
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            if (parameters["ReportFor"] == "ActiveAgents") {
                //output.Add(LookupConstants.Reports.Agents, context.GetActiveAgentsReport(intermediaryTypeID, isBancaStaff, fromDate, toDate, companyId));
                var items = context.GetActiveAgentsReport(intermediaryTypeID, isBancaStaff, fromDate, toDate, companyId).ToList();
                AssignEmptyRow(ref items);
                output.Add(LookupConstants.Reports.Agents, items);
            } else if (parameters["ReportFor"] == "TerminationList") {
                output.Add(LookupConstants.Reports.Agents,context.GetTerminationReport(fromDate,toDate,statusID,companyId));           
            } else if (parameters["ReportFor"] == "TerminationSummary") {
                 output.Add(LookupConstants.Reports.Companies,context.GetTerminationSummaryReport(fromDate,toDate,companyId));
            } else if (parameters["ReportFor"] == "Consolidate") {
                output.Add(LookupConstants.Reports.Agents,context.GetConsolidateReport(year,quarter,companyId,intermediaryTypeID,isBancaStaff,code));
            } else if (parameters["ReportFor"] == "ConsolidateSummary") {
                output.Add(LookupConstants.Reports.Companies, context.GetConsolidateSummaryReport(year, quarter, companyId, intermediaryTypeID, isBancaStaff,code));
            } else if (parameters["ReportFor"] == "AutoTermination") {
                output.Add(LookupConstants.Reports.Agents, context.GetAutoTerminationReport(fromDate, toDate, companyId));
            } else
            {
                //output.Add(LookupConstants.Reports.Agents, context.GetTerminatedAgentsReport(intermediaryTypeID, isBancaStaff, fromDate, toDate, companyId));
                var items = context.GetTerminatedAgentsReport(intermediaryTypeID, isBancaStaff, fromDate, toDate, companyId).ToList();
                AssignEmptyRow(ref items);
                output.Add(LookupConstants.Reports.Agents, items);
            }
            return output;
        }

        private void AssignEmptyRow<T>(ref List<T> items)
        {
            var empty = (T)Activator.CreateInstance(typeof(T));
            if (items.Count() == 0)
                items.Add(empty);
        }

        private string GetHeader(string code) {
            switch (code) {
                case LookupConstants.TerminationAction.Terminate:
                    return "Terminated";                   
                case LookupConstants.TerminationAction.Resign:
                    return "Resigned";                   
                case LookupConstants.TerminationAction.NotRelease:
                    return "Not To Released";
                case LookupConstants.RenewalDetailStatus.Renew:
                    return "Renewed";
            }
            return string.Empty;
        }
        private string GetSummaryStatusHeader(string  code) {
            switch (code) {
                case LookupConstants.TerminationAction.Terminate:
                    return "No of Agents Terminated";
                case LookupConstants.TerminationAction.Resign:
                    return "No of Agents Resigned";
                case LookupConstants.TerminationAction.NotRelease:
                    return "No of Agents Not To Released";
                case LookupConstants.RenewalDetailStatus.Renew:
                    return "No of Agents Renewed";

            }
            return string.Empty;
        }

        private string GetSearchBy(Dictionary<string,string> parameters) {
            string result=string.Empty;
              if (parameters.ContainsKey("IsBancaStaff"))                  
                if (parameters["IsBancaStaff"] != "All") {
                    if (bool.Parse(parameters["IsBancaStaff"]))
                        result = "Banca";
                    else
                        result = "Non Banca";
                }
              
              var intermediary = context.LookupIntermediaryTypes.FirstOrDefault(i => i.Code == LookupConstants.IntermediaryType.General);
              if (intermediary != null) {
                  if(!string.IsNullOrEmpty(result))
                      result+=" or ";
                  result += intermediary.Description;
              }
            return result;
        }
      
        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();

            if (parameters["ReportFor"] == "ActiveAgents" || parameters["ReportFor"] == "TerminatedAgents") 
                output.Add("ReportFor", parameters["ReportFor"]);

            if (parameters["ReportFor"] == "TerminationList" || parameters["ReportFor"] == "TerminationSummary") {
                output.Add("FromDate", parameters["FromDate"]);
                output.Add("ToDate", parameters["ToDate"]);
            }

            if (parameters["ReportFor"] == "TerminationList" || parameters["ReportFor"]=="Consolidate" || parameters["ReportFor"]=="ConsolidateSummary") {
                if(parameters["ReportFor"] == "TerminationList")
                    output.Add("DateHeader", parameters["DateHeader"]);

                if (parameters["ReportFor"] == "Consolidate" || parameters["ReportFor"] == "TerminationList")           
                    output.Add("TOName", parameters["TOName"]);

                output.Add("StatusHeader", parameters["StatusHeader"]);

                if (parameters["ReportFor"] == "ConsolidateSummary")
                    output.Add("SummaryStatusHeader", parameters["SummaryStatusHeader"]);

                if (parameters["ReportFor"] != "TerminationList") {
                    output.Add("SearchValue", parameters["SearchValue"]);
                    output.Add("Year", parameters["Year"]);
                    output.Add("Quarter", parameters["Quarter"]);
                }
            }
            return output;
        }

    }// class

}// namespace
