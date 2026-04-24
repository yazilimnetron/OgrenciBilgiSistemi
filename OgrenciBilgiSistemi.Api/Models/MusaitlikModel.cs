namespace OgrenciBilgiSistemi.Api.Models
{
    public class MusaitlikModel
    {
        public int MusaitlikId { get; set; }
        public int OgretmenKullaniciId { get; set; }
        public int Gun { get; set; }
        public string GunAdi { get; set; } = string.Empty;
        public string BaslangicSaati { get; set; } = string.Empty;
        public string BitisSaati { get; set; } = string.Empty;
    }
}
