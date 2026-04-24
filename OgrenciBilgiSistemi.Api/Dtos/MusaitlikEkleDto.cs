using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Api.Dtos
{
    public class MusaitlikEkleDto
    {
        [Required]
        public int Gun { get; set; }

        [Required]
        public string BaslangicSaati { get; set; } = string.Empty;

        [Required]
        public string BitisSaati { get; set; } = string.Empty;
    }
}
