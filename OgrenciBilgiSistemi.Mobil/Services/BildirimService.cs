using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class BildirimService : TemelApiService
    {
        public async Task<List<Bildirim>> BildirimleriGetir(int sayfaNo = 1)
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}bildirimler?sayfaNo={sayfaNo}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Bildirim>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BILDIRIM HATASI]: {ex.Message}");
            }
            return new();
        }

        public async Task<int> OkunmamisSayisiGetir()
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}bildirimler/okunmamis-sayisi");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
                    return result.GetProperty("sayi").GetInt32();
                }
            }
            catch { }
            return 0;
        }

        public async Task<bool> OkunduIsaretle(int bildirimId)
        {
            try
            {
                var response = await PutAsync($"{BaseUrl}bildirimler/{bildirimId}/okundu", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> TumunuOkunduIsaretle()
        {
            try
            {
                var response = await PutAsync($"{BaseUrl}bildirimler/tumunu-okundu", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
