using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminOgretmenListeView : ContentPage
    {
        private readonly OgretmenListeService _ogretmenListeService;

        public AdminOgretmenListeView(OgretmenListeService ogretmenListeService)
        {
            InitializeComponent();
            _ogretmenListeService = ogretmenListeService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var liste = await _ogretmenListeService.AktifOgretmenleriGetir();
                OgretmenCollection.ItemsSource = liste;
                AltBaslikLabel.Text = $"Toplam {liste.Count} öğretmen";

                if (liste.Count == 0)
                    BosDurumLabel.Text = "Kayıtlı öğretmen bulunamadı.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminOgretmenListe Yükleme Hatası: {ex.Message}");
                BosDurumLabel.Text = "Veriler yüklenemedi.";
            }
        }
    }
}
