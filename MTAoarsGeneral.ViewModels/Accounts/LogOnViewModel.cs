using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MTAoarsGeneral.ViewModels.Accounts {
    public class LogOnViewModel {

        [Display(Name="User name")]
        [Required(ErrorMessage="User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string CaptchaKey { get; set; }
    }// class
}// namespace
