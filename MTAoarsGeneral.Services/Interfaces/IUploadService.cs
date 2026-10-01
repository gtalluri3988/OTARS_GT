using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Maintenance;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IUploadService : IBaseService {

        void UploadIbfim(ExcelList<IbfimExcelViewModel> items);

        void UploadRegistration(ExcelList<RegistrationExcelViewModel> items);

        string GetHistoryErrorPath(long id);

        string GetHistorySuccessPath(long id);

        string GetHistoryRawPath(long id);

    }// interface
}// 
