using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class RandevuListeView : ContentPage
    {
        private readonly RandevuService _randevuService;

        public RandevuListeView(RandevuService randevuService)
        {
            InitializeComponent();
            _randevuService = randevuService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RandevulariYukle();
        }

        private async Task RandevulariYukle()
        {
            try
            {
                var randevular = await _randevuService.RandevulariGetir();
                var gorunumListesi = randevular.Select(r => new RandevuGorunumModel(r)).ToList();
                RandevuCollection.ItemsSource = gorunumListesi;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU LISTE HATASI]: {ex.Message}");
            }
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            await RandevulariYukle();
            RandevuRefresh.IsRefreshing = false;
        }

        private async void OnRandevuTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is RandevuGorunumModel gorunum)
            {
                await Navigation.PushAsync(new RandevuDetayView(gorunum.Randevu));
            }
        }

        private async void OnYeniRandevuClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RandevuOlusturView());
        }

        private async void OnBildirimlerClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BildirimListeView());
        }
    }

    /// <summary>
    /// Randevu listesinde gösterim için yardımcı model.
    /// </summary>
    public class RandevuGorunumModel
    {
        public Randevu Randevu { get; }

        public RandevuGorunumModel(Randevu randevu)
        {
            Randevu = randevu;
        }

        public string KarsiTarafAdi => KullaniciOturum.OgretmenMi
            ? Randevu.VeliAdSoyad
            : Randevu.OgretmenAdSoyad;

        public string? OgrenciAdSoyad => Randevu.OgrenciAdSoyad;
        public bool OgrenciGosterilsinMi => !string.IsNullOrEmpty(Randevu.OgrenciAdSoyad);
        public string TarihMetni => Randevu.RandevuTarihi.ToString("dd.MM.yyyy HH:mm");
        public string SureMetni => $"{Randevu.SureDakika} dk";
        public string DurumAdi => Randevu.DurumAdi;

        public Color DurumRenk => Randevu.Durum switch
        {
            0 => Color.FromArgb("#F39C12"), // Beklemede
            1 => Color.FromArgb("#27AE60"), // Onaylandi
            2 => Color.FromArgb("#E74C3C"), // Reddedildi
            3 => Color.FromArgb("#95A5A6"), // IptalEdildi
            4 => Color.FromArgb("#3498DB"), // Tamamlandi
            _ => Color.FromArgb("#95A5A6")
        };

        public Color DurumArkaplanRenk => Randevu.Durum switch
        {
            0 => Color.FromArgb("#FEF9E7"),
            1 => Color.FromArgb("#EAFAF1"),
            2 => Color.FromArgb("#FDEDEC"),
            3 => Color.FromArgb("#F2F3F4"),
            4 => Color.FromArgb("#EBF5FB"),
            _ => Color.FromArgb("#F2F3F4")
        };
    }
}
