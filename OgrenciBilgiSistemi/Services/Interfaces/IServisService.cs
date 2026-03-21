using Microsoft.AspNetCore.Mvc.Rendering;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Services.Interfaces
{
    public interface IServisService
    {
        Task<SayfalanmisListeModel<ServisModel>> SearchPagedAsync(
            string? searchString, int page, int pageSize = 20, CancellationToken ct = default);

        Task<ServisModel?> GetByIdAsync(int id, CancellationToken ct = default);

        Task EkleAsync(ServisModel model, CancellationToken ct = default);
        Task GuncelleAsync(ServisModel model, CancellationToken ct = default);
        Task SilAsync(int id, CancellationToken ct = default);

        Task<List<SelectListItem>> GetSoforSelectListAsync(int? selectedId = null, CancellationToken ct = default);
    }
}
