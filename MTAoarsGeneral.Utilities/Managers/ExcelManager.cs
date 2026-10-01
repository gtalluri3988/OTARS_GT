using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Managers
{
    public class ExcelManager
    {
        public byte[] Generate<T>(IEnumerable<T> items, string fileName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                var i = 0;
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    i++;
                    if (propertyInfo.CanRead)
                    {
                        worksheet.Cells[1, i].Value = propertyInfo.Name;
                        if (propertyInfo.PropertyType == typeof(DateTime?) || propertyInfo.PropertyType == typeof(DateTime))
                            worksheet.Column(i).Style.Numberformat.Format = AppSettings.SystemDateFormat;
                    }
                }

                var range = worksheet.Cells[1, 1, 1, i];
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                worksheet.Cells["A1"].LoadFromCollection(items, true);

                return package.GetAsByteArray();
            }
        }
    }
}
