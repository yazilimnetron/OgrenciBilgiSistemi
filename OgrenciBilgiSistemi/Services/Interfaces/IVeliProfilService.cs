using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Services.Interfaces
{
    public interface IVeliProfilService
    {
        Task<int> EkleAsync(VeliProfilModel model, CancellationToken ct = default);
        Task GuncelleAsync(VeliProfilModel model, CancellationToken ct = default);
        Task SilAsync(int kullaniciId, CancellationToken ct = default);
        Task<VeliProfilModel?> GetByIdAsync(int kullaniciId, CancellationToken ct = default);
    }
}
