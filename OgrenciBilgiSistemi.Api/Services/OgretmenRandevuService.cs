using Microsoft.Data.SqlClient;
using OgrenciBilgiSistemi.Api.Models;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Api.Services
{
    public class OgretmenRandevuService
    {
        private readonly TenantBaglami _tenantBaglami;
        private string ConnectionString => _tenantBaglami.ConnectionString;

        public OgretmenRandevuService(TenantBaglami tenantBaglami)
        {
            _tenantBaglami = tenantBaglami;
        }

        public async Task<List<OgretmenRandevuTakvimModel>> OgretmeninRandevuTakviminiGetir(int ogretmenId)
        {
            var liste = new List<OgretmenRandevuTakvimModel>();
            const string query = @"
                SELECT OgretmenRandevuId, OgretmenKullaniciId, Tarih, BaslangicSaati, BitisSaati
                FROM OgretmenRandevular
                WHERE OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0
                ORDER BY Tarih, BaslangicSaati";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                liste.Add(new OgretmenRandevuTakvimModel
                {
                    OgretmenRandevuId = reader.GetInt32(reader.GetOrdinal("OgretmenRandevuId")),
                    OgretmenKullaniciId = reader.GetInt32(reader.GetOrdinal("OgretmenKullaniciId")),
                    Tarih = reader.GetDateTime(reader.GetOrdinal("Tarih")),
                    BaslangicSaati = reader.GetTimeSpan(reader.GetOrdinal("BaslangicSaati")).ToString(@"hh\:mm"),
                    BitisSaati = reader.GetTimeSpan(reader.GetOrdinal("BitisSaati")).ToString(@"hh\:mm")
                });
            }
            return liste;
        }

        public async Task<int> Ekle(int ogretmenId, DateTime tarih, TimeSpan baslangic, TimeSpan bitis)
        {
            const string query = @"
                INSERT INTO OgretmenRandevular (OgretmenKullaniciId, Tarih, BaslangicSaati, BitisSaati, IsDeleted)
                OUTPUT INSERTED.OgretmenRandevuId
                VALUES (@ogretmenId, @tarih, @baslangic, @bitis, 0)";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@tarih", tarih.Date);
            cmd.Parameters.AddWithValue("@baslangic", baslangic);
            cmd.Parameters.AddWithValue("@bitis", bitis);
            await conn.OpenAsync();
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task<bool> Sil(int ogretmenRandevuId, int ogretmenId)
        {
            const string query = @"
                UPDATE OgretmenRandevular SET IsDeleted = 1
                WHERE OgretmenRandevuId = @id AND OgretmenKullaniciId = @ogretmenId";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", ogretmenRandevuId);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<RandevuSlotModel>> RandevuSlotlariGetir(int ogretmenId, DateTime baslangicTarih, DateTime bitisTarih)
        {
            var takvimler = new List<OgretmenRandevuTakvimModel>();
            const string takvimQuery = @"
                SELECT OgretmenRandevuId, OgretmenKullaniciId, Tarih, BaslangicSaati, BitisSaati
                FROM OgretmenRandevular
                WHERE OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0
                  AND Tarih BETWEEN @baslangic AND @bitis
                ORDER BY Tarih, BaslangicSaati";

            await using var conn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand(takvimQuery, conn);
            cmd.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd.Parameters.AddWithValue("@baslangic", baslangicTarih.Date);
            cmd.Parameters.AddWithValue("@bitis", bitisTarih.Date);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                takvimler.Add(new OgretmenRandevuTakvimModel
                {
                    OgretmenRandevuId = reader.GetInt32(reader.GetOrdinal("OgretmenRandevuId")),
                    OgretmenKullaniciId = reader.GetInt32(reader.GetOrdinal("OgretmenKullaniciId")),
                    Tarih = reader.GetDateTime(reader.GetOrdinal("Tarih")),
                    BaslangicSaati = reader.GetTimeSpan(reader.GetOrdinal("BaslangicSaati")).ToString(@"hh\:mm"),
                    BitisSaati = reader.GetTimeSpan(reader.GetOrdinal("BitisSaati")).ToString(@"hh\:mm")
                });
            }
            await reader.CloseAsync();

            var doluSlotlar = new HashSet<DateTime>();
            const string randevuQuery = @"
                SELECT RandevuTarihi, SureDakika FROM Randevular
                WHERE OgretmenKullaniciId = @ogretmenId AND IsDeleted = 0
                  AND Durum IN (0, 1)
                  AND RandevuTarihi BETWEEN @baslangic AND @bitis";

            await using var cmd2 = new SqlCommand(randevuQuery, conn);
            cmd2.Parameters.AddWithValue("@ogretmenId", ogretmenId);
            cmd2.Parameters.AddWithValue("@baslangic", baslangicTarih);
            cmd2.Parameters.AddWithValue("@bitis", bitisTarih);

            await using var reader2 = await cmd2.ExecuteReaderAsync();
            while (await reader2.ReadAsync())
            {
                var tarih = reader2.GetDateTime(0);
                var sure = reader2.GetInt32(1);
                for (int i = 0; i < sure; i += 30)
                    doluSlotlar.Add(tarih.AddMinutes(i));
            }

            var sonuc = new List<RandevuSlotModel>();
            foreach (var t in takvimler)
            {
                var baslangic = TimeSpan.Parse(t.BaslangicSaati);
                var bitis = TimeSpan.Parse(t.BitisSaati);

                for (var saat = baslangic; saat + TimeSpan.FromMinutes(30) <= bitis; saat += TimeSpan.FromMinutes(30))
                {
                    var slotTarih = t.Tarih.Date + saat;
                    if (slotTarih <= DateTime.Now) continue;
                    if (doluSlotlar.Contains(slotTarih)) continue;

                    sonuc.Add(new RandevuSlotModel
                    {
                        Tarih = slotTarih,
                        BaslangicSaati = saat.ToString(@"hh\:mm"),
                        BitisSaati = (saat + TimeSpan.FromMinutes(30)).ToString(@"hh\:mm"),
                        OgretmenKullaniciId = ogretmenId
                    });
                }
            }

            return sonuc.OrderBy(s => s.Tarih).ToList();
        }
    }
}
