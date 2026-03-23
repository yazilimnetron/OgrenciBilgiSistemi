namespace OgrenciBilgiSistemi.Api.Dtos
{
    // Servis yoklama toplu kaydetme isteği için kullanılan model
    public class ServisYoklamaKaydetDto
    {
        public int KullaniciId { get; set; }
        public int Periyot { get; set; } // 1 = Sabah, 2 = Akşam
        public List<YoklamaKayitOgesiDto> Kayitlar { get; set; } = new();
    }
}
