using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces {
   public interface IInvoiceRepository : IRepository<Invoice> {

        Dictionary<long, decimal> GetCompanies(int year, int month);

        IEnumerable<Invoice> GetInvoiceList(int pageNo, int pageSize, string search, string orderBy, long? companyId);

        IEnumerable<Invoice> GetInvoiceList(long? companyID, int year, int month);

        int GetInvoiceListCount(long? companyId);

        int GetUnpaidInvoiceCount(long? companyId = null);

        IEnumerable<Posting> GetInvoiceDetails(long invoiceId);

        IEnumerable<InvoiceViewDetail> GetInvoiceViewDetails(long invoiceId);

    }// interface

}// class
