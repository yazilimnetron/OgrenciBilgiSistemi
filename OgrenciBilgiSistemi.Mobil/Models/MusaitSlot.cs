namespace OgrenciBilgiSistemi.Mobil.Models
{
    public class MusaitSlot
    {
        public DateTime Tarih { get; set; }
        public string BaslangicSaati { get; set; } = string.Empty;
        public string BitisSaati { get; set; } = string.Empty;
        public int OgretmenKullaniciId { get; set; }

        public string GosterimMetni => $"{Tarih:dd.MM.yyyy} {BaslangicSaati} - {BitisSaati}";
    }
}
