using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface INotificaitonService {

        bool Notify(long fromCompanyId, long toCompanyId, string notificationCode, INotificationVariableSource variableSource);

    }// interface
}// class
