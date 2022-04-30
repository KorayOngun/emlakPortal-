using emlakPortalı.Models;
using emlakPortalı.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace emlakPortalı.Controllers
{
    // owin ile oturum açma eklenmemiştir
    // oturum açma işleminde token belirlenip veritbanı güncellenir
    // oturum doğrulama gereken her işlemde önce oturum doğrulama yapılır (gereken modellerde mail password token verisi var ) sonuca göre işlem yapılır (orn=361:390 satırlar)
    public class servisController : ApiController
    {
        Database1Entities1 db = new Database1Entities1();
        Sonuc s = new Sonuc();

        [HttpPost]
        [Route("api/yenikayit")]
        public Sonuc YeniKayit(UserView user)
        {
            if (db.Users.Where(x=>x.userName==user.userName || x.userMail == user.userMail).Count() < 1)// aynı kullanıcı adın/mail veritabanında kayıtlı mı?
            {
                try
                {
                    Users u = new Users
                    {
                        userMail = user.userMail,
                        userName = user.userName,
                        userPassword = user.userPassword,
                        token = ""
                    };
                    db.Users.Add(u);
                    db.SaveChanges();

                    s.mesaj = "islem tamam";
                    s.sonuc = true;
                }
                catch (Exception)
                {
                    s.mesaj = "hatalı islem ";
                    s.sonuc = false;
                }
            }
            else
            {
                s.mesaj = "farklı kullanıcı adı/mail deneyin  ";
                s.sonuc = false;
            }
            return s;
        }

       

        [HttpPost]
        [Route("api/hesapkurtarma")]
        public string HesapKurtarma(UserView u)
        {
            Random random = new Random();
            string dogrulama = "";
            for (int i = 0; i < 10; i++)
            {
                dogrulama = dogrulama + random.Next(0, 10).ToString();
            }
            try
            {
               
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("a@hotmail.com");//a@hotmail.com
                message.To.Add(new MailAddress(u.userMail.ToString()));
                message.Subject = "Kurtarma Kodunu başkalarıyla paylaşmayın";
                message.Body = dogrulama;
                smtp.Port = 587;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("a@hotmail.com","a@hotmail.com sifresi");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return "tamamlandı";
            }
            catch (Exception)
            {
               
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress("a@hotmail.com");
                    message.To.Add(new MailAddress(u.userMail.ToString()));
                    message.Subject = "Kurtarma Kodunu başkalarıyla paylaşmayın";
                    message.Body = dogrulama;
                    smtp.Port = 587;
                    smtp.Host = "	smtp.live.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("a@hotmail.com", "a@hotmail.com sifresi");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                    return "tamamlandı";
                }
                catch (Exception)
                {
                    return "hata";
                }
            }
            
        }


        #region mesaj islemleri
        [HttpPost]
        [Route("api/mesajgonder")]
        public Sonuc MesajGonder(MesajView mesaj)
        {
            if (db.Users.Where(x=>x.token==mesaj.token&&x.userMail==mesaj.userMail&&x.userPassword==mesaj.userPassword&&x.userId==mesaj.userId).Count()>0)//oturum dogrulama islemi
            {
                try
                {
                    db.Message.Add(new Message
                    {
                        mesaj = mesaj.mesaj,
                        gonderen = mesaj.gonderen,
                        alan = mesaj.alan,
                        tarih = mesaj.tarih
                    });
                    db.SaveChanges();
                    s.mesaj = "işlem tamam";
                    s.sonuc = true;

                }
                catch (Exception)
                {
                    s.mesaj = "hatalı işlem";
                    s.sonuc = false;
                }
            }
            else
            {
                s.mesaj = "oturum açma hatası";
                s.sonuc = false;
            }
            
            return s;
        }
        [HttpPost]
        [Route("api/mesajcek")]
        public List<MesajView> MesajlarıCek(Users users)
        {
            if (db.Users.Where(x => x.token == users.token && x.userMail == users.userMail && x.userPassword == users.userPassword).Count() > 0)
            {
                return db.Message.Where(x => x.alan == users.userName || x.gonderen == users.userName)
               .Select(x => new MesajView
               {
                   gonderen = x.gonderen
               }).ToList();
            }
            return null;
               
        }
        #endregion
        #region admin islemleri
        [HttpPost]
        [Route("api/adminekle")]
        public Sonuc NewAdmin(AdminView admin)
        {
            try
            {
                if (db.Admins.Where(x => x.adminId == 1 && x.adminMail == admin.adminMail && x.adminPassword == admin.adminPassword && x.adminToken == admin.adminToken).Count() > 0)//sadece 1. admin başka hesapları yetkilendirebilir
                {
                    Admins a = new Admins
                    {
                        adminMail = admin.newadminMail,
                        adminPassword = admin.newadminPassword,
                    };
                    if (db.Admins.Where(x => x.adminMail == admin.newadminMail).Count() < 1)
                    {
                        db.Admins.Add(a);
                        s.mesaj = "işlem başarılı";
                        s.sonuc = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        s.mesaj = "bu mail kullanımda";
                        s.sonuc = false;
                    }

                }
                else
                {
                    s.mesaj = "işlem başarısız";
                    s.sonuc = false;
                }
            }
            catch (Exception)
            {
                s.mesaj = "hata!!!";
                s.sonuc = false;
            }
            return s;
        }
        [HttpPost]
        [Route("api/sohbetgoruntule")]
        public List<MesajView> SohbetGoruntule(MesajView mesaj)
        {
            if (db.Users.Where(x => x.token == mesaj.token && x.userMail == mesaj.userMail && x.userPassword == mesaj.userPassword && x.userId==mesaj.userId).Count() > 0)
            {
                return db.Message.Where(x => x.alan == mesaj.alan && x.gonderen == mesaj.gonderen || x.alan == mesaj.gonderen && x.gonderen == mesaj.alan)
               .Select(x => new MesajView
               {
                   gonderen = x.gonderen,
                   alan = x.alan,
                   mesaj = x.mesaj,
                   tarih = x.tarih,
                   messageId = x.messageId
               }).ToList();
            }
            return null;
        }
        
       
        [HttpPost]
        [Route("api/adminsil")]
        public Sonuc AdminSil(AdminView admin)
        {
            try
            {
                if (db.Admins.Where(x => x.adminId == 1 && x.adminMail == admin.adminMail && x.adminPassword == admin.adminPassword && x.adminToken == admin.adminToken).Count() > 0)
                {
                    db.Admins.Remove(new Admins { adminMail = admin.newadminMail });
                    s.mesaj = "işlem başarılı";
                    s.sonuc = true;
                }
                else
                {
                    s.mesaj = "işlem başarısız";
                    s.sonuc = false;
                }
            }
            catch (Exception)
            {
                s.mesaj = "hata!!!";
                s.sonuc = false;
            }
            return s;
        }
       
        [HttpPost]
        [Route("api/adminmesajkullanıcı")]
        public Sonuc AdminMesajKullanici(AdminUser admin)
        {
            try
            {
                if (db.Admins.Where(x => x.adminId == admin.adminId && x.adminMail == admin.adminMail && x.adminPassword == admin.adminPassword && x.adminToken == admin.adminToken).Count() > 0)
                {
                    var kayit = db.Users.Where(x=>x.userId==admin.userId).First();
                    kayit.adminIslem = admin.adminIslem;
                    kayit.adminMessge = admin.adminMessge;
                    s.mesaj = "işlem başarılı";
                    s.sonuc = true;
                    db.SaveChanges();
                    return s;
                }
                else
                {
                    s.mesaj = "işlem başarısız";
                    s.sonuc = false;
                }
            }
            catch (Exception)
            {
                s.mesaj = "hata!!!";
                s.sonuc = false;
            }
            return s;
        }
        #endregion
        #region ilanlar
        [HttpGet]
        [Route("api/ilanarama/{arama}")]
        public List<ilanView> IlanArama(string arama="")
        {
            return db.Ilan.Where(x => x.aciklama.Contains(arama) || x.baslik.Contains(arama)).Select(x => new ilanView {
                aciklama = x.aciklama,
                baslik = x.baslik,
                resimler = x.resimler,
                ilanId = x.ilanId
            }).ToList();
        }
        [HttpPost]
        [Route("api/ilanaramaCat/")]
        public List<ilanView> IlanAramaCat(ilanCat ilan)
        {
          return  db.Ilan.Where(x => x.durum==ilan.durum && Convert.ToInt32(x.fiyat) > Convert.ToInt32(ilan.fiyatmin) && Convert.ToInt32(x.fiyat) < Convert.ToInt32(ilan.fiyatmax) && x.kat == ilan.kat && x.sehir == ilan.sehir)
                  .Select(x => new ilanView
                  {
                      aciklama = x.aciklama,
                      baslik = x.baslik,
                      cepheler = x.cepheler,
                      ilanTipi = x.ilanTipi,
                      ozellikler = x.ozellikler,
                      resimler = x.resimler,
                      muhit = x.muhit,
                      fiyat = x.fiyat,
                      durum = x.durum,
                      sehir = x.sehir,
                      kat = x.kat
                  }).ToList();
        }
        [HttpPost]
        [Route("api/adminilanduzenle")]
        public Sonuc IlanDuzenle(AdminIlan admin)
        {
            try
            {
                if (db.Admins.Where(x => x.adminId == 1 && x.adminMail == admin.adminMail && x.adminPassword == admin.adminPassword && x.adminToken == admin.adminToken).Count() > 0)
                {
                    var ılan = db.Ilan.Where(x => x.ilanId == admin.ilanId).First();
                    ılan.ilandaMiAdmin = admin.ilandaMiAdmin;
                    db.SaveChanges();
                }
                else
                {
                    s.mesaj = "işlem başarısız";
                    s.sonuc = false;
                }
            }
            catch (Exception)
            {
                s.mesaj = "hata!!!";
                s.sonuc = false;
            }
            return s;
        }
        [HttpPost]
        [Route("api/ilansil")]
        public Sonuc IlanSil(IlanKayitView ılanKayit)
        {
            if (db.Users.Where(x => x.userId == ılanKayit.userId && x.userMail == ılanKayit.userMail && x.token == ılanKayit.token && x.userPassword == ılanKayit.userPassword).Count() > 0)
            {
                var ılan = db.Ilan.Where(x => x.ilanId == ılanKayit.ilanId && x.userId == ılanKayit.userId).First();
                db.Ilan.Remove(ılan);
                s.mesaj = "işlem Tamam";
                s.sonuc = true;
                return s;
            }
            s.mesaj = "hata!!!";
            s.sonuc = false;
            return s;
        }
        [HttpPost]
        [Route("api/ilanduzenle")]
        public Sonuc IlanDuzenle(IlanKayitView ılanKayit)
        {
            
            if (db.Users.Where(x => x.userId == ılanKayit.userId && x.userMail == ılanKayit.userMail && x.token == ılanKayit.token && x.userPassword == ılanKayit.userPassword).Count() > 0)
            {
                try
                {
                    var ılan = db.Ilan.Where(x => x.ilanId == ılanKayit.ilanId).First();
                    ılan.aciklama = ılanKayit.aciklama;
                    ılan.baslik = ılanKayit.baslik;
                    ılan.cepheler = ılanKayit.cepheler;
                    ılan.ozellikler = ılanKayit.ozellikler;
                    ılan.ilanTipi = ılanKayit.ilanTipi;
                    ılan.muhit = ılanKayit.muhit;
                    ılan.resimler = ılanKayit.resimler;
                    db.SaveChanges();
                    s.mesaj = "işlem Tamam";
                    s.sonuc = true;
                    return s;
                }
                catch (Exception)
                {

                    s.mesaj = "güncellem sorunu. Daha sonra tekrar deneyin.";
                    s.sonuc = false;
                    return s;
                }
             
            }
            else
            {
                s.mesaj = "oturum doğrulama hatası";
                s.sonuc = false;
                return s;
            }
        }
        [HttpPost]
        [Route("api/ilankayitbyuserid")]
        public List<ilanView> IlanbyUserId(Users us)
        {
            if (db.Users.Where(x => x.userId == us.userId && x.userPassword == x.userPassword && x.token == us.token && x.userMail == us.userMail).Count() > 0)
            {
                return db.Ilan.Where(x => x.userId == us.userId).Select(x => new ilanView
                {
                    aciklama = x.aciklama,
                    baslik = x.baslik,
                    cepheler = x.cepheler,
                    ilanTipi = x.ilanTipi,
                    ozellikler = x.ozellikler,
                    resimler = x.resimler,
                    muhit = x.muhit,
                    fiyat = x.fiyat,
                    durum = x.durum,
                    sehir = x.sehir,
                    kat = x.kat
                }).ToList();
            }
            return null;
        }
        [HttpPost]
        [Route("api/ilanyeni")]
        public Sonuc YeniIlan(IlanKayitView ilan)
        {

            if (db.Users.Where(x => x.token == ilan.token && x.userMail == ilan.userMail && x.userPassword == ilan.userPassword).Count() > 0)
            {
                Ilan i = new Ilan
                {
                    aciklama = ilan.aciklama,
                    baslik = ilan.baslik,
                    cepheler = ilan.cepheler,
                    ilanTipi = ilan.ilanTipi,
                    muhit = ilan.muhit,
                    ozellikler = ilan.ozellikler,
                    resimler = ilan.resimler,
                    userId = ilan.userId,

                };
                db.Ilan.Add(i);
                s.mesaj = "kayıt başarılı";
                s.sonuc = true;
                db.SaveChanges();
            }
            else
            {
                s.mesaj = "hata!!!";
                s.sonuc = false;
            }

            return s;
        }
        [HttpGet]
        [Route("api/ilankayitbyid/{id}")]
        public ilanView IlanbyId(int id)
        {
            return db.Ilan.Where(x => x.ilanId == id).Select(x => new ilanView
            {
                aciklama = x.aciklama,
                baslik = x.baslik,
                cepheler = x.cepheler,
                ilanTipi = x.ilanTipi,
                ozellikler = x.ozellikler,
                resimler = x.resimler,
                muhit = x.muhit,
                fiyat = x.fiyat,
                durum = x.durum,
                sehir = x.sehir,
                kat = x.kat
            }).FirstOrDefault();
        }
        #endregion

    }
}
