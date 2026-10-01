using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Repositories.Interfaces;

namespace MTAoarsGeneral.Validators.Maintenance
{
    class ReferredDetailUniqueValidator : IValidator<ReferredDetailViewModel>
    {
        IReferredRepository referredRepository;
        public ReferredDetailUniqueValidator(IReferredRepository referredRepository)
        {
            this.referredRepository = referredRepository;
        }

        public IEnumerable<ValidationMessage> Validate(ReferredDetailViewModel item)
        {
            if (item.ID > 0) yield break;

            var detail = referredRepository.GetDetailByCompany(item.ICNumber, item.HeaderID, item.Reason.ID);
            //var detail = referredRepository.GetDetail(item.ICNumber);            
            if (detail == null) yield break;
            if (detail.AllowForRegistration && !detail.IsActive) yield break; //[2018-12-06]

            yield return new ValidationMessage("", "The details for this referred member already exist with the status {0}", detail.LookupReferredDetailStatu.Description);
        }

    }// class
}// namespace
