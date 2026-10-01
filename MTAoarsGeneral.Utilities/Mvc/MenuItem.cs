using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Mvc {
    [Serializable]
    public class MenuItem {

        public MenuItem() {
            Menus = new List<MenuItem>();
        }

        public long ID { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public List<MenuItem> Menus { get; set; }
    }// class

}// namespace
