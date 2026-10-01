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
using MTAoarsGeneral.ViewModels.Maintenance;
using System.Text.RegularExpressions;

namespace MTAoarsGeneral.Validators.Maintenance {
    public class IbfimValidator : IValidator<IbfimExcelViewModel> {
        
        public IEnumerable<ValidationMessage> Validate(IbfimExcelViewModel item) {
            if(String.IsNullOrEmpty(item.ExaminationTypeCode) ||
                !Regex.IsMatch(item.ExaminationTypeCode, "[1-3]{1,2}")) {
                item.AddError("Examination type code is invalid");
            }
            if(String.IsNullOrEmpty(item.ICNO)) {
                item.AddError("IC Number is required");
            }
            if(String.IsNullOrEmpty(item.Result) || 
                !Regex.IsMatch(item.Result, "^(Pass|Fail|Absent)$")) {
                item.AddError("Invalid result it should be either Pass or Fail or Absent");
            }
            yield break;
        }

    }// class
}// namespace
