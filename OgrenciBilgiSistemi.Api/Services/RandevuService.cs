using Microsoft.Data.SqlClient;
using OgrenciBilgiSistemi.Api.Models;
using OgrenciBilgiSistemi.Shared.Enums;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Api.Services
{
    public class RandevuService
    {
        private readonly TenantBaglami _tenantBaglami;
        private string ConnectionString => _tenantBaglami.ConnectionString;

        private static readonly Dictionary<int, string> _durumAdlari = new()
        {
            [0] = "Beklemede",
            [1] = "Onaylandı",
            [2] = "Reddedildi",
            [3] = "İptal Edildi",
            [4] = "Tamamlandı"
        };

        public RandevuService(TenantBaglami tenantBaglami)
        {
            _tenantBaglami = tenantBaglami;
        }

        public async Task<List<RandevuModel>> KullanicininRandevulariniGetir(int kullaniciId, string rol, int sayfaNo = 1, int sayfaBoyutu = 20)
        {
            var liste = new List<RandevuModel>();
            const string query = @"
                SELECT r.RandevuId, r.OgretmenKullaniciId, r.VeliKullaniciId, r.OgrenciId,
                       r.RandevuTarihi, r.SureDakika, r.Durum, r.Not, r.OgretmenTarafindanOlusturuldu,
                       r.OlusturulmaTarihi,
                       og.KullaniciAdi AS OgretmenAd,
                       v.KullaniciAdi AS VeliAd,
                       o.OgrenciAdSoyad
                FROM Randevular r
                INNER JOIN Kullanicilar og ON r.OgretmenKullaniciId = og.KullaniciId
                INNER JOIN Kullanicilar v  ON r.VeliKullaniciId = v.KullaniciId
                LEFT  JOIN Ogrenciler o    ON r.OgrenciId = o.OgrenciId
                WHERE r.IsDeleted = 0
                  AND ((@rol = 'Ogretmen' AND r.OgretmenKullaniciId = @kullaniciId)
                    OR (@rol = 'Veli'      AND r.VeliKullaniciId = @kullaniciId))
                ORDER BY r.RandevuTarihi DESC
                OFFSET @offset ROWS FETCH NEXT @boyut ROWS ONLY";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@kullaniciId", kullaniciId);
            cmd.Parameters.AddWithValue("@rol", rol);
            cmd.Parameters.AddWithValue("@offset", (sayfaNo - 1) * sayfaBoyutu);
            cmd.Parameters.AddWithValue("@boyut", sayfaBoyutu);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var durum = reader.GetInt32(reader.GetOrdinal("Durum"));
                liste.Add(new RandevuModel
                {
                    RandevuId = reader.GetInt32(reader.GetOrdinal("RandevuId")),
                    OgretmenKullaniciId = reader.GetInt32(reader.GetOrdinal("OgretmenKullaniciId")),
                    OgretmenAdSoyad = reader["OgretmenAd"]?.ToString() ?? "",
                    VeliKullaniciId = reader.GetInt32(reader.GetOrdinal("VeliKullaniciId")),
                    VeliAdSoyad = reader["VeliAd"]?.ToString() ?? "",
                    OgrenciId = reader.IsDBNull(reader.GetOrdinal("OgrenciId")) ? null : reader.GetInt32(reader.GetOrdinal("OgrenciId")),
                    OgrenciAdSoyad = reader["OgrenciAdSoyad"]?.ToString(),
                    RandevuTarihi = reader.GetDateTime(reader.GetOrdinal("RandevuTarihi")),
                    SureDakika = reader.GetInt32(reader.GetOrdinal("SureDakika")),
                    Durum = durum,
                    DurumAdi = _durumAdlari.GetValueOrDefault(durum, "Bilinmiyor"),
                    Not = reader["Not"]?.ToString(),
                    OgretmenTarafindanOlusturuldu = reader.GetBoolean(reader.GetOrdinal("OgretmenTarafindanOlusturuldu")),
                    OlusturulmaTarihi = reader.GetDateTime(reader.GetOrdinal("OlusturulmaTarihi"))
                });
            }
            return liste;
        }

        public async Task<RandevuModel?> RandevuGetir(int randevuId)
        {
            const string query = @"
                SELECT r.RandevuId, r.OgretmenKullaniciId, r.VeliKullaniciId, r.OgrenciId,
                       r.RandevuTarihi, r.SureDakika, r.Durum, r.Not, r.OgretmenTarafindanOlusturuldu,
                       r.OlusturulmaTarihi,
                       og.KullaniciAdi AS OgretmenAd,
                       v.KullaniciAdi AS VeliAd,
                       o.OgrenciAdSoyad
                FROM Randevular r
                INNER JOIN Kullanicilar og ON r.OgretmenKullaniciId = og.KullaniciId
                INNER JOIN Kullanicilar v  ON r.VeliKullaniciId = v.KullaniciId
                LEFT  JOIN Ogrenciler o    ON r.OgrenciId = o.OgrenciId
                WHERE r.RandevuId = @id AND r.IsDeleted = 0";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", randevuId);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            var durum = reader.GetInt32(reader.GetOrdinal("Durum"));
            return new RandevuModel
            {
                RandevuId = reader.GetInt32(reader.GetOrdinal("RandevuId")),
                OgretmenKullaniciId = reader.GetInt32(reader.GetOrdinal("OgretmenKullaniciId")),
                OgretmenAdSoyad = reader["OgretmenAd"]?.ToString() ?? "",
                VeliKullaniciId = reader.GetInt32(reader.GetOrdinal("VeliKullaniciId")),
                VeliAdSoyad = reader["VeliAd"]?.ToString() ?? "",
                OgrenciId = reader.IsDBNull(reader.GetOrdinal("OgrenciId")) ? null : reader.GetInt32(reader.GetOrdinal("OgrenciId")),
                OgrenciAdSoyad = reader["OgrenciAdSoyad"]?.ToString(),
                RandevuTarihi = reader.GetDateTime(reader.GetOrdinal("RandevuTarihi")),
                SureDakika = reader.GetInt32(reader.GetOrdinal("SureDakika")),
                Durum = durum,
                DurumAdi = _durumAdlari.GetValueOrDefault(durum, "Bilinmiyor"),
                Not = reader["Not"]?.ToString(),
                OgretmenTarafindanOlusturuldu = reader.GetBoolean(reader.GetOrdinal("OgretmenTarafindanOlusturuldu")),
                OlusturulmaTarihi = reader.GetDateTime(reader.GetOrdinal("OlusturulmaTarihi"))
            };
        }

        public async Task<int> OgretmenRandevuOlustur(int ogretmenId, int veliId, int? ogrenciId, DateTime tarih, int sureDakika, string? not)
        {
            if (await CakismaVarMi(ogretmenId, tarih, sureDakika))
                throw new InvalidOperationException("Bu zaman aralığında zaten bir randevunuz bulunmaktadır.");

            return await RandevuEkle(ogretmenId, veliId, ogrenciId, tarih, sureDakika, not,
                ogretmenTarafindanOlusturuldu: true, durum: (int)RandevuDurumu.Onaylandi);
        }

        public async Task<int> VeliRandevuOlustur(int veliId, int ogretmenId, int? ogrenciId, DateTime tarih, int sureDakika, string? not)
        {
            if (await CakismaVarMi(ogretmenId, tarih, sureDakika))
                throw new InvalidOperationException("Öğretmenin bu zaman aralığında zaten bir randevusu bulunmaktadır.");

            return await RandevuEkle(ogretmenId, veliId, ogrenciId, tarih, sureDakika, not,
                ogretmenTarafindanOlusturuldu: false, durum: (int)RandevuDurumu.Beklemede);
        }

        private async Task<bool> CakismaVarMi(int ogretmenId, DateTime tarih, int sureDakika)
        {
            const string query = @"
                SELECT COUNT(*) FROM Randevular
                WHERE OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0
                  AND Durum IN (0, 1)
                  AND @tarih < DATEADD(MINUTE, SureDakika, RandevuTarihi)
                  AND DATEADD(MINUTE, @sure, @tarih) > RandevuTarihi";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@tarih", tarih);
            cmd.Parameters.AddWithValue("@sure", sureDakika);
            await conn.OpenAsync();
            return (int)(await cmd.ExecuteScalarAsync())! > 0;
        }

        public async Task<bool> DurumGuncelle(int randevuId, int ogretmenId, RandevuDurumu yeniDurum)
        {
            const string query = @"
                UPDATE Randevular SET Durum = @durum, GuncellenmeTarihi = GETDATE()
                WHERE RandevuId = @id AND OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", randevuId);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@durum", (int)yeniDurum);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IptalEt(int randevuId, int kullaniciId)
        {
            const string query = @"
                UPDATE Randevular SET Durum = @durum, GuncellenmeTarihi = GETDATE()
                WHERE RandevuId = @id AND IsDeleted = 0
                  AND (OgretmenKullaniciId = @kullaniciId OR VeliKullaniciId = @kullaniciId)";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", randevuId);
            cmd.Parameters.AddWithValue("@kullaniciId", kullaniciId);
            cmd.Parameters.AddWithValue("@durum", (int)RandevuDurumu.IptalEdildi);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private async Task<int> RandevuEkle(int ogretmenId, int veliId, int? ogrenciId, DateTime tarih, int sureDakika, string? not, bool ogretmenTarafindanOlusturuldu, int durum)
        {
            const string query = @"
                INSERT INTO Randevular
                    (OgretmenKullaniciId, VeliKullaniciId, OgrenciId, RandevuTarihi, SureDakika,
                     Durum, [Not], OgretmenTarafindanOlusturuldu, OlusturulmaTarihi, IsDeleted)
                OUTPUT INSERTED.RandevuId
                VALUES
                    (@ogretmenId, @veliId, @ogrenciId, @tarih, @sure,
                     @durum, @not, @olusturan, GETDATE(), 0)";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@veliId", veliId);
            cmd.Parameters.AddWithValue("@ogrenciId", (object?)ogrenciId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tarih", tarih);
            cmd.Parameters.AddWithValue("@sure", sureDakika);
            cmd.Parameters.AddWithValue("@durum", durum);
            cmd.Parameters.AddWithValue("@not", (object?)not ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@olusturan", ogretmenTarafindanOlusturuldu);
            await conn.OpenAsync();
            return (int)(await cmd.ExecuteScalarAsync())!;
        }
    }
}
