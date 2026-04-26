using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public class OgretmenRandevuService : IOgretmenRandevuService
    {
        private readonly AppDbContext _db;

        public OgretmenRandevuService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<OgretmenRandevuModel>> OgretmeneGoreListele(int ogretmenKullaniciId, CancellationToken ct = default)
        {
            return await _db.OgretmenRandevular
                .AsNoTracking()
                .Include(m => m.Ogretmen)
                .Where(m => m.OgretmenKullaniciId == ogretmenKullaniciId)
                .OrderBy(m => m.Tarih)
                .ThenBy(m => m.BaslangicSaati)
                .ToListAsync(ct);
        }

        public async Task Ekle(OgretmenRandevuModel model, CancellationToken ct = default)
        {
            _db.OgretmenRandevular.Add(model);
            await _db.SaveChangesAsync(ct);
        }

        public async Task Sil(int ogretmenRandevuId, CancellationToken ct = default)
        {
            var ent = await _db.OgretmenRandevular.FindAsync(new object[] { ogretmenRandevuId }, ct)
                      ?? throw new KeyNotFoundException("Randevu takvimi bulunamadı.");

            ent.IsDeleted = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
