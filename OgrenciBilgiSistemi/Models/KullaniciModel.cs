using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    public class KullaniciModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        public bool BeniHatirla { get; set; } = true;

        [Display(Name = "Rol")]
        public KullaniciRolu Rol { get; set; } = KullaniciRolu.Ogretmen;

        public bool KullaniciDurum { get; set; } = true;

        [StringLength(15)]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Telefon numarası yalnızca rakamlardan oluşmalıdır!")]
        public string? Telefon { get; set; }

        public int? OgrenciVeliId { get; set; }

        [ForeignKey(nameof(OgrenciVeliId))]
        [ValidateNever]
        public OgrenciVeliModel? OgrenciVeli { get; set; }

        public int? PersonelId { get; set; }

        [ForeignKey(nameof(PersonelId))]
        [ValidateNever]
        public virtual PersonelModel? Personel { get; set; }

        [NotMapped]
        public int? ServisId { get; set; }

        [NotMapped]
        public List<SelectListItem> Veliler { get; set; } = new();

        [NotMapped]
        public List<SelectListItem> Personeller { get; set; } = new();

        [NotMapped]
        public List<SelectListItem> Servisler { get; set; } = new();

        public ICollection<KullaniciMenuModel> KullaniciMenuler { get; set; } = new List<KullaniciMenuModel>();
    }
}