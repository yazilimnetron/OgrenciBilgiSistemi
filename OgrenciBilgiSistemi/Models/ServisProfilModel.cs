using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    public class ServisProfilModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Plaka zorunludur!")]
        [StringLength(20)]
        [Display(Name = "Plaka")]
        public string Plaka { get; set; } = string.Empty;

        [StringLength(15)]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Telefon numarası yalnızca rakamlardan oluşmalıdır!")]
        [Display(Name = "Şoför Telefon")]
        public string? SoforTelefon { get; set; }

        [Display(Name = "Durum (Aktif)")]
        public bool ServisDurum { get; set; } = true;

        [NotMapped]
        public int OgrenciSayisi { get; set; }

        [ForeignKey(nameof(KullaniciId))]
        [ValidateNever]
        public virtual KullaniciModel Kullanici { get; set; } = null!;
    }
}
