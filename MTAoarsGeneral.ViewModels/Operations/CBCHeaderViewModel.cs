using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Operations {
    
    public class CBCHeaderViewModel {

        public long ID { set; get; }

        public LookupItem Company { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public LookupItem Status { get; set; }
    }// class

}// namespace


