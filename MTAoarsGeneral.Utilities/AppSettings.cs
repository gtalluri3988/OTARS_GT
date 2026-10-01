using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities
{
    public class AppSettings
    {
        public static string SystemDateFormat { get { return ConfigurationManager.AppSettings["SystemDateFormat"]; } }
        public static string SystemDateTimeFormat { get { return ConfigurationManager.AppSettings["SystemDateTimeFormat"]; } }
    }
}
