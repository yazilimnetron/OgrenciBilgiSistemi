using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminOgrenciListeView : ContentPage
    {
        private readonly OgrenciService _ogrenciService;

        public AdminOgrenciListeView(OgrenciService ogrenciService)
        {
            InitializeComponent();
            _ogrenciService = ogrenciService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var liste = await _ogrenciService.TumOgrencileriGetirAsync();
                OgrenciCollection.ItemsSource = liste;
                AltBaslikLabel.Text = $"Toplam {liste.Count} öğrenci";

                if (liste.Count == 0)
                    BosDurumLabel.Text = "Kayıtlı öğrenci bulunamadı.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminOgrenciListe Yükleme Hatası: {ex.Message}");
                BosDurumLabel.Text = "Veriler yüklenemedi.";
            }
        }

        private async void OnOgrenciSecildi(object sender, TappedEventArgs e)
        {
            if ((sender as Border)?.BindingContext is Ogrenci ogrenci)
                await Navigation.PushAsync(new OgrenciDetayView(ogrenci.OgrenciId, _ogrenciService));
        }
    }
}
