using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Notifications {
    public class MailReplyViewModel {

        public long ID { get; set; }

        public long SoureceMailID { get; set; }

        public long FromCompanyID { get; set; }

        public long ToCompanyID { get; set; }

        public string FromCompanyName { get; set; }

        public string ToCompanyName { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }// class
}// namespace
