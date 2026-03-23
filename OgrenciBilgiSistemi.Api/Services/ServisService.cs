using Microsoft.Data.SqlClient;
using OgrenciBilgiSistemi.Api.Models;

namespace OgrenciBilgiSistemi.Api.Services
{
    public class ServisService
    {
        private readonly string _connectionString;

        public ServisService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection bağlantı dizesi eksik.");
        }

        /// <summary>
        /// Belirtilen şoföre (KullaniciId) atanmış aktif öğrencileri getirir.
        /// </summary>
        public async Task<List<OgrenciModel>> ServisOgrencileriGetir(int soforKullaniciId)
        {
            var ogrenciler = new List<OgrenciModel>();
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                const string query = @"
                    SELECT O.OgrenciId, O.OgrenciAdSoyad, O.OgrenciGorsel, O.BirimId,
                           B.BirimAd AS SinifAdi
                    FROM Ogrenciler O
                    LEFT JOIN Birimler B ON O.BirimId = B.BirimId
                    WHERE O.ServisId = @soforId AND O.OgrenciDurum = 1
                    ORDER BY O.OgrenciAdSoyad";

                await using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@soforId", soforKullaniciId);
                await conn.OpenAsync();

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string rawFileName = reader["OgrenciGorsel"]?.ToString() ?? string.Empty;
                    ogrenciler.Add(new OgrenciModel
                    {
                        OgrenciId      = (int)reader["OgrenciId"],
                        OgrenciAdSoyad = reader["OgrenciAdSoyad"]?.ToString() ?? string.Empty,
                        OgrenciGorsel  = string.IsNullOrEmpty(rawFileName) ? "user_icon.png" : rawFileName,
                        BirimId        = reader["BirimId"] as int?,
                        SinifAdi       = reader["SinifAdi"]?.ToString()
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Servis öğrenci listesi alınamadı.", ex);
            }
            return ogrenciler;
        }

        /// <summary>
        /// Belirtilen şoförün servis profil bilgilerini getirir.
        /// </summary>
        public async Task<ServisProfilModel?> ServisProfilGetir(int soforKullaniciId)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                const string query = @"
                    SELECT KullaniciId, Plaka, SoforTelefon, ServisDurum
                    FROM ServisProfiller
                    WHERE KullaniciId = @kullaniciId";

                await using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kullaniciId", soforKullaniciId);
                await conn.OpenAsync();

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ServisProfilModel
                    {
                        KullaniciId = (int)reader["KullaniciId"],
                        Plaka       = reader["Plaka"]?.ToString() ?? string.Empty,
                        SoforTelefon = reader["SoforTelefon"]?.ToString(),
                        ServisDurum = Convert.ToBoolean(reader["ServisDurum"])
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Servis profil bilgisi alınamadı.", ex);
            }
            return null;
        }

        /// <summary>
        /// Belirtilen şoförün bugünkü yoklamasını periyoda göre getirir.
        /// </summary>
        public async Task<Dictionary<int, int>> MevcutServisYoklamaGetir(int soforKullaniciId, int periyot)
        {
            var yoklamaDict = new Dictionary<int, int>();

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                const string query = @"
                    SELECT SY.OgrenciId, SY.DurumId
                    FROM ServisYoklamalar SY
                    WHERE SY.KullaniciId = @kullaniciId
                      AND SY.Periyot = @periyot
                      AND CAST(SY.OlusturulmaTarihi AS DATE) = CAST(GETDATE() AS DATE)";

                await using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kullaniciId", soforKullaniciId);
                cmd.Parameters.AddWithValue("@periyot", periyot);
                await conn.OpenAsync();

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    yoklamaDict[(int)reader["OgrenciId"]] = (int)reader["DurumId"];
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Servis yoklama bilgisi alınamadı.", ex);
            }
            return yoklamaDict;
        }

        /// <summary>
        /// Servis yoklamasını toplu olarak kaydeder veya günceller.
        /// </summary>
        public async Task ServisYoklamaKaydet(
            IEnumerable<(int OgrenciId, int DurumId)> yoklamaVerisi,
            int kullaniciId,
            int periyot)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

            try
            {
                const string query = @"
                    DECLARE @Bugun DATE = CAST(GETDATE() AS DATE);
                    MERGE INTO ServisYoklamalar AS target
                    USING (SELECT @ogrenciId AS OgrenciId) AS source
                    ON (target.OgrenciId = source.OgrenciId
                        AND target.KullaniciId = @kullaniciId
                        AND target.Periyot = @periyot
                        AND CAST(target.OlusturulmaTarihi AS DATE) = @Bugun)
                    WHEN MATCHED THEN
                        UPDATE SET DurumId = @durumId, GuncellenmeTarihi = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (OgrenciId, KullaniciId, DurumId, Periyot, OlusturulmaTarihi)
                        VALUES (@ogrenciId, @kullaniciId, @durumId, @periyot, GETDATE());";

                foreach (var kayit in yoklamaVerisi)
                {
                    await using var cmd = new SqlCommand(query, conn, transaction);
                    cmd.Parameters.AddWithValue("@ogrenciId", kayit.OgrenciId);
                    cmd.Parameters.AddWithValue("@durumId", kayit.DurumId);
                    cmd.Parameters.AddWithValue("@kullaniciId", kullaniciId);
                    cmd.Parameters.AddWithValue("@periyot", periyot);
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
