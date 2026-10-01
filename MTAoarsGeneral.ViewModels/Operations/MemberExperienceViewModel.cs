using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Operations {

    public class MemberExperienceViewModel {

        public long CompanyID { get; set; }

        public string AgencyCode { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

    }

}
