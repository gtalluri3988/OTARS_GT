using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;
using FluentValidation;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Validators.Shared.Fluent;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Reports;

namespace MTAoarsGeneral.Validators.Operations.Fluents {
    
    [FluentRegisterable]
    public class InvoiceReportValidator : AbstractValidator<InvoiceReportViewModel> {

        public InvoiceReportValidator() {
            RuleFor(x => x.Year).GreaterThan(0);
            RuleFor(x => x.Month).GreaterThan(0);
        }

    }// class
}// namespace
