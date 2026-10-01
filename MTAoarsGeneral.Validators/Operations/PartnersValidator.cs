using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Validators.Operations {

    public class PartnersValidator  : IValidator<PartnershipRegistrationViewModel> {

        public IEnumerable<ValidationMessage> Validate(PartnershipRegistrationViewModel item) {
            var checkingNumber = String.IsNullOrWhiteSpace(item.CorporateNominee.NewICNumber)?item.CorporateNominee.PassportNumber : item.CorporateNominee.NewICNumber;
            if (item.Partners.Count(p => p.ICNumber == checkingNumber) > 0) {
                yield return new ValidationMessage("", "Corporate nominee will be added automatically added as a partner, please delete the partner with IC number {0}", checkingNumber);
            }
            foreach (var partner in item.Partners) {
                if (String.IsNullOrEmpty(partner.ICNumber)) continue;
                var count = item.Partners.Count(p => p.ICNumber == partner.ICNumber);
                if (count > 1) {
                    yield return new ValidationMessage("", "Partner with IC Number {0} appears {1} times. Remove all except one", partner.ICNumber, count.ToString());
                }
            }
            
        }

    }// class
}// namespace
