using OgrenciBilgiSistemi.Mobil.Services;
using OgrenciBilgiSistemi.Mobil.ViewModels;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminSinifListeView : ContentPage
    {
        private readonly SinifService _sinifService;

        public AdminSinifListeView(SinifService sinifService)
        {
            InitializeComponent();
            _sinifService = sinifService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var liste = await _sinifService.TumSiniflariOgrenciSayisiIleGetirAsync();
                SinifCollection.ItemsSource = liste;
                AltBaslikLabel.Text = $"Toplam {liste.Count} sınıf";

                if (liste.Count == 0)
                    BosDurumLabel.Text = "Kayıtlı sınıf bulunamadı.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminSinifListe Yükleme Hatası: {ex.Message}");
                BosDurumLabel.Text = "Veriler yüklenemedi.";
            }
        }

        private async void OnSinifTapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (e.Parameter is not SinifGorunumModel secilen) return;

                var ogrenciService = Application.Current?.MainPage?.Handler?.MauiContext?
                    .Services.GetService<OgrenciService>();
                if (ogrenciService is null) return;

                await Navigation.PushAsync(new AdminSinifOgrenciListeView(
                    ogrenciService,
                    secilen.SinifVerisi.BirimId,
                    secilen.Ad));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminSinifListe Sınıf Tıklama Hatası: {ex.Message}");
            }
        }
    }
}
