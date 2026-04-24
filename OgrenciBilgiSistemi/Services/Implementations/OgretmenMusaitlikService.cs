using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public class OgretmenMusaitlikService : IOgretmenMusaitlikService
    {
        private readonly AppDbContext _db;

        public OgretmenMusaitlikService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<OgretmenMusaitlikModel>> OgretmeneGoreListele(int ogretmenKullaniciId, CancellationToken ct = default)
        {
            return await _db.OgretmenMusaitlikler
                .AsNoTracking()
                .Include(m => m.Ogretmen)
                .Where(m => m.OgretmenKullaniciId == ogretmenKullaniciId)
                .OrderBy(m => m.Gun)
                .ThenBy(m => m.BaslangicSaati)
                .ToListAsync(ct);
        }

        public async Task Ekle(OgretmenMusaitlikModel model, CancellationToken ct = default)
        {
            _db.OgretmenMusaitlikler.Add(model);
            await _db.SaveChangesAsync(ct);
        }

        public async Task Sil(int musaitlikId, CancellationToken ct = default)
        {
            var ent = await _db.OgretmenMusaitlikler.FindAsync(new object[] { musaitlikId }, ct)
                      ?? throw new KeyNotFoundException("Müsaitlik bulunamadı.");

            ent.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
