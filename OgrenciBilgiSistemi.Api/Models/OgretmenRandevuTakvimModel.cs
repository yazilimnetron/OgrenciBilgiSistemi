namespace OgrenciBilgiSistemi.Api.Models
{
    public class OgretmenRandevuTakvimModel
    {
        public int OgretmenRandevuId { get; set; }
        public int OgretmenKullaniciId { get; set; }
        public DateTime Tarih { get; set; }
        public string BaslangicSaati { get; set; } = string.Empty;
        public string BitisSaati { get; set; } = string.Empty;
    }
}
