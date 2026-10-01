using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MTAoarsGeneral.Utilities.Config {
    
    public class ConfigManager {

        public virtual string EntityConextConnectionString {
            get {
                return ConfigurationManager.ConnectionStrings["EntityContext"].ConnectionString;
            }
        }

        public virtual bool CPDCheckingAgainstRegistration {
            get {
                return bool.Parse(ConfigurationManager.AppSettings["CPDCheckingAgainstRegistration"]);
            }
        }

        public virtual string UploadBaseDirectory {
            get {
                return ConfigurationManager.AppSettings["UploadBaseDirectory"];
            }
        }

        public virtual string PhotoUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["PhotoUrl"];
            }
        }

        public virtual string SSOUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SSOUrl"];
            }
        }

        public virtual int CoolingOffPeriod {
            get { return 3; }
        }

        public virtual int ConflictFirstReminderDuration {
            get {
                return int.Parse(ConfigurationManager.AppSettings["ConflictFirstReminderDuration"]);
            }
        }

        public virtual int ConflictSecondReminderDuration {
            get {
                return int.Parse(ConfigurationManager.AppSettings["ConflictSecondReminderDuration"]);
            }
        }

        public virtual int ConflictThirdReminderDuration {
            get {
                return int.Parse(ConfigurationManager.AppSettings["ConflictThirdReminderDuration"]);
            }
        }

        public virtual int ConflictAutoCloseDuration {
            get {
                return int.Parse(ConfigurationManager.AppSettings["ConflictAutoCloseDuration"]);
            }
        }

    }// class
}// namespace
