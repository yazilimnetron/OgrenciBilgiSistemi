using System.Net.Http.Json;
using StudentTrackingSystem.Models;
using StudentTrackingSystem.ViewModels;

namespace StudentTrackingSystem.Services
{
    public class ClassService : BaseApiService
    {
        public async Task<List<ClassroomViewModel>> GetAllClassesWithStudentCountAsync()
        {
            try
            {
                // API'deki 'api/Class/all-with-count' endpoint'ine istek atıyoruz
                var response = await _httpClient.GetFromJsonAsync<List<UnitWithCountDto>>($"{BaseUrl}Class/all-with-count");

                if (response != null)
                {
                    // API'den gelen DTO listesini MAUI'nin beklediği ViewModel listesine dönüştürüyoruz
                    return response.Select(dto => new ClassroomViewModel
                    {
                        ClassroomData = dto.UnitData,
                        StudentCount = dto.StudentCount
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API HATASI]: Sınıf listesi çekilemedi: {ex.Message}");
            }

            return new List<ClassroomViewModel>();
        }
    }

    // API'nin BirimOgrenciSayisiModel yapısını karşılayan DTO
    public class UnitWithCountDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("birim")]
        public Birim UnitData { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("ogrenciSayisi")]
        public int StudentCount { get; set; }
    }
}