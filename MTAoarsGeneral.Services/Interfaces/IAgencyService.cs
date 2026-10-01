using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Enquiries;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IAgencyService : IBaseService {
        
        bool Terminate(long principalId,string remarks, DateTime? scheduledOn, bool isAutoTerminate, bool isBatchProcess = false);
        bool Terminate(long principalId, DateTime?  scheduledOn, bool isAutoTerminate, bool isBatchProcess = false);

        bool Renew(long principalId, bool isBatchProcess = false);

        bool NotRelease(long principalId,string remarks, DateTime? scheduledOn, bool isBatchProcess = false);
        bool NotRelease(long principalId,DateTime? scheduledOn, bool isBatchProcess = false);

        bool Resign(long principalId, string remarks, DateTime? scheduledOn, bool isBatchProcess = false);
        bool Resign(long principalId,  DateTime? scheduledOn, bool isBatchProcess = false);

        List<SearchResultViewModel> Enquiry(SearchViewModel search);

    }// interface
}// 
