using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class AdminOgretmenDetayView : ContentPage
    {
        private readonly OgretmenListeService _ogretmenListeService;
        private readonly int _ogretmenId;

        public AdminOgretmenDetayView(int ogretmenId, OgretmenListeService ogretmenListeService)
        {
            InitializeComponent();
            _ogretmenId = ogretmenId;
            _ogretmenListeService = ogretmenListeService;
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
                var detay = await _ogretmenListeService.OgretmenDetayGetir(_ogretmenId);
                if (detay is null)
                {
                    DurumYazisi.Text = "Öğretmen bilgileri yüklenemedi.";
                    return;
                }

                AdSoyadLabel.Text = detay.KullaniciAdi;
                BirimLabel.Text = string.IsNullOrWhiteSpace(detay.BirimAd) ? "Birim atanmamış" : detay.BirimAd;
                TelefonLabel.Text = string.IsNullOrWhiteSpace(detay.Telefon) ? "-" : detay.Telefon;
                EmailLabel.Text = string.IsNullOrWhiteSpace(detay.Email) ? "-" : detay.Email;

                GorselImage.Source = Constants.GorselUrl(detay.GorselPath);

                if (detay.OgretmenDurum)
                {
                    DurumRozet.BackgroundColor = Color.FromArgb("#27AE60");
                    DurumLabel.Text = "Aktif";
                }
                else
                {
                    DurumRozet.BackgroundColor = Color.FromArgb("#95A5A6");
                    DurumLabel.Text = "Pasif";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdminOgretmenDetay Yükleme Hatası: {ex.Message}");
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
    }
}
