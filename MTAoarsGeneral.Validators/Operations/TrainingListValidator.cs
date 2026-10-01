using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Masters;

namespace MTAoarsGeneral.Validators.Operations {
    public class TrainingListValidator : IValidator<TrainingListViewModel> {
        ILookupRepository lookupRepository;

        public TrainingListValidator(ILookupRepository lookupRepository) {
            this.lookupRepository = lookupRepository;
        }

        public IEnumerable<ValidationMessage> Validate(TrainingListViewModel item) {
            if (item.TrainingModel.StartDate.Value.Year != item.Year) {
                yield return new ValidationMessage("", "Year for the start date should match with the selected year {0}", item.Year.ToString());
            }
            if (item.TrainingModel.EndDate.Value.Year != item.Year) {
                yield return new ValidationMessage("", "Year for the start date should match with the selected year {0}", item.Year.ToString());
            }
            
            var diff = (item.TrainingModel.EndDate.Value - item.TrainingModel.StartDate.Value).TotalDays + 1;
            var tt = lookupRepository.Get<LookupTrainingType>(item.TrainingModel.TrainingTypeID.Value);
            if (tt.Code == LookupConstants.TrainingType.ProfessionalQualification && item.TrainingModel.CreditHours != 8) {
                yield return new ValidationMessage("", "Achievement of Professional Qualification CPD credit hours should be 8");
            }
            if (item.TrainingModel.CreditHours > diff * tt.HoursPerDay) {
                yield return new ValidationMessage("", "Maximum hours per day is allowed for {0} is {1}", tt.Description, tt.HoursPerDay.ToString());
            }
            

        }

    }// class
}// namespace
