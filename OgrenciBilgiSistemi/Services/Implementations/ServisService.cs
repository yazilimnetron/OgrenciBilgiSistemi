using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public sealed class ServisService : IServisService
    {
        private readonly AppDbContext _db;

        public ServisService(AppDbContext db) => _db = db;

        public async Task<SayfalanmisListeModel<ServisModel>> SearchPagedAsync(
            string? searchString, int page, int pageSize = 20, CancellationToken ct = default)
        {
            var query = _db.Servisler
                .Include(s => s.Kullanici)
                .Include(s => s.Ogrenciler)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim();
                query = query.Where(srv => srv.Plaka.Contains(s) ||
                    (srv.Kullanici != null && srv.Kullanici.KullaniciAdi.Contains(s)));
            }

            return await SayfalanmisListeModel<ServisModel>
                .CreateAsync(query.OrderBy(s => s.Plaka), page, pageSize, ct);
        }

        public Task<ServisModel?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.Servisler.FindAsync([id], ct).AsTask();

        public async Task EkleAsync(ServisModel model, CancellationToken ct = default)
        {
            _db.Servisler.Add(model);
            await _db.SaveChangesAsync(ct);
        }

        public async Task GuncelleAsync(ServisModel model, CancellationToken ct = default)
        {
            var servis = await _db.Servisler.FindAsync([model.ServisId], ct)
                ?? throw new KeyNotFoundException("Servis bulunamadı.");

            servis.Plaka = model.Plaka;
            servis.KullaniciId = model.KullaniciId;
            servis.ServisDurum = model.ServisDurum;

            await _db.SaveChangesAsync(ct);
        }

        public async Task SilAsync(int id, CancellationToken ct = default)
        {
            var servis = await _db.Servisler.FindAsync([id], ct);
            if (servis == null) return;

            servis.ServisDurum = false;
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<SelectListItem>> GetSoforSelectListAsync(
            int? selectedId = null, CancellationToken ct = default)
            => await _db.Kullanicilar
                .AsNoTracking()
                .Where(k => k.KullaniciDurum && k.Rol == KullaniciRolu.Sofor)
                .OrderBy(k => k.KullaniciAdi)
                .Select(k => new SelectListItem
                {
                    Value = k.KullaniciId.ToString(),
                    Text = k.KullaniciAdi,
                    Selected = selectedId.HasValue && k.KullaniciId == selectedId.Value
                })
                .ToListAsync(ct);
    }
}
