using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Api.Dtos
{
    public class RandevuOlusturDto
    {
        [Required]
        public int KarsiTarafKullaniciId { get; set; }

        public int? OgrenciId { get; set; }

        [Required]
        public DateTime RandevuTarihi { get; set; }

        public int SureDakika { get; set; } = 30;

        [StringLength(500)]
        public string? Not { get; set; }
    }
}
