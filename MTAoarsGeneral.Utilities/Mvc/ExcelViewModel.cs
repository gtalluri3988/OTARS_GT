using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MTAoarsGeneral.Utilities.Mvc {
    
    public class ExcelViewModel {

        string rawData;
        string remarks;
        List<string> errors;
       
        public ExcelViewModel(string rawData) {
            this.rawData = rawData;
            this.errors = new List<string>();
        }

        public string GetRawData() {
            return rawData;
        }

        public List<string> GetErrors() {
            return errors;
        }

        public void AddError(string format, params string[] args) {
            errors.Add(String.Format(format, args));
        }

        public bool IsValid() {
            return errors.Count == 0;
        }

        public void SetRemarks(string remarks) {
            this.remarks = remarks;
        }

        public string GetRemarks() {
            return this.remarks;
        }

    }// class

}// namespace
