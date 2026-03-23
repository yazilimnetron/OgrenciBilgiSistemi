namespace OgrenciBilgiSistemi.Api.Models
{
    public class ServisProfilModel
    {
        public int KullaniciId { get; set; }
        public string Plaka { get; set; } = string.Empty;
        public string? SoforTelefon { get; set; }
        public bool ServisDurum { get; set; }
    }
}
