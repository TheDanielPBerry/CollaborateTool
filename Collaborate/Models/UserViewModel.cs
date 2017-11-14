using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Collaborate.Models
{
    public class UserViewModel
    {

        public String Email { get; set; }

        [DisplayName("Username")]
        public String UserName { get; set; }

        public String Password { get; set; }


        [DisplayName("Confirm Password")]
        public String PasswordConfirm { get; set; }


        


    }
}