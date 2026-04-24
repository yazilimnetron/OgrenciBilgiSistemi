using System.Text;
using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class MusaitlikService : TemelApiService
    {
        public async Task<List<Musaitlik>> MusaitlikleriGetir()
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}musaitlik/benim");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Musaitlik>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MUSAITLIK HATASI]: {ex.Message}");
            }
            return new();
        }

        public async Task<bool> MusaitlikEkle(int gun, string baslangicSaati, string bitisSaati)
        {
            try
            {
                var body = new { gun, baslangicSaati, bitisSaati };
                var content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await PostAsync($"{BaseUrl}musaitlik", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> MusaitlikSil(int musaitlikId)
        {
            try
            {
                var response = await DeleteAsync($"{BaseUrl}musaitlik/{musaitlikId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<MusaitSlot>> MusaitSlotlariGetir(int ogretmenId)
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}musaitlik/ogretmen/{ogretmenId}/slotlar");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<MusaitSlot>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MUSAITLIK HATASI]: {ex.Message}");
            }
            return new();
        }
    }
}
