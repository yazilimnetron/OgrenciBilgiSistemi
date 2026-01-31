using System.Text.Json;

namespace StudentTrackingSystem.Services
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;

        // API URL Yapılandırması
        // GÜNCELLEME: Mobil veri erişimi için Dış IP adresi (85.106.232.156) tanımlandı.
        protected readonly string BaseUrl = "http://81.214.75.22:5196/api/";

        // JSON Serileştirme Seçenekleri: API'den gelen verilerin 
        // küçük/büyük harf duyarlılığını yönetmek için merkezi ayar.
        protected readonly JsonSerializerOptions _jsonOptions;

        public BaseApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // 'name' ile 'Name' arasındaki farkı yok sayar.
            };
        }

        /// <summary>
        /// API haberleşmesi sırasında oluşan hataları yönetmek için ortak metot.
        /// </summary>
        protected async Task<bool> HandleResponseStatus(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return true;

            // Hata durumunda loglama veya kullanıcıya bildirim mekanizması buraya eklenebilir.
            var errorContent = await response.Content.ReadAsStringAsync();
            return false;
        }
    }
}