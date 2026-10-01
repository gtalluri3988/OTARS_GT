using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Mvc {
    [Serializable]
    public class LookupItem {

        public long ID { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }// class

}// namespace
