namespace OgrenciBilgiSistemi.Api.Models
{
    public class MusaitSlotModel
    {
        public DateTime Tarih { get; set; }
        public string BaslangicSaati { get; set; } = string.Empty;
        public string BitisSaati { get; set; } = string.Empty;
        public int OgretmenKullaniciId { get; set; }
    }
}
