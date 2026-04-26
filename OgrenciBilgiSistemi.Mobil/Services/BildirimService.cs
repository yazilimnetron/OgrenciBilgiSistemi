using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class BildirimService : TemelApiService
    {
        public async Task<List<Bildirim>> BildirimleriGetir(int sayfaNo = 1)
        {
            var response = await GetAsync($"{BaseUrl}bildirimler?sayfaNo={sayfaNo}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[BILDIRIM HATASI]: {(int)response.StatusCode} {response.StatusCode} — {body}");
                throw new Exception($"Sunucu yanıtı: {(int)response.StatusCode} — {body}");
            }

            return JsonSerializer.Deserialize<List<Bildirim>>(body, _jsonOptions) ?? new();
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
