using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MTAoarsGeneral.Services.Interfaces {
    
    public interface IReportService {

        Dictionary<string, IEnumerable<dynamic>> GetDataSource(string key, Dictionary<string, string> parameters);

        Dictionary<string, string> GetReportParameters(string key, Dictionary<string, string> parameters);
    }// interface

}// namespace
