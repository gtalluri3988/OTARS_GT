using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MTAoarsGeneral.Web
{
    public class AppSettings
    {
        public static string Authority { get { return ConfigurationManager.AppSettings["Authority"]; } }
        public static string SSOAuthBaseUri { get { return ConfigurationManager.AppSettings["SSOAuthBaseUri"]; } }
        public static string RedirectUri { get { return ConfigurationManager.AppSettings["RedirectUri"]; } }

        public static string IssuerSigningCert { get { return ConfigurationManager.AppSettings["issuerSigningCert"]; } }

        public static string SystemDateFormat { get { return ConfigurationManager.AppSettings["SystemDateFormat"]; } }
        public static string SystemDateTimeFormat { get { return ConfigurationManager.AppSettings["SystemDateTimeFormat"]; } }
    }
}