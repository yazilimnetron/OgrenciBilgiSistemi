using System.Net.Http.Json;
using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class ServisService : TemelApiService
    {
        /// <summary>
        /// Belirtilen servise atanmış öğrencileri API'den getirir.
        /// </summary>
        public async Task<List<Ogrenci>> ServisOgrencileriGetir(int servisId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}servisler/{servisId}/ogrenciler");

                if (await YanitDurumuIsle(response))
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<Ogrenci>>(json, _jsonOptions) ?? new List<Ogrenci>();
                    foreach (var o in list)
                        o.OgrenciGorsel = Constants.GorselUrl(o.OgrenciGorsel);
                    return list;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API HATASI]: {ex.Message}");
            }

            return new List<Ogrenci>();
        }

        /// <summary>
        /// Belirtilen servisin bilgilerini API'den getirir.
        /// </summary>
        public async Task<Servis?> ServisGetir(int servisId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}servisler/{servisId}");

                if (await YanitDurumuIsle(response))
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Servis>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API HATASI]: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Belirtilen servisin bugünkü yoklamasını periyoda göre API'den getirir.
        /// </summary>
        public async Task<Dictionary<int, int>> MevcutServisYoklamaGetir(int servisId, int periyot)
        {
            if (KullaniciOturum.DemoModuMu)
                return new Dictionary<int, int>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<int, int>>($"{BaseUrl}servisler/{servisId}/yoklama/{periyot}");
                return response ?? new Dictionary<int, int>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API HATASI]: {ex.Message}");
            }

            return new Dictionary<int, int>();
        }

        /// <summary>
        /// Servis yoklamasını toplu olarak API'ye göndererek kaydeder.
        /// </summary>
        public async Task ServisYoklamaKaydet(IEnumerable<(int OgrenciId, int DurumId)> yoklamaVerisi, int kullaniciId, int periyot)
        {
            if (KullaniciOturum.DemoModuMu)
                return;

            try
            {
                var model = new
                {
                    KullaniciId = kullaniciId,
                    Periyot = periyot,
                    Kayitlar = yoklamaVerisi.Select(a => new
                    {
                        OgrenciId = a.OgrenciId,
                        DurumId = a.DurumId
                    }).ToList()
                };

                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}servisler/yoklama-kaydet", model);

                if (!await YanitDurumuIsle(response))
                    throw new Exception("Servis yoklaması kaydedilemedi.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Servis yoklama kaydı sırasında hata oluştu: {ex.Message}");
            }
        }
    }
}
