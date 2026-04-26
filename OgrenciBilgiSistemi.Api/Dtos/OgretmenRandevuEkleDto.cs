using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Api.Dtos
{
    public class OgretmenRandevuEkleDto
    {
        [Required]
        public DateTime Tarih { get; set; }

        [Required]
        public string BaslangicSaati { get; set; } = string.Empty;

        [Required]
        public string BitisSaati { get; set; } = string.Empty;
    }
}
