using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Constants
{
    public class GlobalConstants
    {

        public const string CurrentIdentity = "CurrentIdentity";

        public const string MTACompanyCode = "995";

        public const string ISMCompanyCode = "996";

        public const string ICICompanyCode = "999";

        public const string DateTimeFormat = "dd/MM/yyyy HH:mm tt";

        public const string DateFormat = "dd/MM/yyyy";
        //public const string DateFormat = "MM/dd/yyyy";

        public const string CaptchaKey = "CaptchaKey";

        public static TimeSpan HugeTransactionTimeSpan = new TimeSpan(8, 0, 0);

        public const string CurrentAdminActivity = "CurrentAdminActivity";

        public const string CurrentReferredAttachments = "CurrentReferredAttachments";

        public const string CurrentConflictAttachments = "CurrentConflictAttachments";

        public const string CurrentRenewalPageSettings = "CurrentRenewalPageSettings";

        public const string CurrentPhoto = "CurrentPhoto";

        public const int MaxUploadMemberCount = 9;

        public const string CurrentCompany = "CurrentCompany";

    }
}
