#region Gerekli Kütüphanelerin Tanımlanması
using Microsoft.Extensions.Logging;                                         // Uygulama içi loglama ve hata takibi için gerekli kütüphane.
using CommunityToolkit.Maui;                                                // MAUI için hazır bileşenler ve yardımcı araçlar sunan toolkit kütüphanesi.
#endregion

namespace StudentTrackingSystem                                             // Uygulamanın ana isim uzayı.
{                                                                           // İsim uzayı kapsamı başlangıcı.
    public static class MauiProgram                                         // Uygulamanın temel yapılandırma sınıfı.
    {                                                                       // Sınıf gövdesi başlangıcı.

        #region Uygulama Oluşturma ve Yapılandırma Metodu
        public static MauiApp CreateMauiApp()                               // Uygulama örneğini döndüren statik metot.
        {                                                                   // Metot gövdesi başlangıcı.

            #region Builder (İnşa Edici) Başlatılması
            var builder = MauiApp.CreateBuilder();                          // MAUI uygulamasını yapılandırmak için builder nesnesi oluşturulur.
            #endregion

            #region MAUI ve Toolkit Kayıt İşlemleri
            builder                                                         // Yapılandırıcı nesne üzerinden devam edilir.
                .UseMauiApp<App>()                                          // Uygulamanın ana giriş sınıfı olarak 'App' sınıfı atanır.
                .UseMauiCommunityToolkit()                                  // Community Toolkit kütüphanesi uygulamaya entegre edilir.
            #endregion

            #region Yazı Tiplerinin (Fonts) Yapılandırılması
                .ConfigureFonts(fonts =>                                    // Uygulama genelinde kullanılacak fontlar tanımlanır.
                {                                                           // Font konfigürasyon bloğu başlangıcı.
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");  // Standart OpenSans fontu sisteme ve takma ismine kaydedilir.
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); // Yarı kalın OpenSans fontu sisteme ve takma ismine kaydedilir.
                });                                                         // Font konfigürasyon bloğu sonu.
            #endregion

            #region Hata Ayıklama (Debug) Günlüğü Yapılandırması
#if DEBUG                                                                   // Sadece Debug modunda derlenecek koşullu blok.
            builder.Logging.AddDebug();                                     // Geliştirme aşamasında çıktı (output) penceresine log yazdırılmasını sağlar.
#endif                                                                      // Koşullu blok sonu.
            #endregion

            #region Uygulamanın İnşası ve Döndürülmesi
            return builder.Build();                                         // Tüm yapılandırmalar tamamlanarak uygulama nesnesi oluşturulur ve döndürülür.
            #endregion
        }                                                                   // Metot gövdesi sonu.
        #endregion

    }                                                                       // Sınıf gövdesi sonu.
}                                                                           // İsim uzayı kapsamı sonu.