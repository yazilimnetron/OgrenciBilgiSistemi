using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class VeliListeService : TemelApiService
    {
        public async Task<List<Veli>> AktifVelileriGetir()
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}veliler/aktif");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Veli>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VELI LISTE HATASI]: {ex.Message}");
            }
            return new();
        }
    }
}
