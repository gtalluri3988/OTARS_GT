using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Administrative;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Shared;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IRenewalService : IBaseService {

        void GenerateRenewableRecords();

        void Save(RenewalHeaderViewModel model);

        void Submit(RenewalHeaderViewModel model);

        void SendReminders();

        void Process();

        void Accept(long headerId);

        void Reject(long headerId);

        RenewalPageViewModel GetRenwalDetails(GridPageViewModel model, long headerId, bool isSubmitted);

        RenewalDetailsViewModel GetItemsRenewalDetails(GridPageViewModel model, long headerId, bool isSubmitted);

        void SaveRenewalDetails(RenewalDetailsViewModel model);

        void SubmitRenewalDetails(RenewalDetailsViewModel model);

        void SaveAdminstrativeRenewalDetails(RenewalDetailsViewModel model);

        //void UpdateRenewalDetails(RenewalDetailsViewModel model, string statusCode);
    }// interface
}// 
