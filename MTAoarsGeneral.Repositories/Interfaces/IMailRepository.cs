using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces {

    public interface IMailRepository : IRepository<Mail> {

        void ChangeFromStatus(long id, long statusId);

        void ChangeToStatus(long id, long statusId);

        IEnumerable<Mail> GetReceivedMails(int pageNo, int pageSize, string search, string orderBy, long companyId);

        int GetReceivedMailsCount(long companyId, string search);

        int GetReceivedMailsCount(long companyId, long statusId);

        IEnumerable<Mail> GetSentMails(int pageNo, int pageSize, string search, string orderBy, long companyId);

        int GetSentMailsCount(long companyId, string search);

        IEnumerable<Mail> GetReceivedMails(long companyId, long statusId);

    }// interface

}// namespace
