using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.DomainModels {
    public interface ILookupEntity : IEntity {
        string Code { get; set; }
        string Description { get; set; }
        bool IsActive { get; set; }
    }// interface
}// 
