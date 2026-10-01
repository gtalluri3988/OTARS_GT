using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.ViewModels.Administrative;

namespace MTAoarsGeneral.Services.Interfaces
{
    public interface IAdministrativeService : IBaseService
    {
        void UpdateAgency(AdministrativeAgentViewModel model, long principalId, bool isTerminated);
        AdministrativeAgentViewModel GetByPrincipal(long id);
        AdministrativeAgentViewModel GetByPrincipalHistory(long id);
        void InsertTBEResultToAuditTrail(TBEEnquirySearchViewModel model, string TableName);
        void InsertReferredToAuditTrail(ReferredSearchViewModel model);
        void InsertRenewalToAuditTrail(RenewalDetailsViewModel model);
    }// interface
}
