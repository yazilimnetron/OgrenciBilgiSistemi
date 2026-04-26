using System.Text;
using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class OgretmenRandevuService : TemelApiService
    {
        public async Task<List<OgretmenRandevu>> OgretmenRandevulariGetir()
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}ogretmen-randevu/benim");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<OgretmenRandevu>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OGRETMEN RANDEVU HATASI]: {ex.Message}");
            }
            return new();
        }

        public async Task<bool> OgretmenRandevuEkle(DateTime tarih, string baslangicSaati, string bitisSaati)
        {
            try
            {
                var body = new { tarih = tarih.ToString("yyyy-MM-dd"), baslangicSaati, bitisSaati };
                var content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await PostAsync($"{BaseUrl}ogretmen-randevu", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> OgretmenRandevuSil(int ogretmenRandevuId)
        {
            try
            {
                var response = await DeleteAsync($"{BaseUrl}ogretmen-randevu/{ogretmenRandevuId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<RandevuSlot>> RandevuSlotlariGetir(int ogretmenId)
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}ogretmen-randevu/ogretmen/{ogretmenId}/slotlar");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<RandevuSlot>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OGRETMEN RANDEVU HATASI]: {ex.Message}");
            }
            return new();
        }
    }
}
