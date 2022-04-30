using emlakPortalı.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emlakPortalı.ViewModel
{
    public class ilanView
    {
        public int ilanId { get; set; }
        public  int  userId { get; set; }

        public string baslik { get; set; }
        public string aciklama { get; set; }
        public string ilanTipi { get; set; }
        public string muhit { get; set; }
        public string ozellikler { get; set; }
        public string cepheler { get; set; }
        public string ilandaMiAdmin { get; set; }
        public string ilandaMiKullanici { get; set; }
        public string resimler { get; set; }
        public string fiyat { get; set; }
        public string durum { get; set; }
        public string kat { get; set; }
        public string sehir { get; set; }
    }
}