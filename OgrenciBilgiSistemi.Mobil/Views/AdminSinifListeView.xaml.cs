using OgrenciBilgiSistemi.Mobil.Services;

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
    }
}
