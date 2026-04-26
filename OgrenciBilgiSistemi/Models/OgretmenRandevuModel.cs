using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OgrenciBilgiSistemi.Models
{
    public class OgretmenRandevuModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OgretmenRandevuId { get; set; }

        [Required]
        [Display(Name = "Öğretmen")]
        public int OgretmenKullaniciId { get; set; }

        [Required]
        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; }

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
