using System.Text;
using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class RandevuService : TemelApiService
    {
        public async Task<List<Randevu>> RandevulariGetir(int sayfaNo = 1)
        {
            var response = await GetAsync($"{BaseUrl}randevular/benim?sayfaNo={sayfaNo}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU HATASI]: {(int)response.StatusCode} {response.StatusCode} — {body}");
                throw new Exception($"Sunucu yanıtı: {(int)response.StatusCode} — {body}");
            }

            return JsonSerializer.Deserialize<List<Randevu>>(body, _jsonOptions) ?? new();
        }

        public async Task<Randevu?> RandevuGetir(int randevuId)
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}randevular/{randevuId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Randevu>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU HATASI]: {ex.Message}");
            }
            return null;
        }

        public async Task<(bool Basarili, string? HataMesaji)> RandevuOlustur(int karsiTarafId, int? ogrenciId, DateTime tarih, int sureDakika, string? not)
        {
            try
            {
                var body = new
                {
                    karsiTarafKullaniciId = karsiTarafId,
                    ogrenciId,
                    randevuTarihi = tarih,
                    sureDakika,
                    not
                };
                var content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await PostAsync($"{BaseUrl}randevular", content);

                if (response.IsSuccessStatusCode)
                    return (true, null);

                // 409 Conflict (slot çakışması) — server { mesaj: "..." } döndürüyor
                var responseBody = await response.Content.ReadAsStringAsync();
                string? mesaj = null;
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("mesaj", out var m))
                        mesaj = m.GetString();
                }
                catch { /* body JSON değilse mesajı boş bırak */ }

                return (false, mesaj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RANDEVU HATASI]: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<(bool CakismaVar, string? Mesaj)> CakismaKontrolu(DateTime tarih, int sureDakika, int? karsiTarafId)
        {
            try
            {
                var url = $"{BaseUrl}randevular/cakisma-kontrolu" +
                          $"?tarih={Uri.EscapeDataString(tarih.ToString("o"))}" +
                          $"&sureDakika={sureDakika}";
                if (karsiTarafId.HasValue)
                    url += $"&karsiTarafKullaniciId={karsiTarafId.Value}";

                var response = await GetAsync(url);
                if (!response.IsSuccessStatusCode) return (false, null);

                var body = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);
                var cakisma = doc.RootElement.GetProperty("cakismaVar").GetBoolean();
                var mesaj = doc.RootElement.TryGetProperty("mesaj", out var m) ? m.GetString() : null;
                return (cakisma, mesaj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CAKISMA HATASI]: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<bool> Onayla(int randevuId)
        {
            try
            {
                var response = await PutAsync($"{BaseUrl}randevular/{randevuId}/onayla", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> Reddet(int randevuId)
        {
            try
            {
                var response = await PutAsync($"{BaseUrl}randevular/{randevuId}/reddet", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> IptalEt(int randevuId)
        {
            try
            {
                var response = await PutAsync($"{BaseUrl}randevular/{randevuId}/iptal", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
