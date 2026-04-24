using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;
using OgrenciBilgiSistemi.Shared.Enums;

namespace OgrenciBilgiSistemi.Services.Implementations
{
    public class BildirimService : IBildirimService
    {
        private readonly AppDbContext _db;

        public BildirimService(AppDbContext db)
        {
            _db = db;
        }

        public async Task Olustur(int aliciKullaniciId, BildirimTuru tur, string mesaj, int? randevuId = null, CancellationToken ct = default)
        {
            var bildirim = new BildirimModel
            {
                AliciKullaniciId = aliciKullaniciId,
                Tur = tur,
                Mesaj = mesaj,
                RandevuId = randevuId,
                Okundu = false,
                OlusturulmaTarihi = DateTime.Now
            };

            _db.Bildirimler.Add(bildirim);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<int> OkunmamisSayisi(int kullaniciId, CancellationToken ct = default)
        {
            return await _db.Bildirimler
                .AsNoTracking()
                .Where(b => b.AliciKullaniciId == kullaniciId && !b.Okundu)
                .CountAsync(ct);
        }
    }
}
