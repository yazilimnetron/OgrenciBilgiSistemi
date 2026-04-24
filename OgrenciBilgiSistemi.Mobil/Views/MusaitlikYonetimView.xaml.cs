using OgrenciBilgiSistemi.Mobil.Models;
using OgrenciBilgiSistemi.Mobil.Services;

namespace OgrenciBilgiSistemi.Mobil.Views
{
    public partial class MusaitlikYonetimView : ContentPage
    {
        private readonly MusaitlikService _musaitlikService;

        public MusaitlikYonetimView(MusaitlikService musaitlikService)
        {
            InitializeComponent();
            _musaitlikService = musaitlikService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await MusaitlikleriYukle();
        }

        private async Task MusaitlikleriYukle()
        {
            try
            {
                var musaitlikler = await _musaitlikService.MusaitlikleriGetir();
                MusaitlikCollection.ItemsSource = musaitlikler;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MUSAITLIK LISTE HATASI]: {ex.Message}");
            }
        }

        private async void OnEkleClicked(object sender, EventArgs e)
        {
            if (GunPicker.SelectedIndex < 0)
            {
                await DisplayAlert("Uyarı", "Lütfen bir gün seçin.", "Tamam");
                return;
            }

            var gun = GunPicker.SelectedIndex + 1; // 1=Pazartesi .. 5=Cuma
            var baslangic = BaslangicSaati.Time.ToString(@"hh\:mm");
            var bitis = BitisSaati.Time.ToString(@"hh\:mm");

            if (BaslangicSaati.Time >= BitisSaati.Time)
            {
                await DisplayAlert("Uyarı", "Başlangıç saati bitiş saatinden önce olmalıdır.", "Tamam");
                return;
            }

            var sonuc = await _musaitlikService.MusaitlikEkle(gun, baslangic, bitis);
            if (sonuc)
            {
                await MusaitlikleriYukle();
                GunPicker.SelectedIndex = -1;
            }
            else
            {
                await DisplayAlert("Hata", "Müsaitlik eklenirken bir sorun oluştu.", "Tamam");
            }
        }

        private async void OnSilClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is int musaitlikId)
            {
                var onay = await DisplayAlert("Onay", "Bu müsaitliği silmek istiyor musunuz?", "Evet", "Hayır");
                if (!onay) return;

                var sonuc = await _musaitlikService.MusaitlikSil(musaitlikId);
                if (sonuc)
                    await MusaitlikleriYukle();
                else
                    await DisplayAlert("Hata", "Müsaitlik silinirken bir sorun oluştu.", "Tamam");
            }
        }
    }
}
