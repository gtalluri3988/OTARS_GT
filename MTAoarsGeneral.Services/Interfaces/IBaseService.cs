using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IBaseService {

        ServiceContext CurrentContext { get; }
    }
}
