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

    // API'den gelen veriyi karşılamak için geçici DTO tanımı
    // (Bunu ayrı bir dosyaya da alabilirsin)
    public class UnitWithCountDto
    {
        public Unit UnitData { get; set; }
        public int StudentCount { get; set; }
    }
}