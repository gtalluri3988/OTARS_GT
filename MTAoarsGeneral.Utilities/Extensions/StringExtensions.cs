using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Web;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MTAoarsGeneral.Utilities.Extensions
{
    public static class StringExtensions
    {

        public static string GetModelProperty(this string input, params string[] properties)
        {
            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(input)) sb.Append(input);
            foreach (var property in properties)
            {
                if (String.IsNullOrEmpty(property)) continue;
                if (sb.Length > 0) sb.Append(".");
                sb.Append(property);
            }
            return sb.ToString();
        }

        public static bool IsNewIC(this string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (input.Length != 12) return false;
            DateTime result;
            return DateTime.TryParseExact(input.Substring(0, 6), "yyMMdd", null, DateTimeStyles.None, out result);
        }

        public static string ToProperCase(this string input)
        {
            if (input == null) return input;
            if (input.Length < 2) return input.ToUpper();

            string[] words = input.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1).ToLower() + " ";
            }

            return result.TrimEnd();
        }

        public static string Encrypt(this string item)
        {
            return new EncryptionManager().Encrypt(item);
        }

        public static string Decrypt(this string item)
        {
            return new EncryptionManager().Decrypt(item);
        }


        public static string Encode(this string item)
        {
            return HttpUtility.UrlEncode(item);
        }
        public static string Decode(this string item)
        {
            return HttpUtility.UrlDecode(item);
        }

        public static string WatermarkImage(this string imagepath, string watermark)
        {
            Bitmap bitMapImage = new Bitmap(imagepath);
            Graphics graphicImage = Graphics.FromImage(bitMapImage);
            graphicImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            ////graphicImage.DrawRectangle(new Pen(Color.Black), 0, bitMapImage.Height - 30, bitMapImage.Width, 30);
            //graphicImage.FillRectangle(new SolidBrush(Color.Black), 0, bitMapImage.Height - 30, bitMapImage.Width, 30);
            //graphicImage.DrawString(watermark, new Font("Arial", 11, FontStyle.Regular), new SolidBrush(Color.White), new Point((bitMapImage.Width / 2) - 45, bitMapImage.Height - 25));//new Point(bitMapImage.Width /2 - 45, 15));

            //graphicImage.FillRectangle(new SolidBrush(Color.FromArgb(58, 100, 168)), bitMapImage.Width - 170, bitMapImage.Height - 40, 170, 30);
            //graphicImage.DrawString(watermark, new Font("Arial", 16, FontStyle.Bold), new SolidBrush(Color.White), new Point((bitMapImage.Width - 150), bitMapImage.Height - 25 - 11));

            graphicImage.FillRectangle(new SolidBrush(Color.FromArgb(63, 163, 180)), bitMapImage.Width - 210, bitMapImage.Height - 70, 210, 50);
            graphicImage.DrawString(watermark, new Font("Arial", 24, FontStyle.Bold), new SolidBrush(Color.White), new Point((bitMapImage.Width - 205), bitMapImage.Height - 63));


            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitMapImage.Save(memory, bitMapImage.RawFormat);
                graphicImage.Dispose();
                bitMapImage.Dispose();
                using (System.IO.FileStream fs = new System.IO.FileStream(imagepath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                {
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            return imagepath;
        }

        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        /// <summary>
        /// Comparison key for business registration numbers: upper case without spaces, '-', '/' and '.'
        /// so "PT-1234", "pt 1234" and "PT1234" are the same entity.
        /// Must stay in line with AgencyRepository.SelectByRegistrationNumber.
        /// </summary>
        public static string ToRegistrationNumberKey(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return input.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(".", "").ToUpperInvariant();
        }

        public static bool IsNewBusinessRegistrationNumber(this string input)
        {
            if (input.Length != 12) return false;

            //Check new registration number 12 digits
            var regex12digits = new Regex(@"^\d{12}$");
            if (!regex12digits.IsMatch(input)) return false;

            // var year = Convert.ToInt32(input.Substring(0, 4));
            // if (year < 2019 || year > DateTime.Now.Year) return false; //new business registration launched on 11-Oct-2019

            var businessType = Convert.ToInt32(input.Substring(4, 2)); //Valid type "01","02","03","04","05","06"
            if (businessType > 6) return false;

            return true;
        }
    }
}
