using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Globalization;
using MTAoarsGeneral.Utilities.CSV;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Attributes;
namespace MTAoarsGeneral.Utilities.Mvc
{

    public class ExcelModelBinder : DefaultModelBinder
    {

        Dictionary<string, int> columnPositions;
        Type itemType;
        PropertyInfo[] itemProperties;

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null) return base.BindModel(controllerContext, bindingContext);
            var files = valueProviderResult.RawValue as HttpPostedFileBase[];
            var identity = ObjectContainer.Container.Resolve<IScopeDataProvider>()
                .Get<Identity>(GlobalConstants.CurrentIdentity);
            var list = Activator.CreateInstance(bindingContext.ModelType) as IExcelList;
            list.CompanyID = identity.CompanyID;
            list.FileName = files[0].FileName;
            AssignTypes(bindingContext);
            using (var csvReader = new CsvFileReader(files[0].InputStream, EmptyLineBehavior.Ignore))
            {
                var firstRow = true;
                var columns = new List<string>();
                while (csvReader.ReadRow(columns))
                {
                    var array = columns.ToArray();
                    var raw = String.Join(",", array);
                    if (firstRow)
                    {
                        AssignColumnPositions(array);
                        list.RawColumnsData = raw;
                        firstRow = false;
                    }
                    else
                    {
                        list.Add(GetItem(array));
                    }
                }
            }
            return list;
        }

        void AssignColumnPositions(string header)
        {
            var columns = header.Split(',');
            AssignColumnPositions(columns);
        }

        void AssignColumnPositions(string[] columns)
        {
            columnPositions = new Dictionary<string, int>();
            for (var i = 0; i < columns.Length; i++)
            {
                if (String.IsNullOrEmpty(columns[i].Trim().Replace(" ", ""))) throw new Exception("Column header is empty. Please make sure column header has some value. If CSV is created from excel with empty commas on header, delete the columns and try to create CSV again");
                columnPositions.Add(columns[i].Trim().Replace(" ", ""), i);
            }
        }

        void AssignTypes(ModelBindingContext bindingContext)
        {
            itemType = bindingContext.ModelType.GetGenericArguments()[0];
            itemProperties = itemType.GetProperties();
        }

        object GetItem(string input)
        {
            var values = input.Split(',');
            return GetItem(values);
        }

        object GetItem(string[] values)
        {
            var output = Activator.CreateInstance(itemType, String.Join(",", values)) as ExcelViewModel;
            foreach (var property in itemProperties)
            {
                if (CheckProperty(output, property) == false) continue;
                int position = columnPositions[property.Name];
                if (position >= values.Length) continue;
                AssignValue(output, property, values[position]);
            }
            return output;
        }

        void AssignValue(ExcelViewModel currentItem, PropertyInfo property, string value)
        {
            switch (property.PropertyType.ToString())
            {
                case "System.Nullable`1[System.DateTime]":
                case "System.DateTime":
                    AssignDateTimeValue(currentItem, property, value);
                    break;
                case "System.Nullable`1[System.Decimal]":
                case "System.Decimal":
                    AssignDecimalValue(currentItem, property, value);
                    break;
                case "System.Nullable`1[System.Boolean]":
                case "System.Boolean":
                    AssignBooleanValue(currentItem, property, value);
                    break;
                case "System.Nullable`1[System.Int32]":
                case "System.Int32":
                    AssignIntValue(currentItem, property, value);
                    break;
                default:
                    property.SetValue(currentItem, value, null);
                    break;
            }
        }

        void AssignDateTimeValue(ExcelViewModel currentItem, PropertyInfo property, string value)
        {
            DateTime result;
            if (!String.IsNullOrWhiteSpace(value)) value = value.Trim();
            if (DateTime.TryParseExact(value, "d'/'M'/'yyyy", null, DateTimeStyles.None, out result))
            {
                property.SetValue(currentItem, result, null);
            }
            else if (String.IsNullOrWhiteSpace(value) == false)
            {
                currentItem.AddError("Unable to convert {0} value for the property {1} to DateTime", value, property.Name);
            }
        }

        void AssignDecimalValue(ExcelViewModel currentItem, PropertyInfo property, string value)
        {
            Decimal result;
            if (Decimal.TryParse(value, out result))
            {
                property.SetValue(currentItem, result, null);
            }
            else if (String.IsNullOrWhiteSpace(value) == false)
            {
                currentItem.AddError("Unable to convert {0} value for the property {1} to Decimal", value, property.Name);
            }
        }

        void AssignBooleanValue(ExcelViewModel currentItem, PropertyInfo property, string value)
        {
            var input = (value != null) ? value.ToUpper() : String.Empty;
            switch (input)
            {
                case "YES":
                    property.SetValue(currentItem, true, null);
                    break;
                case "NO":
                    property.SetValue(currentItem, false, null);
                    break;
                default:
                    if (String.IsNullOrWhiteSpace(input) == false)
                    {
                        currentItem.AddError("Unable to convert {0} value for the property {1} to Boolean", value, property.Name);
                    }
                    break;
            }
        }

        void AssignIntValue(ExcelViewModel currentItem, PropertyInfo property, string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                property.SetValue(currentItem, result, null);
            }
            else if (String.IsNullOrWhiteSpace(value) == false)
            {
                currentItem.AddError("Unable to convert {0} value for the property {1} to Int", value, property.Name);
            }
        }

        bool CheckProperty(ExcelViewModel currentItem, PropertyInfo property)
        {
            if (columnPositions.Keys.Contains(property.Name)) return true;
            if (property.GetCustomAttributes(typeof(SkipExcelAttribute), true).Count() > 0) return false;
            // Check for conditional skip based on AgencyTypeCode
            var conditionalSkipAttrs = property.GetCustomAttributes(typeof(SkipExcelForAgencyTypeAttribute), true);
            if (conditionalSkipAttrs.Count() > 0)
            {
                var agencyTypeCodeProp = itemType.GetProperty("AgencyTypeCode");
                if (agencyTypeCodeProp != null)
                {
                    var agencyTypeCode = agencyTypeCodeProp.GetValue(currentItem, null) as string;
                    var skipAttr = conditionalSkipAttrs[0] as SkipExcelForAgencyTypeAttribute;
                    if (skipAttr != null && skipAttr.AgencyTypeCodes.Contains(agencyTypeCode))
                    {
                        return false; // Skip validation for this agency type
                    }
                }
            }

            currentItem.AddError("The property {0} is not exist in the upload file", property.Name);
            return false;
        }

    }// class

}// namespace
