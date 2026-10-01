using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Mvc {

    public class JsonResponseModel {
        List<KeyValuePair<string, string>> errors;
        public JsonResponseModel() {
            errors = new List<KeyValuePair<string, string>>();
        }

        public bool Result { get; set; }

        public List<KeyValuePair<string, string>> Errors {
            get {
                return errors;
            }
        }

        public string RedirectUrl { get; set; }

        public object Data { get; set; }

        public string Message { get; set; }

        public void AddError(string key, string message) {
            errors.Add(new KeyValuePair<string, string>(key, message));
        }

        public void AddError(string message) {
            errors.Add(new KeyValuePair<string, string>(String.Empty, message));
        }

        
    }// class

}// namespace
