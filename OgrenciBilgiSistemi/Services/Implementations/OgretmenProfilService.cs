using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public sealed class OgretmenProfilService : IOgretmenProfilService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public OgretmenProfilService(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<SayfalanmisListeModel<OgretmenProfilModel>> SearchPagedAsync(
            string? searchString, int page, int pageSize = 20, CancellationToken ct = default)
        {
            var query = _db.OgretmenProfiller
                .Include(o => o.Kullanici)
                .Include(o => o.Birim)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim();
                query = query.Where(o =>
                    o.Kullanici.KullaniciAdi.Contains(s) ||
                    (o.Email != null && o.Email.Contains(s)) ||
                    (o.Birim != null && o.Birim.BirimAd.Contains(s)));
            }

            return await SayfalanmisListeModel<OgretmenProfilModel>
                .CreateAsync(query.OrderBy(o => o.Kullanici.KullaniciAdi), page, pageSize, ct);
        }

        public async Task<int> EkleAsync(OgretmenProfilModel model, CancellationToken ct = default)
        {
            if (model.GorselFile != null && model.GorselFile.Length > 0)
                model.GorselPath = await SaveImageAsync(model.GorselFile, ct);

            _db.OgretmenProfiller.Add(model);
            await _db.SaveChangesAsync(ct);
            return model.KullaniciId;
        }

        public async Task GuncelleAsync(OgretmenProfilModel model, CancellationToken ct = default)
        {
            var mevcut = await _db.OgretmenProfiller.FindAsync([model.KullaniciId], ct)
                ?? throw new KeyNotFoundException("Öğretmen profili bulunamadı.");

            mevcut.BirimId = model.BirimId;
            mevcut.Email = model.Email;
            mevcut.OgretmenDurum = model.OgretmenDurum;

            if (model.GorselFile != null && model.GorselFile.Length > 0)
                mevcut.GorselPath = await SaveImageAsync(model.GorselFile, ct);

            await _db.SaveChangesAsync(ct);
        }

        public async Task SilAsync(int kullaniciId, CancellationToken ct = default)
        {
            var profil = await _db.OgretmenProfiller.FindAsync([kullaniciId], ct);
            if (profil == null) return;

            _db.OgretmenProfiller.Remove(profil);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<OgretmenProfilModel?> GetByIdAsync(int kullaniciId, CancellationToken ct = default)
            => await _db.OgretmenProfiller
                .Include(o => o.Birim)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.KullaniciId == kullaniciId, ct);

        private async Task<string> SaveImageAsync(IFormFile file, CancellationToken ct)
        {
            var root = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "ogretmen");
            Directory.CreateDirectory(root);

            var ext = Path.GetExtension(file.FileName);
            var name = $"ogr_{Guid.NewGuid():N}{ext}";
            var full = Path.Combine(root, name);

            using (var fs = new FileStream(full, FileMode.Create))
                await file.CopyToAsync(fs, ct);

            var rel = Path.Combine("uploads", "ogretmen", name).Replace("\\", "/");
            return "/" + rel;
        }
    }
}
