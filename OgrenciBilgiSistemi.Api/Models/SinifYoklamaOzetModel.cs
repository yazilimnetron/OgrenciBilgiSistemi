namespace OgrenciBilgiSistemi.Api.Models
{
    /// <summary>
    /// Bir sınıfın belirli bir tarihteki tüm öğrenci yoklamalarını,
    /// her öğrenci için 8 ders durumu + yoklamayı yapan öğretmen bilgisiyle döner.
    /// </summary>
    public class SinifYoklamaOzetModel
    {
        public int OgrenciId { get; set; }
        public string OgrenciAdSoyad { get; set; } = string.Empty;
        public int OgrenciNo { get; set; }

        public int? Ders1 { get; set; }
        public int? Ders2 { get; set; }
        public int? Ders3 { get; set; }
        public int? Ders4 { get; set; }
        public int? Ders5 { get; set; }
        public int? Ders6 { get; set; }
        public int? Ders7 { get; set; }
        public int? Ders8 { get; set; }

        public int? KullaniciId { get; set; }
        public string? KullaniciAdi { get; set; }
    }
}
