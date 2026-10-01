using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Web.Reports
{
    public partial class Display : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                reportViewer.Visible = true;
                inputParamters = GetParameters();
                reportViewer.LocalReport.ReportPath = ReportFile;
                var reportService = ObjectContainer.Container.Resolve<IReportService>();
                var sources = reportService.GetDataSource(ReportName, inputParamters);
                reportViewer.LocalReport.DataSources.Clear();
                foreach (var key in sources.Keys)
                {
                    var dataSource = new ReportDataSource(key, sources[key]);
                    reportViewer.LocalReport.DataSources.Add(dataSource);
                }
                var parameters = reportService.GetReportParameters(ReportName, inputParamters);
                foreach (var key in parameters.Keys)
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter(key, parameters[key]));
                }
                reportViewer.PageCountMode = PageCountMode.Actual;
                reportViewer.LocalReport.Refresh();
            }
        }

        public Dictionary<string, string> inputParamters { get; set; }

        public string ReportName
        {
            get
            {
                return Request.QueryString["report"];
            }
        }

        public string StoredProcedureName
        {
            get
            {
                return String.Format("Get{0}Report", ReportName);
            }
        }

        public string ReportFile
        {
            get
            {
                switch (ReportName)
                {
                    case LookupConstants.Reports.ActiveAgentsStatistics:
                    case LookupConstants.Reports.NewRegistrationStatistics:
                    case LookupConstants.Reports.RenewalStatistics:
                    case LookupConstants.Reports.TerminatedAgentsStatistics:
                        return String.Format("Rdls\\Statistics.rdlc", ReportName);
                    case LookupConstants.Reports.RenewalAscii:
                        return String.Format("Rdls\\Ascii.rdlc", ReportName);
                    case LookupConstants.Reports.Referred:
                        if (inputParamters.ContainsKey("AgentType") && inputParamters["AgentType"] != "0") return String.Format("Rdls\\{0}Admin.rdlc", ReportName);
                        break;
                    case LookupConstants.Reports.AdministrativeAuditTrail:
                        return @"Rdls\AdministrativeAuditTrail.rdlc";
                    case LookupConstants.Reports.PhotoUploadSummary:
                        return @"Rdls\PhotoUploadSummary.rdlc";
                    case LookupConstants.Reports.TBEExamination:
                        return @"Rdls\TBEExamination.rdlc";
                    case LookupConstants.Reports.ResignedActiveAgents:
                        return @"Rdls\ResignedActiveAgents.rdlc";
                }
                return String.Format("Rdls\\{0}.rdlc", ReportName);
            }
        }

        public Dictionary<string, string> GetParameters()
        {
            var output = new Dictionary<string, string>();
            foreach (var key in Request.QueryString.AllKeys)
            {
                if (key == "report") continue;
                output.Add(key, Request.QueryString[key]);
            }
            return output;
        }


    }// class
}// namespace