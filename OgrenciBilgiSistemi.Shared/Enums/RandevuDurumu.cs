using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Shared.Enums
{
    public enum RandevuDurumu
    {
        [Display(Name = "Beklemede")]
        Beklemede = 0,

        [Display(Name = "Onaylandı")]
        Onaylandi = 1,

        [Display(Name = "Reddedildi")]
        Reddedildi = 2,

        [Display(Name = "İptal Edildi")]
        IptalEdildi = 3,

        [Display(Name = "Tamamlandı")]
        Tamamlandi = 4
    }
}
