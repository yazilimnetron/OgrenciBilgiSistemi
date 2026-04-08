using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Shared.Enums
{
    public enum DonanimTipi : byte
    {
        UsbRfid = 1,

        ZKTeco = 2,

        QrOkuyucu = 3,

        [Display(Name = "Diğer")]
        Diger = 9
    }

    public enum IstasyonTipi : short
    {
        [Display(Name = "AnaKapı")]
        AnaKapi = 10,

        Yemekhane = 20,
    }

    // Öğrenci detay sayfası ve Excel çıktısı için rapor tipi seçimi
    public enum RaporTipi : byte
    {
        [Display(Name = "Tümü")]
        Tumu = 0,

        [Display(Name = "Ana Kapı Geçişleri")]
        AnaKapiGecisleri = 1,

        [Display(Name = "Yemekhane Geçişleri")]
        YemekhaneGecisleri = 2,

        [Display(Name = "Sınıf Yoklaması")]
        SinifYoklamasi = 3,

        [Display(Name = "Servis Yoklaması")]
        ServisYoklamasi = 4,
    }
}
