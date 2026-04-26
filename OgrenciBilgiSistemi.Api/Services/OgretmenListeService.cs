using Microsoft.Data.SqlClient;
using OgrenciBilgiSistemi.Api.Models;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Api.Services
{
    public class OgretmenListeService
    {
        private readonly TenantBaglami _tenantBaglami;
        private string ConnectionString => _tenantBaglami.ConnectionString;

        public OgretmenListeService(TenantBaglami tenantBaglami)
        {
            _tenantBaglami = tenantBaglami;
        }

        public async Task<List<OgretmenListeModel>> AktifOgretmenleriGetir()
        {
            var liste = new List<OgretmenListeModel>();
            const string query = @"
                SELECT k.KullaniciId, k.KullaniciAdi
                FROM Kullanicilar k
                INNER JOIN OgretmenProfiller op ON k.KullaniciId = op.KullaniciId
                WHERE k.KullaniciDurum = 1 AND k.Rol = 2 AND op.OgretmenDurum = 1
                ORDER BY k.KullaniciAdi";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                liste.Add(new OgretmenListeModel
                {
                    KullaniciId = reader.GetInt32(reader.GetOrdinal("KullaniciId")),
                    KullaniciAdi = reader.GetString(reader.GetOrdinal("KullaniciAdi"))
                });
            }
            return liste;
        }
    }
}
