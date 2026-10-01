using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface ICBCService : IBaseService {
        void Process();

        CBCHeaderViewModel GetHeader(CBCStartViewModel model);

        CBCDetailViewModel Add(long headerId, string agencyNumber);

        void ChangeHeaderStatus(long headerId, string newStatusCode);

        bool Check(CBCStartViewModel model);

        
    }// interface
}// 
