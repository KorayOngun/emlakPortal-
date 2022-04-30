using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emlakPortalı.ViewModel
{
    public class AdminIlan
    {
        public int adminId { get; set; }
        public string adminMail { get; set; }
        public string adminPassword { get; set; }
        public string adminToken { get; set; }
        public int ilanId { get; set; }
        public int userId { get; set; }

        public string ilandaMiAdmin { get; set; }
    }
}