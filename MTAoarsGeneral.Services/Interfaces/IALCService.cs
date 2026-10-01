using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IALCService : IBaseService {
        void Save(ALCHeaderViewModel input);

    }// interface
}// 
