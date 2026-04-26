using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class RandevuOlusturView : ContentPage
    {
        private readonly RandevuService _randevuService;
        private readonly OgretmenRandevuService _ogretmenRandevuService;
        private readonly VeliService _veliService;
        private readonly OgrenciService _ogrenciService;
        private readonly OgretmenListeService _ogretmenListeService;

        private List<Ogrenci> _cocuklar = new();
        private List<Ogrenci> _sinifOgrencileri = new();
        private List<OgretmenBilgi> _ogretmenler = new();
        private List<RandevuSlot> _randevuSlotlar = new();
        private RandevuSlot _secilenSlot;
        private int? _karsiTarafId;
        private int? _sinifOgretmenId;

        public RandevuOlusturView()
        {
            InitializeComponent();

            _randevuService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<RandevuService>();
            _ogretmenRandevuService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<OgretmenRandevuService>();
            _veliService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<VeliService>();
            _ogrenciService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<OgrenciService>();
            _ogretmenListeService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<OgretmenListeService>();

            SurePicker.SelectedIndex = 1; // 30 dakika varsayılan
            TarihSecici.MinimumDate = DateTime.Today;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (KullaniciOturum.VeliMi)
            {
                AltBaslikLabel.Text = "Öğretmenle görüşme talebi";
                OgretmenSecimBorder.IsVisible = true;
                SlotSecimBorder.IsVisible = true;
                await VeliIcinHazirla();
            }
            else if (KullaniciOturum.OgretmenMi)
            {
                AltBaslikLabel.Text = "Veli ile görüşme planla";
                VeliSecimBorder.IsVisible = true;
                TarihSecimBorder.IsVisible = true;
                await OgretmenIcinHazirla();
            }
        }

        private async Task VeliIcinHazirla()
        {
            try
            {
                _cocuklar = await _veliService.CocuklarimiGetir();

                if (_cocuklar.Count == 1)
                {
                    OgrenciPicker.ItemsSource = _cocuklar.Select(c => c.OgrenciAdSoyad).ToList();
                    OgrenciPicker.SelectedIndex = 0;
                    OgrenciPicker.IsEnabled = false;
                }
                else
                {
                    OgrenciPicker.ItemsSource = _cocuklar.Select(c => c.OgrenciAdSoyad).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU FORM HATASI]: {ex.Message}");
            }
        }

        private async Task OgretmenIcinHazirla()
        {
            try
            {
                if (KullaniciOturum.BirimId.HasValue)
                {
                    _sinifOgrencileri = await _ogrenciService.SinifaGoreOgrencileriGetirAsync(KullaniciOturum.BirimId.Value);
                    VeliOgrenciPicker.ItemsSource = _sinifOgrencileri.Select(o => o.OgrenciAdSoyad).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU FORM HATASI]: {ex.Message}");
            }
        }

        private async void OnOgrenciSecildi(object sender, EventArgs e)
        {
            var index = OgrenciPicker.SelectedIndex;
            if (index < 0 || index >= _cocuklar.Count) return;

            var ogrenci = _cocuklar[index];
            _sinifOgretmenId = ogrenci.OgretmenId;

            _ogretmenler = await _ogretmenListeService.AktifOgretmenleriGetir();
            OgretmenPicker.ItemsSource = _ogretmenler.Select(o => o.KullaniciAdi).ToList();
            OgretmenPickerBorder.IsVisible = true;

            if (_sinifOgretmenId.HasValue && _sinifOgretmenId.Value > 0)
            {
                var sinifOgretmenIndex = _ogretmenler.FindIndex(o => o.KullaniciId == _sinifOgretmenId.Value);
                if (sinifOgretmenIndex >= 0)
                {
                    OgretmenPicker.SelectedIndex = sinifOgretmenIndex;
                    OgretmenBilgiLabel.Text = "Sınıf öğretmeni seçili";
                    OgretmenBilgiLabel.IsVisible = true;
                }
            }
        }

        private async void OnOgretmenSecildi(object sender, EventArgs e)
        {
            var index = OgretmenPicker.SelectedIndex;
            if (index < 0 || index >= _ogretmenler.Count) return;

            var secilen = _ogretmenler[index];
            _karsiTarafId = secilen.KullaniciId;
            _secilenSlot = null;

            if (secilen.KullaniciId == _sinifOgretmenId)
            {
                OgretmenBilgiLabel.Text = "Sınıf öğretmeni seçili";
                OgretmenBilgiLabel.IsVisible = true;
            }
            else
            {
                OgretmenBilgiLabel.IsVisible = false;
            }

            _randevuSlotlar = await _ogretmenRandevuService.RandevuSlotlariGetir(secilen.KullaniciId);
            SlotCollection.ItemsSource = _randevuSlotlar;
        }

        private void OnSlotSecildi(object sender, SelectionChangedEventArgs e)
        {
            _secilenSlot = e.CurrentSelection.FirstOrDefault() as RandevuSlot;
        }

        private async void OnOlusturClicked(object sender, EventArgs e)
        {
            OlusturButton.IsEnabled = false;

            try
            {
                int? ogrenciId = null;
                DateTime randevuTarihi;
                int sureDakika = SureSecimindenDakika();

                if (KullaniciOturum.VeliMi)
                {
                    if (_karsiTarafId == null)
                    {
                        await DisplayAlert("Uyarı", "Lütfen bir öğrenci seçin.", "Tamam");
                        return;
                    }
                    if (_secilenSlot == null)
                    {
                        await DisplayAlert("Uyarı", "Lütfen müsait bir saat seçin.", "Tamam");
                        return;
                    }

                    var index = OgrenciPicker.SelectedIndex;
                    if (index >= 0 && index < _cocuklar.Count)
                        ogrenciId = _cocuklar[index].OgrenciId;

                    randevuTarihi = _secilenSlot.Tarih.Date +
                        TimeSpan.Parse(_secilenSlot.BaslangicSaati);
                }
                else
                {
                    var veliIndex = VeliOgrenciPicker.SelectedIndex;
                    if (veliIndex < 0 || veliIndex >= _sinifOgrencileri.Count)
                    {
                        await DisplayAlert("Uyarı", "Lütfen bir öğrenci seçin.", "Tamam");
                        return;
                    }

                    var secilenOgrenci = _sinifOgrencileri[veliIndex];
                    _karsiTarafId = secilenOgrenci.VeliId;
                    ogrenciId = secilenOgrenci.OgrenciId;
                    randevuTarihi = TarihSecici.Date + SaatSecici.Time;
                }

                string not = string.IsNullOrWhiteSpace(NotEditor.Text) ? null : NotEditor.Text.Trim();

                var sonuc = await _randevuService.RandevuOlustur(
                    _karsiTarafId.Value, ogrenciId, randevuTarihi, sureDakika, not);

                if (sonuc)
                {
                    await DisplayAlert("Başarılı", "Randevu oluşturuldu.", "Tamam");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Hata", "Randevu oluşturulurken bir sorun oluştu.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU OLUSTUR HATASI]: {ex.Message}");
                await DisplayAlert("Hata", "Beklenmeyen bir hata oluştu.", "Tamam");
            }
            finally
            {
                OlusturButton.IsEnabled = true;
            }
        }

        private int SureSecimindenDakika()
        {
            return SurePicker.SelectedIndex switch
            {
                0 => 15,
                1 => 30,
                2 => 45,
                3 => 60,
                _ => 30
            };
        }
    }
}
