using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Masters {
    public class RoleMasterViewModel {

        public long ID { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public byte[] RecordVersion { get; set; }

        public List<MenuItem> Menus { get; set; }

    }
}
