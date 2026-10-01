using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using System.Data;


namespace MTAoarsGeneral.Repositories.Interfaces {
    public interface IReportRepository {

        Dictionary<string, IEnumerable<dynamic>> GetData(Dictionary<string, string> parameters);

        Dictionary<string, string> GetParameters(Dictionary<string, string> parameters);

    }// interface
}// namespace
