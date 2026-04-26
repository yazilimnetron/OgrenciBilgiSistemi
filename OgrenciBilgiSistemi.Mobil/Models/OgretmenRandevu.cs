namespace OgrenciBilgiSistemi.Mobil.Models
{
    public class OgretmenRandevu
    {
        public int OgretmenRandevuId { get; set; }
        public int OgretmenKullaniciId { get; set; }
        public DateTime Tarih { get; set; }
        public string BaslangicSaati { get; set; } = string.Empty;
        public string BitisSaati { get; set; } = string.Empty;

        public string TarihMetni => Tarih.ToString("dd.MM.yyyy");
    }
}
