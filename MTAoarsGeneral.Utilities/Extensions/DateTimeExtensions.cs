using MTAoarsGeneral.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Extensions {
    
    public static class DateTimeExtensions
    {
        public static string SystemDateFormat { get { return ConfigurationManager.AppSettings["SystemDateFormat"] ?? GlobalConstants.DateFormat; } }
        public static string SystemDateTimeFormat { get { return ConfigurationManager.AppSettings["SystemDateTimeFormat"] ?? GlobalConstants.DateTimeFormat; } }

        public static int GetQuarter(this DateTime date) {
            var month = date.Month;
            if (month % 3 == 0) return month / 3;
            return Convert.ToInt32(month / 3) + 1;
        }

        public static DateTime GetCurrentQuarterStartDate(this DateTime date) {
            var quarter = date.GetQuarter();
            var month = ((quarter - 1) * 3) + 1;
            return new DateTime(date.Year, month, 1);
        }

        public static DateTime GetCurrentQuarterEndDate(this DateTime date) {
            return date.GetCurrentQuarterStartDate().AddMonths(3).AddDays(-1);
        }

        public static DateTime GetPreviousQuarterEndDate(this DateTime date) {
            return date.GetCurrentQuarterStartDate().AddDays(-1);
        }

        public static DateTime GetQuarterStartDate(this DateTime date, int quarter, int year) {
            var month = ((quarter - 1) * 3) + 1;
            return new DateTime(year, month, 1);
        }

        public static DateTime GetQuarterEndDate(this DateTime date, int quarter, int year) {
            return date.GetQuarterStartDate(quarter, year).AddMonths(3).AddDays(-1);
        }



        /// <summary>
        /// Convert to Date (string) using AppSettings format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateFormat(this DateTime? date)
        {
            if (date == null) return string.Empty;
            return Convert.ToDateTime(date).ToDateFormat();
        }
        public static string ToDateFormat(this DateTime date)
        {
            return date.ToString(SystemDateFormat);
        }

        /// <summary>
        /// Convert to Date Time (string) using AppSettings format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateTimeFormat(this DateTime? date)
        {
            if (date == null) return String.Empty;
            return Convert.ToDateTime(date).ToDateTimeFormat();
        }
        public static string ToDateTimeFormat(this DateTime date)
        {
            return date.ToString(SystemDateFormat);
        }

    }// class

}// namespace
