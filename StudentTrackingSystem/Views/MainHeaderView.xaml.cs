using StudentTrackingSystem.Services;
using System;

namespace StudentTrackingSystem.Views;

public partial class MainHeaderView : ContentView
{
    #region Bindable Properties (Mevcut yapýn)
    public static readonly BindableProperty ShowBackProperty =
        BindableProperty.Create(nameof(ShowBack), typeof(bool), typeof(MainHeaderView), true);

    public static readonly BindableProperty ShowProfileProperty =
        BindableProperty.Create(nameof(ShowProfile), typeof(bool), typeof(MainHeaderView), true);

    public static readonly BindableProperty UserInitialProperty =
        BindableProperty.Create(nameof(UserInitial), typeof(string), typeof(MainHeaderView), string.Empty);

    public bool ShowBack { get => (bool)GetValue(ShowBackProperty); set => SetValue(ShowBackProperty, value); }
    public bool ShowProfile { get => (bool)GetValue(ShowProfileProperty); set => SetValue(ShowProfileProperty, value); }
    public string UserInitial { get => (string)GetValue(UserInitialProperty); set => SetValue(UserInitialProperty, value); }
    #endregion

    public MainHeaderView()
    {
        InitializeComponent();
        UpdateInitial();
    }

    private void UpdateInitial()
    {
        if (string.IsNullOrEmpty(UserInitial))
        {
            var name = UserSession.FullName;
            UserInitial = (!string.IsNullOrEmpty(name) && name != "Kullanýcý")
                          ? name.Trim().Substring(0, 1).ToUpper()
                          : "?";
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        // 1. ADIM: Alttan modern ActionSheet menüsünü aç
        // Parametreler: Baþlýk, Ýptal Butonu Metni, Yýkýcý Buton (Kýrmýzý), Seçenekler...
        string action = await Shell.Current.CurrentPage.DisplayActionSheet(
            "Profil Ýþlemleri",
            "Vazgeç",
            null,
            "Çýkýþ Yap");

        if (action == "Çýkýþ Yap")
        {
            // 2. ADIM: Yanlýþlýkla basýlmalara karþý son onay
            bool answer = await Shell.Current.CurrentPage.DisplayAlert(
                "Oturumu Kapat",
                "Uygulamadan çýkýþ yapmak istediðinize emin misiniz?",
                "Evet",
                "Hayýr");

            if (answer)
            {
                PerformLogout();
            }
        }
    }

    private async void PerformLogout()
    {
        // 3. ADIM: Veri Temizliði (Ana Kural: WEB API için oturumu sýfýrla)
        UserSession.FullName = null;
        // Eðer varsa Token veya Id sýfýrlama:
        // UserSession.TeacherId = 0;

        // 4. ADIM: Güvenli Navigasyon
        // //LoginPage kullanarak tüm geçmiþi siler ve geri dönüþü engelleriz.
        await Shell.Current.GoToAsync("//LoginView");
    }
}