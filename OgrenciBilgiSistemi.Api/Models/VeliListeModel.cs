namespace OgrenciBilgiSistemi.Api.Models
{
    public class VeliListeModel
    {
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string? VeliAdSoyad { get; set; }
        public string? VeliTelefon { get; set; }
    }
}
