using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminSinifOgrenciListeView : ContentPage
    {
        private readonly OgrenciService _ogrenciService;
        private readonly int _sinifId;

        public AdminSinifOgrenciListeView(OgrenciService ogrenciService, int sinifId, string sinifAdi)
        {
            InitializeComponent();
            _ogrenciService = ogrenciService;
            _sinifId = sinifId;
            SinifAdiLabel.Text = string.IsNullOrWhiteSpace(sinifAdi) ? "Sınıf" : sinifAdi;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var liste = await _ogrenciService.SinifaGoreOgrencileriGetirAsync(_sinifId);
                OgrenciCollection.ItemsSource = liste;
                AltBaslikLabel.Text = $"{liste.Count} öğrenci";

                if (liste.Count == 0)
                    BosDurumLabel.Text = "Bu sınıfta kayıtlı öğrenci yok.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminSinifOgrenciListe Yükleme Hatası: {ex.Message}");
                BosDurumLabel.Text = "Veriler yüklenemedi.";
            }
        }
    }
}
