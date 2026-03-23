using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Dtos;
using OgrenciBilgiSistemi.Api.Services;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [Route("api/ogrenciler")]
    [ApiController]
    [Authorize]
    public class OgrencilerController : ControllerBase
    {
        private readonly OgrenciService _ogrenciService;
        private readonly ServisService _servisService;

        public OgrencilerController(OgrenciService ogrenciService, ServisService servisService)
        {
            _ogrenciService = ogrenciService;
            _servisService = servisService;
        }

        #region Rol Bazlı Öğrenci Metotları

        // Rol bazlı: Giriş yapan kullanıcıya ait öğrencileri getirir
        // Öğretmen → tüm sınıflar, Şoför → kendi servisi, Veli → kendi çocukları
        [HttpGet("benim")]
        public async Task<IActionResult> BenimOgrencilerim()
        {
            try
            {
                var rol = User.FindFirst("rol")?.Value;
                var kullaniciId = User.FindFirst("kullaniciId")?.Value;

                if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(kullaniciId))
                    return Unauthorized("Oturum bilgileri eksik.");

                switch (rol)
                {
                    case "Veli":
                        var veliIdStr = User.FindFirst("veliId")?.Value;
                        if (string.IsNullOrEmpty(veliIdStr) || !int.TryParse(veliIdStr, out var veliId))
                            return BadRequest("Veli bilgisi bulunamadı.");
                        var cocuklar = await _ogrenciService.VeliyeGoreOgrencileriGetirAsync(veliId);
                        return Ok(cocuklar);

                    case "Sofor":
                        var servisIdStr = User.FindFirst("servisId")?.Value;
                        if (string.IsNullOrEmpty(servisIdStr) || !int.TryParse(servisIdStr, out var servisId))
                            return BadRequest("Servis bilgisi bulunamadı.");
                        var servisOgrencileri = await _servisService.ServisOgrencileriGetir(servisId);
                        return Ok(servisOgrencileri);

                    default:
                        // Öğretmen: tüm sınıfları görebilir, bu endpoint sınıf listesine yönlendirir
                        return Ok(new { mesaj = "Öğretmenler sınıf listesinden öğrenci görüntüler.", rol });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci listesi alınırken bir hata oluştu." });
            }
        }

        #endregion

        #region Öğrenci Bilgi Metotları

        // 1. Sınıf ID'sine göre öğrenci listesini getirir
        [HttpGet("class/{sinifId}")]
        public async Task<IActionResult> SinifaGoreGetir(int sinifId)
        {
            try
            {
                var ogrenciler = await _ogrenciService.SinifaGoreOgrencileriGetirAsync(sinifId);
                return Ok(ogrenciler);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci listesi alınırken bir hata oluştu." });
            }
        }

        // 2. ID'ye göre tek öğrenci getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> IdIleGetir(int id)
        {
            try
            {
                var ogrenci = await _ogrenciService.OgrenciGetirAsync(id);
                if (ogrenci is null)
                    return NotFound(new { message = $"{id} numaralı öğrenci bulunamadı." });

                var yetkiSonucu = OgrenciyeErisimKontrol(ogrenci.VeliId, ogrenci.ServisId);
                if (yetkiSonucu != null) return yetkiSonucu;

                return Ok(ogrenci);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci bilgisi alınırken bir hata oluştu." });
            }
        }

        // 3. Öğrencinin tüm detaylarını (Veli, Servis vb.) getirir
        [HttpGet("{id}/details")]
        public async Task<IActionResult> DetayGetir(int id)
        {
            try
            {
                // Önce öğrenciyi getirip yetki kontrolü yap
                var ogrenci = await _ogrenciService.OgrenciGetirAsync(id);
                if (ogrenci is null)
                    return NotFound(new { message = $"{id} numaralı öğrenci bulunamadı." });

                var yetkiSonucu = OgrenciyeErisimKontrol(ogrenci.VeliId, ogrenci.ServisId);
                if (yetkiSonucu != null) return yetkiSonucu;

                var detaylar = await _ogrenciService.OgrenciDetayGetirAsync(id);
                if (detaylar.Count == 0)
                    return NotFound(new { message = $"{id} numaralı öğrenci bulunamadı." });
                return Ok(detaylar);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci detayları alınırken bir hata oluştu." });
            }
        }

        #endregion

        #region Öğrenci CRUD (Admin Only)

        // 4. Yeni öğrenci ekler — yalnızca admin
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Ekle([FromBody] OgrenciKaydetDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OgrenciAdSoyad))
                return BadRequest(new { error = "OgrenciAdSoyad boş olamaz." });

            if (dto.OgrenciNo <= 0)
                return BadRequest(new { error = "OgrenciNo sıfırdan büyük olmalıdır." });

            try
            {
                int yeniId = await _ogrenciService.EkleAsync(dto);
                return CreatedAtAction(nameof(IdIleGetir), new { id = yeniId }, new { ogrenciId = yeniId });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci eklenirken bir hata oluştu." });
            }
        }

        // 5. Mevcut öğrenciyi günceller — yalnızca admin
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Guncelle(int id, [FromBody] OgrenciKaydetDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OgrenciAdSoyad))
                return BadRequest(new { error = "OgrenciAdSoyad boş olamaz." });

            if (dto.OgrenciNo <= 0)
                return BadRequest(new { error = "OgrenciNo sıfırdan büyük olmalıdır." });

            try
            {
                bool basarili = await _ogrenciService.GuncelleAsync(id, dto);
                if (!basarili)
                    return NotFound(new { message = $"{id} numaralı öğrenci bulunamadı." });
                return Ok(new { message = "Öğrenci başarıyla güncellendi." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci güncellenirken bir hata oluştu." });
            }
        }

        // 6. Öğrenciyi pasife alır (soft-delete) — yalnızca admin
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Sil(int id)
        {
            try
            {
                bool basarili = await _ogrenciService.SilAsync(id);
                if (!basarili)
                    return NotFound(new { message = $"{id} numaralı öğrenci bulunamadı." });
                return Ok(new { message = "Öğrenci pasife alındı." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Öğrenci silinirken bir hata oluştu." });
            }
        }

        #endregion

        #region Yoklama Metotları

        // 7. Öğrencinin haftalık yoklama geçmişini getirir
        [HttpGet("{id}/weekly-attendance")]
        public async Task<IActionResult> HaftalikYoklamaGetir(int id, [FromQuery] DateTime baslangic, [FromQuery] DateTime bitis)
        {
            try
            {
                var ogrenci = await _ogrenciService.OgrenciGetirAsync(id);
                if (ogrenci is null)
                    return NotFound(new { message = $"{id} numaralı öğrenci bulunamadı." });

                var yetkiSonucu = OgrenciyeErisimKontrol(ogrenci.VeliId, ogrenci.ServisId);
                if (yetkiSonucu != null) return yetkiSonucu;

                var yoklamalar = await _ogrenciService.HaftalikYoklamaGetirAsync(id, baslangic, bitis);
                return Ok(yoklamalar);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Haftalık yoklama bilgisi alınırken bir hata oluştu." });
            }
        }

        // 8. Mevcut yoklama durumunu getirir (Dictionary döner)
        [HttpGet("attendance/{sinifId}/{dersNumarasi}")]
        public async Task<IActionResult> YoklamaGetir(int sinifId, int dersNumarasi)
        {
            try
            {
                var yoklama = await _ogrenciService.MevcutYoklamaGetirAsync(sinifId, dersNumarasi);
                return Ok(yoklama);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Yoklama bilgisi alınırken bir hata oluştu." });
            }
        }

        // 8. Toplu yoklama kaydetme (POST)
        [HttpPost("attendance/save-bulk")]
        public async Task<IActionResult> TopluYoklamaKaydet([FromBody] TopluYoklamaGuncelleDto model)
        {
            if (model.Kayitlar == null || model.Kayitlar.Count == 0)
                return BadRequest(new { error = "Yoklama kaydı listesi boş olamaz." });

            try
            {
                var formatliVeri = model.Kayitlar.Select(k => (k.OgrenciId, k.DurumId));

                await _ogrenciService.TopluYoklamaKaydetAsync(
                    formatliVeri,
                    model.SinifId,
                    model.KullaniciId,
                    model.DersNumarasi
                );

                return Ok(new { message = "Yoklama başarıyla kaydedildi." });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Yoklama kaydedilirken bir hata oluştu." });
            }
        }

        #endregion

        #region Yardımcı Metotlar

        /// <summary>
        /// Rol bazlı öğrenci erişim kontrolü.
        /// Veli sadece kendi çocuğunu, şoför sadece kendi servisindeki öğrenciyi görebilir.
        /// Öğretmen tüm öğrencilere erişebilir.
        /// </summary>
        private IActionResult? OgrenciyeErisimKontrol(int? ogrenciVeliId, int? ogrenciServisId)
        {
            var rol = User.FindFirst("rol")?.Value;

            switch (rol)
            {
                case "Veli":
                    var veliIdStr = User.FindFirst("veliId")?.Value;
                    if (!int.TryParse(veliIdStr, out var veliId) || ogrenciVeliId != veliId)
                        return Forbid();
                    break;

                case "Sofor":
                    var servisIdStr = User.FindFirst("servisId")?.Value;
                    if (!int.TryParse(servisIdStr, out var servisId) || ogrenciServisId != servisId)
                        return Forbid();
                    break;

                // Öğretmen: tüm öğrencilere erişebilir
            }

            return null; // Erişim onaylandı
        }

        #endregion
    }
}
