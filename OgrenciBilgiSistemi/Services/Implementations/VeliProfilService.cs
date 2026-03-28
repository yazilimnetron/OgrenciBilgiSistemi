using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public sealed class VeliProfilService : IVeliProfilService
    {
        private readonly AppDbContext _db;

        public VeliProfilService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> EkleAsync(VeliProfilModel model, CancellationToken ct = default)
        {
            await _db.VeliProfiller.AddAsync(model, ct);
            await _db.SaveChangesAsync(ct);
            return model.KullaniciId;
        }

        public async Task GuncelleAsync(VeliProfilModel model, CancellationToken ct = default)
        {
            _db.VeliProfiller.Update(model);
            await _db.SaveChangesAsync(ct);
        }

        public async Task SilAsync(int kullaniciId, CancellationToken ct = default)
        {
            var veli = await _db.VeliProfiller
                .FirstOrDefaultAsync(v => v.KullaniciId == kullaniciId, ct);

            if (veli is null) return;

            veli.VeliDurum = false;
            await _db.SaveChangesAsync(ct);
        }

        public async Task<VeliProfilModel?> GetByIdAsync(int kullaniciId, CancellationToken ct = default)
        {
            return await _db.VeliProfiller
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.KullaniciId == kullaniciId, ct);
        }
    }
}
