using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    [QueryProperty(nameof(RandevuId), "randevuId")]
    public partial class RandevuDetayView : ContentPage
    {
        private readonly RandevuService _randevuService;
        private Randevu _randevu;
        private int _randevuId;

        public int RandevuId
        {
            get => _randevuId;
            set
            {
                _randevuId = value;
                _ = RandevuYukle();
            }
        }

        public RandevuDetayView(RandevuService randevuService)
        {
            InitializeComponent();
            _randevuService = randevuService;
        }

        /// <summary>
        /// Listedeki randevu nesnesiyle doğrudan açılması için alternatif yapıcı.
        /// </summary>
        public RandevuDetayView(Randevu randevu) : this(
            Application.Current.MainPage.Handler.MauiContext.Services.GetService<RandevuService>())
        {
            _randevu = randevu;
            _randevuId = randevu.RandevuId;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_randevu != null)
                DetaylariGoster();
        }

        private async Task RandevuYukle()
        {
            try
            {
                _randevu = await _randevuService.RandevuGetir(_randevuId);
                if (_randevu != null)
                    MainThread.BeginInvokeOnMainThread(DetaylariGoster);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU DETAY HATASI]: {ex.Message}");
            }
        }

        private void DetaylariGoster()
        {
            if (_randevu == null) return;

            TarihLabel.Text = _randevu.RandevuTarihi.ToString("dd.MM.yyyy HH:mm");
            SureLabel.Text = $"{_randevu.SureDakika} dakika";
            OgretmenLabel.Text = _randevu.OgretmenAdSoyad;
            VeliLabel.Text = _randevu.VeliAdSoyad;

            if (!string.IsNullOrEmpty(_randevu.OgrenciAdSoyad))
            {
                OgrenciLabel.Text = _randevu.OgrenciAdSoyad;
                OgrenciSatir.IsVisible = true;
            }

            if (!string.IsNullOrEmpty(_randevu.Not))
            {
                NotLabel.Text = _randevu.Not;
                NotBorder.IsVisible = true;
            }

            // Durum gösterimi
            switch (_randevu.Durum)
            {
                case 0: // Beklemede
                    DurumIkon.Text = "\u23F3";
                    DurumLabel.Text = "Beklemede";
                    DurumLabel.TextColor = Color.FromArgb("#F39C12");
                    DurumAciklama.Text = "Öğretmen onayı bekleniyor";
                    break;
                case 1: // Onaylandi
                    DurumIkon.Text = "\u2705";
                    DurumLabel.Text = "Onaylandı";
                    DurumLabel.TextColor = Color.FromArgb("#27AE60");
                    DurumAciklama.Text = "Randevu onaylandı";
                    break;
                case 2: // Reddedildi
                    DurumIkon.Text = "\u274C";
                    DurumLabel.Text = "Reddedildi";
                    DurumLabel.TextColor = Color.FromArgb("#E74C3C");
                    DurumAciklama.Text = "Randevu reddedildi";
                    break;
                case 3: // IptalEdildi
                    DurumIkon.Text = "\u26D4";
                    DurumLabel.Text = "İptal Edildi";
                    DurumLabel.TextColor = Color.FromArgb("#95A5A6");
                    DurumAciklama.Text = "Randevu iptal edildi";
                    break;
                case 4: // Tamamlandi
                    DurumIkon.Text = "\u2714";
                    DurumLabel.Text = "Tamamlandı";
                    DurumLabel.TextColor = Color.FromArgb("#3498DB");
                    DurumAciklama.Text = "Randevu tamamlandı";
                    break;
            }

            AksiyonlariAyarla();
        }

        private void AksiyonlariAyarla()
        {
            AksiyonPanel.IsVisible = false;
            OnaylaButton.IsVisible = false;
            ReddetButton.IsVisible = false;
            IptalButton.IsVisible = false;

            if (_randevu.Durum == 0 && KullaniciOturum.OgretmenMi)
            {
                // Öğretmen bekleyen randevuyu onaylayabilir/reddedebilir
                AksiyonPanel.IsVisible = true;
                OnaylaButton.IsVisible = true;
                ReddetButton.IsVisible = true;
            }

            if (_randevu.Durum == 0 || _randevu.Durum == 1)
            {
                // Her iki taraf bekleyen veya onaylanan randevuyu iptal edebilir
                AksiyonPanel.IsVisible = true;
                IptalButton.IsVisible = true;
            }
        }

        private async void OnOnaylaClicked(object sender, EventArgs e)
        {
            var onay = await DisplayAlert("Onay", "Randevuyu onaylamak istiyor musunuz?", "Evet", "Hayır");
            if (!onay) return;

            var sonuc = await _randevuService.Onayla(_randevu.RandevuId);
            if (sonuc)
            {
                await DisplayAlert("Başarılı", "Randevu onaylandı.", "Tamam");
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("Hata", "Randevu onaylanırken bir sorun oluştu.", "Tamam");
        }

        private async void OnReddetClicked(object sender, EventArgs e)
        {
            var onay = await DisplayAlert("Onay", "Randevuyu reddetmek istiyor musunuz?", "Evet", "Hayır");
            if (!onay) return;

            var sonuc = await _randevuService.Reddet(_randevu.RandevuId);
            if (sonuc)
            {
                await DisplayAlert("Başarılı", "Randevu reddedildi.", "Tamam");
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("Hata", "Randevu reddedilirken bir sorun oluştu.", "Tamam");
        }

        private async void OnIptalClicked(object sender, EventArgs e)
        {
            var onay = await DisplayAlert("Onay", "Randevuyu iptal etmek istiyor musunuz?", "Evet", "Hayır");
            if (!onay) return;

            var sonuc = await _randevuService.IptalEt(_randevu.RandevuId);
            if (sonuc)
            {
                await DisplayAlert("Başarılı", "Randevu iptal edildi.", "Tamam");
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("Hata", "Randevu iptal edilirken bir sorun oluştu.", "Tamam");
        }
    }
}
