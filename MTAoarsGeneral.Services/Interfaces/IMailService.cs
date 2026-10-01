using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.ViewModels.Shared;

namespace MTAoarsGeneral.Services.Interfaces {
    
    public interface IMailService : IBaseService {

        MailListViewModel GetReceivedMails(GridPageViewModel model);

        MailListViewModel GetSentMails(GridPageViewModel model);

        MailViewModel Read(long id);

        MailReplyViewModel GetReplyMail(long id);

        void Send(MailReplyViewModel model);

        void Send(NewMailViewModel model);
    }// interface

}// namespace
