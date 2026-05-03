using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OgrenciBilgiSistemi.Models
{
    public class DuyuruOkumaModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DuyuruOkumaId { get; set; }

        [Required]
        public int DuyuruId { get; set; }

        [Required]
        public int KullaniciId { get; set; }

        public DateTime OkunduTarihi { get; set; } = DateTime.Now;

        [ForeignKey(nameof(DuyuruId))]
        [ValidateNever]
        public virtual DuyuruModel Duyuru { get; set; } = null!;

        [ForeignKey(nameof(KullaniciId))]
        [ValidateNever]
        public virtual KullaniciModel Kullanici { get; set; } = null!;
    }
}
