namespace OgrenciBilgiSistemi.Api.Models
{
    public class BildirimModel
    {
        public int BildirimId { get; set; }
        public int Tur { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public int? RandevuId { get; set; }
        public bool Okundu { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}
