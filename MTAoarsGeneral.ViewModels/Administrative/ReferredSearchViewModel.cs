using MTAoarsGeneral.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Administrative
{
    [Serializable]
    public class ReferredSearchViewModel
    {
        public string IcNumber { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string Category { get; set; }

        public List<ReferredResponseViewModel> ReferredMembers { get; set; }
        public ReferredResponseViewModel ReferredMember { get; set; }
    }

    public class ReferredResponseViewModel
    {
        public int ID { get; set; }
        public string ICNumber {get;set;}
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int ReasonID { get; set; }
        public int CategoryID { get; set; }
        public string ReasonDescription { get; set; }
        public string CategoryDescription { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedDateText
        {
            get
            {
                return (!this.CreatedDate.HasValue) ? "" : Convert.ToDateTime(this.CreatedDate).ToString(GlobalConstants.DateFormat);
            }
        }
    }

    //public class ReferredReasonViewModel
    //{
    //    public int ID { get; set; }
    //    public string Code { get; set; }
    //    public string Description { get; set; }
    //    public int Category { get; set; }
    //}

    //public class ReferredCategoryViewModel
    //{
    //    public int ID { get; set; }
    //    public string Code { get; set; }
    //    public string Description { get; set; }
    //}
}
