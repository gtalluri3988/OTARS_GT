using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Maintenance;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IActivityService : IBaseService {

        void Save(AdminActivityViewModel model);

    }// interface
}// 
