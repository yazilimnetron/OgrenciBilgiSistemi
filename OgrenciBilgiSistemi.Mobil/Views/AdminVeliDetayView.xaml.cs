using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminVeliDetayView : ContentPage
    {
        private readonly VeliListeService _veliListeService;
        private readonly int _veliId;

        public AdminVeliDetayView(int veliId, VeliListeService veliListeService)
        {
            InitializeComponent();
            _veliId = veliId;
            _veliListeService = veliListeService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DetayYukle();
        }

        private async Task DetayYukle()
        {
            try
            {
                var detay = await _veliListeService.VeliDetayGetir(_veliId);
                if (detay is null)
                {
                    DurumYazisi.Text = "Veli bilgileri yüklenemedi.";
                    return;
                }

                AdSoyadLabel.Text = detay.KullaniciAdi;
                YakinlikLabel.Text = detay.YakinlikMetni;
                TelefonLabel.Text = string.IsNullOrWhiteSpace(detay.Telefon) ? "-" : detay.Telefon;
                EmailLabel.Text = string.IsNullOrWhiteSpace(detay.VeliEmail) ? "-" : detay.VeliEmail;
                AdresLabel.Text = string.IsNullOrWhiteSpace(detay.VeliAdres) ? "-" : detay.VeliAdres;
                MeslekLabel.Text = string.IsNullOrWhiteSpace(detay.VeliMeslek) ? "-" : detay.VeliMeslek;
                IsYeriLabel.Text = string.IsNullOrWhiteSpace(detay.VeliIsYeri) ? "-" : detay.VeliIsYeri;

                CocuklarBaslik.Text = detay.Cocuklar.Count > 0
                    ? $"Çocuklar ({detay.Cocuklar.Count})"
                    : "Çocuklar";
                CocukCollection.ItemsSource = detay.Cocuklar;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminVeliDetay Yükleme Hatası: {ex.Message}");
                DurumYazisi.Text = "Veriler yüklenemedi.";
            }
        }

        private void OnTelefonTapped(object sender, EventArgs e)
        {
            try
            {
                var no = TelefonLabel.Text?.Trim();
                if (PhoneDialer.Default.IsSupported && !string.IsNullOrEmpty(no) && no != "-")
                    PhoneDialer.Default.Open(no);
            }
            catch { }
        }

        private async void OnCocukSecildi(object sender, TappedEventArgs e)
        {
            if ((sender as Border)?.BindingContext is VeliDetayOgrenci ogrenci)
            {
                var ogrenciService = Servis<OgrenciService>();
                if (ogrenciService is null) return;
                await Navigation.PushAsync(new OgrenciDetayView(ogrenci.OgrenciId, ogrenciService));
            }
        }

        private static T? Servis<T>() where T : class
            => Application.Current?.MainPage?.Handler?.MauiContext?.Services.GetService<T>();
    }
}
