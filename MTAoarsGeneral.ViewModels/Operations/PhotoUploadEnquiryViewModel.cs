using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Operations
{
    public class PhotoUploadEnquirySearchViewModel
    {
        public string AgencyNumber { get; set; }
        public List<PhotoUploadEnquiryResponseViewModel> Results;
        public string Message { get; set; }
    }

    public class PhotoUploadEnquiryResponseViewModel
    {
        public string AgencyNumber { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
