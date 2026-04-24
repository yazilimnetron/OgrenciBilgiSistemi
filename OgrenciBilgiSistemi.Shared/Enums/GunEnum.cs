using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Shared.Enums
{
    public enum GunEnum
    {
        [Display(Name = "Pazartesi")]
        Pazartesi = 1,

        [Display(Name = "Salı")]
        Sali = 2,

        [Display(Name = "Çarşamba")]
        Carsamba = 3,

        [Display(Name = "Perşembe")]
        Persembe = 4,

        [Display(Name = "Cuma")]
        Cuma = 5,

        [Display(Name = "Cumartesi")]
        Cumartesi = 6,

        [Display(Name = "Pazar")]
        Pazar = 7
    }
}
