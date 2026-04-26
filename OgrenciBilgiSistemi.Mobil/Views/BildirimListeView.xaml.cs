using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class BildirimListeView : ContentPage
    {
        private readonly BildirimService _bildirimService;
        private readonly List<BildirimGorunumModel> _bildirimler = new();
        private int _sayfaNo = 1;
        private bool _dahaFazlaVar = true;

        public BildirimListeView(BildirimService bildirimService)
        {
            InitializeComponent();
            _bildirimService = bildirimService;
        }

        public BildirimListeView() : this(
            Application.Current.MainPage.Handler.MauiContext.Services.GetService<BildirimService>())
        {
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _bildirimler.Clear();
            _sayfaNo = 1;
            _dahaFazlaVar = true;
            await BildirimleriYukle();
        }

        private async Task BildirimleriYukle()
        {
            try
            {
                var yeniler = await _bildirimService.BildirimleriGetir(_sayfaNo);
                if (yeniler.Count == 0)
                {
                    _dahaFazlaVar = false;
                    return;
                }

                foreach (var b in yeniler)
                    _bildirimler.Add(new BildirimGorunumModel(b));

                BildirimCollection.ItemsSource = null;
                BildirimCollection.ItemsSource = _bildirimler;

                var okunmamis = _bildirimler.Count(b => b.OkunmadiMi);
                AltBaslikLabel.Text = okunmamis > 0 ? $"{okunmamis} okunmamış bildirim" : "Tüm bildirimler okundu";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BILDIRIM LISTE HATASI]: {ex.Message}");
            }
        }

        private async void OnDahaFazlaYukle(object sender, EventArgs e)
        {
            if (!_dahaFazlaVar) return;
            _sayfaNo++;
            await BildirimleriYukle();
        }

        private async void OnBildirimTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not BildirimGorunumModel gorunum) return;

            if (!gorunum.Bildirim.Okundu)
            {
                await _bildirimService.OkunduIsaretle(gorunum.Bildirim.BildirimId);
                gorunum.Bildirim.Okundu = true;
                BildirimCollection.ItemsSource = null;
                BildirimCollection.ItemsSource = _bildirimler;

                var okunmamis = _bildirimler.Count(b => b.OkunmadiMi);
                AltBaslikLabel.Text = okunmamis > 0 ? $"{okunmamis} okunmamış bildirim" : "Tüm bildirimler okundu";
            }

            if (gorunum.Bildirim.RandevuId.HasValue)
            {
                var randevuService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<RandevuService>();
                var detayView = new RandevuDetayView(randevuService);
                detayView.RandevuId = gorunum.Bildirim.RandevuId.Value;
                await Navigation.PushAsync(detayView);
            }
        }

        private async void OnTumunuOkunduClicked(object sender, TappedEventArgs e)
        {
            var sonuc = await _bildirimService.TumunuOkunduIsaretle();
            if (sonuc)
            {
                foreach (var b in _bildirimler)
                    b.Bildirim.Okundu = true;

                BildirimCollection.ItemsSource = null;
                BildirimCollection.ItemsSource = _bildirimler;
                AltBaslikLabel.Text = "Tüm bildirimler okundu";
            }
        }
    }

    public class BildirimGorunumModel
    {
        public Bildirim Bildirim { get; }

        public BildirimGorunumModel(Bildirim bildirim)
        {
            Bildirim = bildirim;
        }

        public bool OkunmadiMi => !Bildirim.Okundu;

        public Color ArkaplanRenk => Bildirim.Okundu
            ? Colors.White
            : Color.FromArgb("#FFF8F0");

        public Color SolCizgiRenk => Bildirim.Okundu
            ? Color.FromArgb("#ECF0F1")
            : Color.FromArgb("#E67E22");
    }
}
