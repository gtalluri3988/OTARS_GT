using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Repositories.Operations {

    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(EntityContext context)
            : base(context) {
        }

        public Dictionary<long, decimal> GetCompanies(int year, int month) {
          var list =   Context.Postings
                .Where(posting => posting.AccountingYear == year
                    && posting.AccountingMonth == month
                    && posting.Amount < 0)
                 .GroupBy(group => new { group.CompanyID })
                 .Select(s => new { CompanyID = s.Key.CompanyID, Total = s.Sum(posting => posting.Amount) });
          var output = new Dictionary<long, decimal>();
          foreach (var item in list) {
              output.Add(item.CompanyID, item.Total);
          }
          return output;
        }

        public IEnumerable<Invoice> GetInvoiceList(int pageNo, int pageSize, string search, string orderBy, long? companyId) {
            var query = Context.Invoices.Where(search) as IQueryable<Invoice>;
            if (companyId != null) query = query.Where(p => p.CompanyID == companyId.Value );
            return query
                .OrderBy(orderBy)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize);
        }


        public IEnumerable<Invoice> GetInvoiceList(long? companyId,int year,int month)
        {
            var query = Context.Invoices as IEnumerable<Invoice>;
            if (companyId != null) query=query.Where(p => p.CompanyID == companyId);
            query= query.Where(p => p.Month == month && p.Year == year);
            return query;
        }
        public int GetInvoiceListCount(long? companyId) {
            var query = Context.Invoices as IEnumerable<Invoice>;
            if (companyId != null) query = query.Where(p => p.CompanyID == companyId) as IEnumerable<Invoice>;
            return query.Count();
        }

        public int GetUnpaidInvoiceCount(long? companyId = null) {
            IEnumerable<Invoice> query = Context.Invoices.Where(p => p.Receipts.Count == 0);
            if (companyId != null) query = query.Where(p => p.CompanyID == companyId);
            return query.Count();
        }

        public IEnumerable<Posting> GetInvoiceDetails(long invoiceId) {
            var invoice = Context.Invoices.First(p => p.ID == invoiceId);
            return Context.Postings.
                Where(p => p.CompanyID == invoice.CompanyID
                    && p.AccountingMonth == invoice.Month
                    && p.AccountingYear == invoice.Year && p.Amount < 0).AsEnumerable();
        }

        public IEnumerable<InvoiceViewDetail> GetInvoiceViewDetails(long invoiceId)
        {
            var items = Context.InvoiceViewDetails.Where(i => i.InvoiceID == invoiceId).ToList();
            return items;
        }

    }// class

}// namespace
