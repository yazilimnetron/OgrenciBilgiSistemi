using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class VeliDuyurularView : ContentPage
    {
        private readonly DuyuruService _duyuruService;
        private readonly List<DuyuruGorunumModel> _duyurular = new();

        public VeliDuyurularView(DuyuruService duyuruService)
        {
            InitializeComponent();
            _duyuruService = duyuruService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _duyurular.Clear();
            await DuyurulariYukle();
        }

        private async Task DuyurulariYukle()
        {
            try
            {
                var liste = await _duyuruService.BenimDuyurular();
                foreach (var d in liste)
                    _duyurular.Add(new DuyuruGorunumModel(d));

                DuyuruCollection.ItemsSource = null;
                DuyuruCollection.ItemsSource = _duyurular;

                AltBaslikGuncelle();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DUYURU LISTE HATASI]: {ex.Message}");
            }
        }

        private void AltBaslikGuncelle()
        {
            var okunmamis = _duyurular.Count(d => d.OkunmadiMi);
            AltBaslikLabel.Text = okunmamis > 0
                ? $"{okunmamis} okunmamış duyuru"
                : "Tüm duyurular okundu";
        }

        private async void OnDuyuruTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not DuyuruGorunumModel gorunum) return;
            if (gorunum.Duyuru.Okundu) return;

            var basarili = await _duyuruService.OkunduIsaretle(gorunum.Duyuru.DuyuruId);
            if (!basarili) return;

            gorunum.Duyuru.Okundu = true;
            DuyuruCollection.ItemsSource = null;
            DuyuruCollection.ItemsSource = _duyurular;
            AltBaslikGuncelle();
        }

        private async void OnTumunuOkunduClicked(object sender, TappedEventArgs e)
        {
            var sonuc = await _duyuruService.TumunuOkunduIsaretle();
            if (!sonuc) return;

            foreach (var d in _duyurular)
                d.Duyuru.Okundu = true;

            DuyuruCollection.ItemsSource = null;
            DuyuruCollection.ItemsSource = _duyurular;
            AltBaslikLabel.Text = "Tüm duyurular okundu";
        }
    }

    public class DuyuruGorunumModel
    {
        public Duyuru Duyuru { get; }

        public DuyuruGorunumModel(Duyuru duyuru)
        {
            Duyuru = duyuru;
        }

        public bool OkunmadiMi => !Duyuru.Okundu;

        public Color ArkaplanRenk => Duyuru.Okundu
            ? Colors.White
            : Color.FromArgb("#FFF8F0");

        public Color SolCizgiRenk => Duyuru.Okundu
            ? Color.FromArgb("#ECF0F1")
            : Color.FromArgb("#E67E22");
    }
}
