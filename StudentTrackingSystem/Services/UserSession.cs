#region Kütüphane Referansları
using System;                                                                       // Temel sistem fonksiyonları ve hata yönetimi için gereklidir.
using System.Collections.Generic;                                                   // Liste ve koleksiyon tabanlı veri yapıları için kullanılır.
using System.Linq;                                                                  // Veriler üzerinde sorgulama ve filtreleme yapmayı sağlar.
using System.Text;                                                                  // Yazı karakterlerini ve metin işlemlerini yönetir.
using System.Threading.Tasks;                                                       // Arka plan işlemleri ve görev yönetimi için kullanılır.
#endregion                                                                          // Referanslar bölümünün bitişi.

namespace StudentTrackingSystem.Services                                            // Bu dosyanın projedeki servis katmanı içinde olduğunu belirtir.
{                                                                                   // İsim uzayı başlangıç ayracı.
    #region Oturum Yönetim Merkezi
    public static class UserSession                                                 // Kullanıcı bilgilerini uygulama boyunca hafızada tutan sınıftır.
    {                                                                               // Sınıf gövdesinin başlangıç ayracı.
        #region Kullanıcı Bilgileri
        private static int _userId;                                                 // Kullanıcı ID'sini güvenli şekilde saklayan gizli değişken.
        private static string _fullName = "Kullanıcı";                                // Kullanıcı ismini saklayan ve varsayılan değer atanan değişken.
        private static int? _unitId;                                                // Bağlı olunan birim (şube/rol) bilgisini saklayan değişken.
        private static int? _serviceId;                                             // Şoförler için atanan servis ID bilgisini saklayan değişken.
        #endregion                                                                  // Bilgi saklama bölümünün sonu.

        #region Erişim Özellikleri (Properties)

        public static int UserId                                                    // Kullanıcı ID'sine diğer sayfalardan ulaşmayı sağlayan kapı.
        {                                                                           // Özellik başlangıç ayracı.
            get                                                                     // Değer okuma işlemi başladığında.
            {                                                                       // Getter başlangıcı.
                try { return _userId; }                                             // Saklanan kullanıcı numarasını geri döndürür.
                catch { return 0; }                                                 // Hata olursa uygulama çökmesin diye 0 değerini döner.
            }                                                                       // Getter bitişi.
            set                                                                     // Değer atama işlemi başladığında.
            {                                                                       // Setter başlangıcı.
                try { _userId = value; }                                            // Gelen ID değerini güvenli bir şekilde gizli değişkene yazar.
                catch { /**/ }                                                      // Atama hatasında sessiz kalarak stabiliteyi korur.
            }                                                                       // Setter bitişi.
        }                                                                           // Özellik bitiş ayracı.

        public static string FullName                                               // Kullanıcı ismine her yerden ulaşmayı sağlayan kapı.
        {                                                                           // Özellik başlangıç ayracı.
            get                                                                     // İsim bilgisi istendiğinde.
            {                                                                       // Getter başlangıcı.
                try { return _fullName; }                                           // Kayıtlı olan isim ve soyisim bilgisini döndürür.
                catch { return "Kullanıcı"; }                                       // Hata anında ekranın boş kalmaması için varsayılan ismi döner.
            }                                                                       // Getter bitişi.
            set                                                                     // İsim değiştirilmek veya atanmak istendiğinde.
            {                                                                       // Setter başlangıcı.
                try { _fullName = string.IsNullOrEmpty(value) ? "Kullanıcı" : value; } // Boş değer gelirse "Kullanıcı" yazar, doluysa değeri atar.
                catch { /**/ }                                                      // Hata durumunda işlemi iptal eder ve çökmeyi engeller.
            }                                                                       // Setter bitişi.
        }                                                                           // Özellik bitiş ayracı.

        public static int? UnitId                                                   // Birim (Rol/Şube) ID'sine erişim sağlayan özellik.
        {                                                                           // Özellik başlangıç ayracı.
            get                                                                     // Değer okuma işlemi.
            {                                                                       // Getter başlangıcı.
                try { return _unitId; }                                             // Saklanan birim ID'sini döndürür.
                catch { return null; }                                              // Hata durumunda boş değer döner.
            }                                                                       // Getter bitişi.
            set                                                                     // Değer atama işlemi.
            {                                                                       // Setter başlangıcı.
                try { _unitId = value; }                                            // Değeri gizli değişkene atar.
                catch { /**/ }                                                      // Hatayı sessizce geçiştirir.
            }                                                                       // Setter bitişi.
        }                                                                           // Özellik bitiş ayracı.

        public static int? ServiceId                                                // Sorumlu olunan servis ID'sine erişim sağlayan özellik.
        {                                                                           // Özellik başlangıç ayracı.
            get                                                                     // Değer okuma işlemi.
            {                                                                       // Getter başlangıcı.
                try { return _serviceId; }                                          // Şoförün servis ID'sini döndürür.
                catch { return null; }                                              // Hata durumunda boş döner.
            }                                                                       // Getter bitişi.
            set                                                                     // Değer atama işlemi.
            {                                                                       // Setter başlangıcı.
                try { _serviceId = value; }                                         // Gelen servis ID'sini atar.
                catch { /**/ }                                                      // Hatayı sessizce yönetir.
            }                                                                       // Setter bitişi.
        }                                                                           // Özellik bitiş ayracı.

        #endregion                                                                  // Erişim özellikleri bölümünün sonu.
    }                                                                               // Sınıf bitiş ayracı.
    #endregion                                                                      // Ana bölge bitişi.
}                                                                                   // İsim uzayı bitiş ayracı.