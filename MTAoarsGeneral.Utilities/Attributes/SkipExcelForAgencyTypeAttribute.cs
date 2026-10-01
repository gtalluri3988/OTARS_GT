using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Attributes
{
    /// <summary>
    /// Attribute to conditionally skip Excel property validation based on AgencyTypeCode
    /// </summary>
    public class SkipExcelForAgencyTypeAttribute : Attribute
    {
        public string[] AgencyTypeCodes { get; set; }

        public SkipExcelForAgencyTypeAttribute(params string[] agencyTypeCodes)
        {
            AgencyTypeCodes = agencyTypeCodes ?? new string[0];
        }
    }
}

