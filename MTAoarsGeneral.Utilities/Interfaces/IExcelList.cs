using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MTAoarsGeneral.Utilities.Interfaces {

    public interface IExcelList  : IList {
        string RawColumnsData { get; set; }

        long CompanyID { get; set; }

        string FileName { get; set; }
    }

}
