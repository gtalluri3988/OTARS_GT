using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class ExcelUploadVariableSource : INotificationVariableSource {

        public string Code { get; set; }

        public string FileName { get; set; }

        public string RawFilePath { get; set; }

        public string SuccessFilePath { get; set; }

        public string ErrorFilePath { get; set; }

        public int TotalValidRecords { get; set; }

        public int TotalInvalidRecords { get; set; }
    }// class

}// namespace
