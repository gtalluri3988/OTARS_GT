using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Masters;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface ICPDService : IBaseService {
        void Process();
        bool Check(TrainingListViewModel model);
    }// interface
}// 
