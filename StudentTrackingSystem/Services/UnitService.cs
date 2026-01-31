#region Kütüphane Referansları
using System.Net.Http.Json;
using StudentTrackingSystem.Models;
using System.Threading.Tasks;
using System;
#endregion

namespace StudentTrackingSystem.Services
{
    /// <summary>
    /// Birim (Sınıf/Departman) bilgilerini API üzerinden yöneten servis.
    /// </summary>
    public class UnitService : BaseApiService
    {
        #region Yapıcı Metot
        public UnitService() : base()
        {
            // BaseApiService'den gelen HttpClient ve BaseUrl kullanılacak.
        }
        #endregion

        #region Birim İşlemleri

        /// <summary>
        /// Belirli bir birimin detaylarını (Ad, Sınıf mı?) API üzerinden getirir.
        /// </summary>
        /// <param name="unitId">Birim ID</param>
        public async Task<Unit> GetUnitByIdAsync(int unitId)
        {
            try
            {
                // API Ucu: GET api/units/{id}
                var unit = await _httpClient.GetFromJsonAsync<Unit>($"{BaseUrl}units/{unitId}");
                return unit;
            }
            catch (Exception ex)
            {
                // Hata durumunda UI katmanına anlamlı bir mesaj fırlatıyoruz
                throw new Exception($"Birim bilgileri yüklenemedi: {ex.Message}");
            }
        }

        #endregion
    }
}