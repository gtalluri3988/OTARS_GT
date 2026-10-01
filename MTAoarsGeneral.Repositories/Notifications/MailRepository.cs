using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Extensions;
    

namespace MTAoarsGeneral.Repositories.Notifications {

    public class MailRepository : GenericRepository<Mail>, IMailRepository{

        ILookupRepository lookupRepository;

        public MailRepository(EntityContext context, ILookupRepository lookupRepository)
            : base(context) {
                this.lookupRepository = lookupRepository;
        }

        public void ChangeFromStatus(long id, long statusId) {
            var mail = Context.Mails.Where(p => p.ID == id).FirstOrDefault();
            if (mail == null) return;
            mail.FromStatusID = statusId;
            Context.SaveChanges();
        }

        public void ChangeToStatus(long id, long statusId) {
            var mail = Context.Mails.Where(p => p.ID == id).FirstOrDefault();
            if (mail == null) return;
            mail.ToStatusID = statusId;
            Context.SaveChanges();
        }

        public IEnumerable<Mail> GetReceivedMails(int pageNo, int pageSize, string search, string orderBy, long companyId) {
            return Context.Mails.Where(p => p.ToCompanyID == companyId)
                .Where(search)
                .OrderBy(orderBy)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize);
        }

        public int GetReceivedMailsCount(long companyId, string search) {
            return Context.Mails.Where(p => p.ToCompanyID == companyId)
                .Where(search).Count();
        }

        public int GetReceivedMailsCount(long companyId, long statusId) {
            return Context.Mails.Where(p => p.ToCompanyID == companyId
                && p.ToStatusID == statusId).Count();
        }

        public IEnumerable<Mail> GetReceivedMails(long companyId, long statusId) {
            return Context.Mails.Where(p => p.ToCompanyID == companyId && p.ToStatusID == statusId);
        }

        public IEnumerable<Mail> GetSentMails(int pageNo, int pageSize, string search, string orderBy, long companyId) {
            return Context.Mails.Where(p => p.FromCompanyID == companyId)
                .Where(search)
                .OrderBy(orderBy)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize);
        }

        public int GetSentMailsCount(long companyId, string search) {
            return Context.Mails.Where(p => p.FromCompanyID == companyId).Where(search).Count();
        }

    }// class

}// namespace
