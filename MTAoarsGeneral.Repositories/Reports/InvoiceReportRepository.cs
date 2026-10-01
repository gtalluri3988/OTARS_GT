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
    
    public class InvoiceReportRepository : IReportRepository{

        EntityContext context;
        IScopeDataProvider dataProvider;
        public InvoiceReportRepository(EntityContext context,  IScopeDataProvider dataProvider) {
            this.context = context;
            this.dataProvider = dataProvider;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters) {
            var year = int.Parse(parameters["Year"]);
            var month = int.Parse(parameters["Month"]);
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);            
            long? companyId = null;            
            if(parameters.ContainsKey("CompanyID"))companyId=long.Parse(parameters["CompanyID"]);
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            if (parameters["ReportFor"] == "InvoiceDetail")
            {
                output.Add(LookupConstants.Reports.InvoiceDetail, context.GetInvoiceDetail(year, month,companyId));
            }
            else
            {
                output.Add(LookupConstants.Reports.Invoice, context.GetInvoiceReport(year, month));
            }
            return output;
        }


        public Dictionary<string, string> GetParameters(Dictionary<string, string> parameters) {
            var output = new Dictionary<string, string>();
            output.Add("Year", parameters["Year"]);
            output.Add("Month", parameters["Month"]);
            if (parameters["ReportFor"] == "InvoiceDetail")
            {
                Company fromCompany;
                Company toCompany;
                Invoice invoice;
                var companyID =long.Parse( parameters["CompanyID"]);
                var month = int.Parse(parameters["Month"]);
                var year =int.Parse( parameters["Year"]);
                invoice = context.Invoices.FirstOrDefault(i => i.CompanyID ==companyID && i.Month==month && i.Year == year);
                if (invoice == null) return output;
                fromCompany = context.Companies.FirstOrDefault(c => c.Code == GlobalConstants.MTACompanyCode);
                toCompany = context.Companies.FirstOrDefault(c => c.ID == companyID);
                if (toCompany.Code == GlobalConstants.MTACompanyCode)
                    fromCompany = context.Companies.FirstOrDefault(c => c.Code == GlobalConstants.ISMCompanyCode);

                output.Add("InvoiceNo", invoice.Number);
                output.Add("FromCompanyName", fromCompany.Name.ToUpper());
                output.Add("FromCompanyAddress1", fromCompany.Address.Address1.ToUpper());
                output.Add("FromCompanyAddress2", fromCompany.Address.Address2.ToUpper());
                output.Add("FromCompanyCity", fromCompany.Address.City.ToUpper());
                output.Add("FromCompanyState",fromCompany.Address.LookupState==null?string.Empty: fromCompany.Address.LookupState.Description.ToUpper());
                output.Add("FromCompanyPostalCode", fromCompany.Address.PostalCode.ToUpper());


                output.Add("ToCompanyName", toCompany.Name.ToUpper());
                output.Add("ToCompanyAddress1", toCompany.Address.Address1.ToUpper());
                output.Add("ToCompanyAddress2", toCompany.Address.Address2.ToUpper());
                output.Add("ToCompanyCity", toCompany.Address.City.ToUpper());
                output.Add("ToCompanyState", toCompany.Address.LookupState == null ? string.Empty : toCompany.Address.LookupState.Description.ToUpper());
                output.Add("ToCompanyPostalCode", toCompany.Address.PostalCode.ToUpper());
                output.Add("ToCompanyEmail",toCompany.CompanyContacts.Count>0? toCompany.CompanyContacts.First().Email:string.Empty);
                output.Add("ToCompanyTelephone", toCompany.CompanyContacts.Count > 0 ? toCompany.CompanyContacts.First().Phone : string.Empty);
                output.Add("ToCompanyFax", toCompany.CompanyContacts.Count > 0 ? toCompany.CompanyContacts.First().Fax : string.Empty);
                output.Add("ToCompanyAccountNo", toCompany.AccountNo);
                output.Add("IsMTA", fromCompany.Code == GlobalConstants.MTACompanyCode ? "True" : "False");

                output.Add("Date", DateTime.Now.ToString("dd/MM/yyyy"));
            }
            return output;
        }

    }// class

}// namespace
