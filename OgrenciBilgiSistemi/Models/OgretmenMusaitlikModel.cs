using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OgrenciBilgiSistemi.Shared.Enums;

namespace OgrenciBilgiSistemi.Models
{
    public class OgretmenMusaitlikModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MusaitlikId { get; set; }

        [Required]
        [Display(Name = "Öğretmen")]
        public int OgretmenKullaniciId { get; set; }

        [Required]
        [Display(Name = "Gün")]
        public GunEnum Gun { get; set; }

        [Required]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan BaslangicSaati { get; set; }

        [Required]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan BitisSaati { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation
        [ForeignKey(nameof(OgretmenKullaniciId))]
        [ValidateNever]
        public virtual KullaniciModel Ogretmen { get; set; } = null!;
    }
}
