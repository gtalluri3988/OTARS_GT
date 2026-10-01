using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Utilities.Mvc {
    
    public class ExcelList<T> : List<T>, IExcelList {

        public string RawColumnsData { get; set; }

        public long CompanyID { get; set; }

        public string FileName { get; set; }

        public long CreatedBy { get; set; }

    }// class

}// namesapce
