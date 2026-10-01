using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Attributes;

namespace MTAoarsGeneral.Validators.Operations {
    public class PhotoValidator : IValidator<IExistableCompanyGuarantor> {
        ITbeExemptionRepository tbeExemptionRepository;
        IAgencyRepository agencyRepository;

        public PhotoValidator(IAgencyRepository agencyRepository, ITbeExemptionRepository tbeExemptionRepository)
        {
            this.agencyRepository = agencyRepository;
            this.tbeExemptionRepository = tbeExemptionRepository;
        }

        public IEnumerable<ValidationMessage> Validate(IExistableCompanyGuarantor item)
        {
            var propInfo = item.GetType().GetProperty("PhotoPath");
            var isIgnoreValidate = propInfo.GetCustomAttributes(typeof(IgnoreValidateAttribute), true).Length > 0;

            if (!isIgnoreValidate && string.IsNullOrEmpty(item.PhotoPath))
            {
                yield return new ValidationMessage("", "Please upload photograph");
            }
        }

    }// class
}// namespace
