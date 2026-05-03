namespace OgrenciBilgiSistemi.Mobil.Models
{
    public class Veli
    {
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string? VeliAdSoyad { get; set; }
        public string? VeliTelefon { get; set; }

        public string GorunenAd =>
            string.IsNullOrWhiteSpace(VeliAdSoyad) ? KullaniciAdi : VeliAdSoyad!;
    }
}
