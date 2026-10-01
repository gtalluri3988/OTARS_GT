using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.ViewModels.Notifications {
    public class MailViewModel {

        public long ID { get; set; }

        public string FromCompanyName { get; set; }

        public string ToCompanyName { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string FromStatusDescription { get; set; }

        public string ToStatusDescription { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedOnDisplay {
            get {
                return CreatedOn.ToString(GlobalConstants.DateTimeFormat);
            }
        }

    }// class
}// namespace
