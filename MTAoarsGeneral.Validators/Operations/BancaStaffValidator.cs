using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Validators.Operations
{
    public class BancaStaffValidator : IValidator<RegistrationIndexViewModel>
    {
        IMemberRepository memberRepository;
        ILookupRepository lookupRepository;

        public BancaStaffValidator(IMemberRepository memberRepository, ILookupRepository lookupRepository)
        {
            this.memberRepository = memberRepository;
            this.lookupRepository = lookupRepository;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationIndexViewModel item)
        {
            //[20200724][CR No# URS-IT-OTARS-020] - if agent is active with Banca, agent cannot register as Non-Banca, and vice-versa
            var agencyPrincipals = memberRepository.GetPrincipals(item.ICNumber);
            foreach (var ap in agencyPrincipals)
            {
                var errorMessage = "This Agent \"{0}\" is currently registered as an {1} as {2}. Agency Number \"{3}\".";
                if (item.IsFamily)
                {
                    if (ap.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.Family 
                        && ap.IsBancaStaff != null && ap.IsBancaStaff != item.IsBancaStaff)
                    {
                        yield return new ValidationMessage("", errorMessage,
                            item.ICNumber,
                            ap.LookupIntermediaryType.Description,
                            Convert.ToBoolean(ap.IsBancaStaff) ? "Banca" : "Non Banca",
                            ap.AgencyNumber);
                    }
                }
                if (item.IsGeneral)
                {
                    if (ap.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General
                        && ap.IsBancaStaff != null && ap.IsBancaStaff != item.IsBancaStaff)
                    {
                        yield return new ValidationMessage("", errorMessage, 
                            item.ICNumber, 
                            ap.LookupIntermediaryType.Description,
                            Convert.ToBoolean(ap.IsBancaStaff) ? "Banca" : "Non Banca",
                            ap.AgencyNumber);
                    }
                }
            }

            yield break;
        }
    }
}
