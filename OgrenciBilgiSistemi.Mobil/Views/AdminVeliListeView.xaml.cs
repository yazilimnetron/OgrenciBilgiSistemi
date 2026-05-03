using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminVeliListeView : ContentPage
    {
        private readonly VeliListeService _veliListeService;

        public AdminVeliListeView(VeliListeService veliListeService)
        {
            InitializeComponent();
            _veliListeService = veliListeService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var liste = await _veliListeService.AktifVelileriGetir();
                VeliCollection.ItemsSource = liste;
                AltBaslikLabel.Text = $"Toplam {liste.Count} veli";

                if (liste.Count == 0)
                    BosDurumLabel.Text = "Kayıtlı veli bulunamadı.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminVeliListe Yükleme Hatası: {ex.Message}");
                BosDurumLabel.Text = "Veriler yüklenemedi.";
            }
        }
    }
}
