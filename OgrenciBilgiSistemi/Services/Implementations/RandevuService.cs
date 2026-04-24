using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;
using OgrenciBilgiSistemi.Shared.Enums;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public class RandevuService : IRandevuService
    {
        private readonly AppDbContext _db;

        public RandevuService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SayfalanmisListeModel<RandevuModel>> AraVeListele(
            string? arama, int? ogretmenId, RandevuDurumu? durum,
            DateTime? baslangic, DateTime? bitis,
            int sayfaNo, int sayfaBoyutu = 20, CancellationToken ct = default)
        {
            var query = _db.Randevular
                .AsNoTracking()
                .Include(r => r.Ogretmen)
                .Include(r => r.Veli)
                .Include(r => r.Ogrenci)
                .AsQueryable();

            if (ogretmenId.HasValue)
                query = query.Where(r => r.OgretmenKullaniciId == ogretmenId.Value);

            if (durum.HasValue)
                query = query.Where(r => r.Durum == durum.Value);

            if (baslangic.HasValue)
                query = query.Where(r => r.RandevuTarihi >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(r => r.RandevuTarihi <= bitis.Value);

            if (!string.IsNullOrWhiteSpace(arama))
            {
                var q = arama.Trim();
                query = query.Where(r =>
                    r.Ogretmen.KullaniciAdi.Contains(q) ||
                    r.Veli.KullaniciAdi.Contains(q) ||
                    (r.Ogrenci != null && r.Ogrenci.OgrenciAdSoyad.Contains(q)));
            }

            query = query.OrderByDescending(r => r.RandevuTarihi);

            return await SayfalanmisListeModel<RandevuModel>.CreateAsync(query, sayfaNo, sayfaBoyutu, ct);
        }

        public async Task<RandevuModel?> IdIleGetir(int randevuId, CancellationToken ct = default)
        {
            return await _db.Randevular
                .AsNoTracking()
                .Include(r => r.Ogretmen)
                .Include(r => r.Veli)
                .Include(r => r.Ogrenci)
                .FirstOrDefaultAsync(r => r.RandevuId == randevuId, ct);
        }

        public async Task IptalEt(int randevuId, CancellationToken ct = default)
        {
            var randevu = await _db.Randevular.FindAsync(new object[] { randevuId }, ct)
                          ?? throw new KeyNotFoundException("Randevu bulunamadı.");

            randevu.Durum = RandevuDurumu.IptalEdildi;
            randevu.GuncellenmeTarihi = DateTime.Now;
            await _db.SaveChangesAsync(ct);
        }
    }
}
