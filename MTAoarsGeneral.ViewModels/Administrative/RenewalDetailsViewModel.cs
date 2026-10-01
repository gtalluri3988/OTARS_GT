using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Administrative
{
    [Serializable]
    public class RenewalDetailsViewModel
    {
        public int ID { get; set; }
        public string AgencyNumber { get; set; }
        public string CompanyName { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public string Status { get; set; }
        public List<CompanyInRenewalHeaderViewModel> Companies {get;set;}
        public List<RenewalHearderViewModel> RenewalHeader { get; set; }
        public RenewalHearderViewModel Header { get; set; }
        [Dependency]
        [Binder(typeof(JsonModelBinder))]
        public List<RenewalDetailViewModel> RenewalDetail { get; set; }
        public RenewalDetailViewModel RenewalDetailModel { get; set; }
        public int TotalRecord { get; set; }
    }
    
    public class CompanyInRenewalHeaderViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string BusinessRegNo { get; set; }
    }

    public class RenewalHearderViewModel
    {
        public int ID { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public IEnumerable<LookupItem> RenewalDetailStatuses { get; set; }
        public string CompanyName { get; set; }
    }
    
}
