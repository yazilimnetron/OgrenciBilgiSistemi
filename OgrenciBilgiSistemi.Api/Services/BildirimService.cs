using Microsoft.Data.SqlClient;
using OgrenciBilgiSistemi.Api.Models;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Api.Services
{
    public class BildirimService
    {
        private readonly TenantBaglami _tenantBaglami;
        private string ConnectionString => _tenantBaglami.ConnectionString;

        public BildirimService(TenantBaglami tenantBaglami)
        {
            _tenantBaglami = tenantBaglami;
        }

        public async Task Olustur(int aliciKullaniciId, int tur, string mesaj, int? randevuId)
        {
            const string query = @"
                INSERT INTO Bildirimler (AliciKullaniciId, Tur, Mesaj, RandevuId, Okundu, OlusturulmaTarihi, IsDeleted)
                VALUES (@aliciId, @tur, @mesaj, @randevuId, 0, GETDATE(), 0)";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@aliciId", aliciKullaniciId);
            cmd.Parameters.AddWithValue("@tur", tur);
            cmd.Parameters.AddWithValue("@mesaj", mesaj);
            cmd.Parameters.AddWithValue("@randevuId", (object?)randevuId ?? DBNull.Value);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<BildirimModel>> KullanicininBildirimleriniGetir(int kullaniciId, int sayfaNo = 1, int sayfaBoyutu = 20)
        {
            var liste = new List<BildirimModel>();
            const string query = @"
                SELECT BildirimId, Tur, Mesaj, RandevuId, Okundu, OlusturulmaTarihi
                FROM Bildirimler
                WHERE AliciKullaniciId = @kullaniciId AND IsDeleted = 0
                ORDER BY OlusturulmaTarihi DESC
                OFFSET @offset ROWS FETCH NEXT @boyut ROWS ONLY";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@kullaniciId", kullaniciId);
            cmd.Parameters.AddWithValue("@offset", (sayfaNo - 1) * sayfaBoyutu);
            cmd.Parameters.AddWithValue("@boyut", sayfaBoyutu);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                liste.Add(new BildirimModel
                {
                    BildirimId = reader.GetInt32(reader.GetOrdinal("BildirimId")),
                    Tur = reader.GetInt32(reader.GetOrdinal("Tur")),
                    Mesaj = reader.GetString(reader.GetOrdinal("Mesaj")),
                    RandevuId = reader.IsDBNull(reader.GetOrdinal("RandevuId")) ? null : reader.GetInt32(reader.GetOrdinal("RandevuId")),
                    Okundu = reader.GetBoolean(reader.GetOrdinal("Okundu")),
                    OlusturulmaTarihi = reader.GetDateTime(reader.GetOrdinal("OlusturulmaTarihi"))
                });
            }
            return liste;
        }

        public async Task<int> OkunmamisSayisi(int kullaniciId)
        {
            const string query = "SELECT COUNT(*) FROM Bildirimler WHERE AliciKullaniciId = @id AND Okundu = 0 AND IsDeleted = 0";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", kullaniciId);
            await conn.OpenAsync();
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task<bool> OkunduIsaretle(int bildirimId, int kullaniciId)
        {
            const string query = "UPDATE Bildirimler SET Okundu = 1 WHERE BildirimId = @id AND AliciKullaniciId = @kullaniciId AND IsDeleted = 0";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", bildirimId);
            cmd.Parameters.AddWithValue("@kullaniciId", kullaniciId);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> TumunuOkunduIsaretle(int kullaniciId)
        {
            const string query = "UPDATE Bildirimler SET Okundu = 1 WHERE AliciKullaniciId = @id AND Okundu = 0 AND IsDeleted = 0";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", kullaniciId);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
