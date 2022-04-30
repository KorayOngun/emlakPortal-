using emlakPortalı.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emlakPortalı.ViewModel
{
    public class UserView
    {
        public int userId { get; set; }
        public string userMail { get; set; }
        public string userPassword { get; set; }
        public string token { get; set; }
        public string adminIslem { get; set; }
        public string adminMessge { get; set; }
        public string userName { get; set; }

  
    }
}