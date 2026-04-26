using System.Text.Json;
using OgrenciBilgiSistemi.Mobil.Models;

namespace OgrenciBilgiSistemi.Mobil.Services
{
    public class OgretmenListeService : TemelApiService
    {
        public async Task<List<OgretmenBilgi>> AktifOgretmenleriGetir()
        {
            try
            {
                var response = await GetAsync($"{BaseUrl}ogretmenler/aktif");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<OgretmenBilgi>>(json, _jsonOptions) ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OGRETMEN LISTE HATASI]: {ex.Message}");
            }
            return new();
        }
    }
}
