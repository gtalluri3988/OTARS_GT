using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class TBEResultViewModel
    {
        [Binder(typeof(JsonModelBinder))]
        [Dependency]
        public TBEEnquirySearchViewModel TBEResult { get; set; }

    }
}
