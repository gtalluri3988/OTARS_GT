using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Shared {
    
    public class PhotoViewModel {

        [Display(Name="Photograph")]
        public string PhotoPath { get; set; }

    }// class

}// namespace
