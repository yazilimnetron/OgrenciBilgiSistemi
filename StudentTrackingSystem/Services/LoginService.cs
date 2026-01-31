using System.Text;
using System.Text.Json;
using StudentTrackingSystem.Models;

namespace StudentTrackingSystem.Services
{
    public class LoginService : BaseApiService
    {
        public async Task<bool> LoginAsUserAsync(string username, string password)
        {
            try
            {
                // API'ye gönderilecek veri paketi
                var loginData = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // API'deki AuthController -> [HttpPost("login")] ucuna POST isteği atar
                var response = await _httpClient.PostAsync($"{BaseUrl}auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    // API'den dönen JSON verisini User modeline dönüştürüyoruz
                    var user = JsonSerializer.Deserialize<User>(responseString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (user != null)
                    {
                        // Başarılı girişte UserSession (hafıza) güncellenir
                        UserSession.UserId = user.Id;
                        UserSession.FullName = user.Username;
                        UserSession.UnitId = user.UnitId;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda konsola yazdırılır, arayüzde false döner
                System.Diagnostics.Debug.WriteLine($"[API HATASI]: {ex.Message}");
            }

            return false;
        }
    }
}