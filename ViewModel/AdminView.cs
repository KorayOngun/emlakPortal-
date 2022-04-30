using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emlakPortalı.ViewModel
{
    public class AdminView
    {
        public int adminId { get; set; }
        public string adminMail { get; set; }
        public string adminPassword { get; set; }
        public string adminToken { get; set; }
        
        public string newadminMail { get; set; }
        public string newadminPassword { get; set; }

      
    }
}