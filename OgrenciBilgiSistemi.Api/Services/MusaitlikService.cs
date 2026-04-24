using Microsoft.Data.SqlClient;
using OgrenciBilgiSistemi.Api.Models;
using OgrenciBilgiSistemi.Shared.Enums;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Api.Services
{
    public class MusaitlikService
    {
        private readonly TenantBaglami _tenantBaglami;
        private string ConnectionString => _tenantBaglami.ConnectionString;

        private static readonly Dictionary<int, string> _gunAdlari = new()
        {
            [1] = "Pazartesi", [2] = "Salı", [3] = "Çarşamba",
            [4] = "Perşembe", [5] = "Cuma", [6] = "Cumartesi", [7] = "Pazar"
        };

        // GunEnum → .NET DayOfWeek eşlemesi
        private static readonly Dictionary<int, DayOfWeek> _gunMap = new()
        {
            [1] = DayOfWeek.Monday, [2] = DayOfWeek.Tuesday, [3] = DayOfWeek.Wednesday,
            [4] = DayOfWeek.Thursday, [5] = DayOfWeek.Friday, [6] = DayOfWeek.Saturday, [7] = DayOfWeek.Sunday
        };

        public MusaitlikService(TenantBaglami tenantBaglami)
        {
            _tenantBaglami = tenantBaglami;
        }

        public async Task<List<MusaitlikModel>> OgretmeninMusaitlikleriniGetir(int ogretmenId)
        {
            var liste = new List<MusaitlikModel>();
            const string query = @"
                SELECT MusaitlikId, OgretmenKullaniciId, Gun, BaslangicSaati, BitisSaati
                FROM OgretmenMusaitlikler
                WHERE OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0
                ORDER BY Gun, BaslangicSaati";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var gun = reader.GetInt32(reader.GetOrdinal("Gun"));
                liste.Add(new MusaitlikModel
                {
                    MusaitlikId = reader.GetInt32(reader.GetOrdinal("MusaitlikId")),
                    OgretmenKullaniciId = reader.GetInt32(reader.GetOrdinal("OgretmenKullaniciId")),
                    Gun = gun,
                    GunAdi = _gunAdlari.GetValueOrDefault(gun, "Bilinmiyor"),
                    BaslangicSaati = reader.GetTimeSpan(reader.GetOrdinal("BaslangicSaati")).ToString(@"hh\:mm"),
                    BitisSaati = reader.GetTimeSpan(reader.GetOrdinal("BitisSaati")).ToString(@"hh\:mm")
                });
            }
            return liste;
        }

        public async Task<int> Ekle(int ogretmenId, int gun, TimeSpan baslangic, TimeSpan bitis)
        {
            const string query = @"
                INSERT INTO OgretmenMusaitlikler (OgretmenKullaniciId, Gun, BaslangicSaati, BitisSaati, IsDeleted)
                OUTPUT INSERTED.MusaitlikId
                VALUES (@ogretmenId, @gun, @baslangic, @bitis, 0)";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@gun", gun);
            cmd.Parameters.AddWithValue("@baslangic", baslangic);
            cmd.Parameters.AddWithValue("@bitis", bitis);
            await conn.OpenAsync();
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task<bool> Sil(int musaitlikId, int ogretmenId)
        {
            const string query = @"
                UPDATE OgretmenMusaitlikler SET IsDeleted = 1
                WHERE MusaitlikId = @id AND OgretmenKullaniciId = @ogretmenId";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", musaitlikId);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// Haftalık tekrarlayan slotları somut tarihlere genişletir ve dolu olanları çıkarır.
        /// 30 dakikalık bloklar halinde döner.
        /// </summary>
        public async Task<List<MusaitSlotModel>> MusaitSlotlariGetir(int ogretmenId, DateTime baslangicTarih, DateTime bitisTarih)
        {
            // 1. Haftalık slotları al
            var musaitlikler = await OgretmeninMusaitlikleriniGetir(ogretmenId);

            // 2. Mevcut onaylı/bekleyen randevuları al
            var doluSlotlar = new HashSet<DateTime>();
            const string randevuQuery = @"
                SELECT RandevuTarihi, SureDakika FROM Randevular
                WHERE OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0
                  AND Durum IN (0, 1)
                  AND RandevuTarihi BETWEEN @baslangic AND @bitis";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(randevuQuery, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@baslangic", baslangicTarih);
            cmd.Parameters.AddWithValue("@bitis", bitisTarih);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tarih = reader.GetDateTime(0);
                var sure = reader.GetInt32(1);
                // Randevunun kapsadığı tüm 30dk blokları işaretle
                for (int i = 0; i < sure; i += 30)
                    doluSlotlar.Add(tarih.AddMinutes(i));
            }

            // 3. Her gün için slotları genişlet
            var sonuc = new List<MusaitSlotModel>();
            for (var gun = baslangicTarih.Date; gun <= bitisTarih.Date; gun = gun.AddDays(1))
            {
                var haftaninGunu = gun.DayOfWeek;
                foreach (var m in musaitlikler)
                {
                    if (!_gunMap.TryGetValue(m.Gun, out var dotnetGun) || dotnetGun != haftaninGunu)
                        continue;

                    var baslangic = TimeSpan.Parse(m.BaslangicSaati);
                    var bitis = TimeSpan.Parse(m.BitisSaati);

                    for (var saat = baslangic; saat + TimeSpan.FromMinutes(30) <= bitis; saat += TimeSpan.FromMinutes(30))
                    {
                        var slotTarih = gun + saat;
                        if (slotTarih <= DateTime.Now) continue; // Geçmiş slotları atla
                        if (doluSlotlar.Contains(slotTarih)) continue; // Dolu slotları atla

                        sonuc.Add(new MusaitSlotModel
                        {
                            Tarih = slotTarih,
                            BaslangicSaati = saat.ToString(@"hh\:mm"),
                            BitisSaati = (saat + TimeSpan.FromMinutes(30)).ToString(@"hh\:mm"),
                            OgretmenKullaniciId = ogretmenId
                        });
                    }
                }
            }

            return sonuc.OrderBy(s => s.Tarih).ToList();
        }
    }
}
