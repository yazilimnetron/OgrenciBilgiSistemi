using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Dtos;
using OgrenciBilgiSistemi.Api.Services;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [ApiController]
    [Route("api/servisler")]
    [Authorize]
    public class ServisController : ControllerBase
    {
        private readonly ServisService _servisService;

        public ServisController(ServisService servisService)
        {
            _servisService = servisService;
        }

        /// <summary>
        /// Belirtilen servise (KullaniciId) atanmış öğrencileri getirir.
        /// Yalnızca servis kendi servis ID'si ile erişebilir.
        /// </summary>
        [HttpGet("{servisId}/ogrenciler")]
        public async Task<IActionResult> ServisOgrencileriGetir(int servisId)
        {
            var yetkiSonucu = ServisErisimKontrol(servisId);
            if (yetkiSonucu != null) return yetkiSonucu;

            var ogrenciler = await _servisService.ServisOgrencileriGetir(servisId);
            return Ok(ogrenciler);
        }

        /// <summary>
        /// Belirtilen servisün servis profil bilgilerini getirir.
        /// Yalnızca servis kendi profilini görüntüleyebilir.
        /// </summary>
        [HttpGet("{servisId}")]
        public async Task<IActionResult> ServisProfilGetir(int servisId)
        {
            var yetkiSonucu = ServisErisimKontrol(servisId);
            if (yetkiSonucu != null) return yetkiSonucu;

            var profil = await _servisService.ServisProfilGetir(servisId);
            if (profil == null)
                return NotFound("Servis profili bulunamadı.");

            return Ok(profil);
        }

        /// <summary>
        /// Belirtilen servisün bugünkü yoklamasını periyoda göre getirir.
        /// Yalnızca servis kendi yoklamasını görüntüleyebilir.
        /// </summary>
        [HttpGet("{servisId}/yoklama/{periyot}")]
        public async Task<IActionResult> ServisYoklamaGetir(int servisId, int periyot)
        {
            var yetkiSonucu = ServisErisimKontrol(servisId);
            if (yetkiSonucu != null) return yetkiSonucu;

            try
            {
                var yoklama = await _servisService.MevcutServisYoklamaGetir(servisId, periyot);
                return Ok(yoklama);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Servis yoklama bilgisi alınırken bir hata oluştu." });
            }
        }

        /// <summary>
        /// Servis yoklamasını toplu olarak kaydeder.
        /// KullaniciId body'den değil, JWT token'dan alınır.
        /// </summary>
        [HttpPost("yoklama-kaydet")]
        public async Task<IActionResult> ServisYoklamaKaydet([FromBody] ServisYoklamaKaydetDto model)
        {
            var rol = User.FindFirst("rol")?.Value;
            if (rol != "Servis")
                return Forbid();

            var servisIdStr = User.FindFirst("servisId")?.Value;
            if (!int.TryParse(servisIdStr, out var tokenServisId))
                return Unauthorized("Oturum bilgileri eksik.");

            if (model.Kayitlar == null || model.Kayitlar.Count == 0)
                return BadRequest(new { error = "Yoklama kaydı listesi boş olamaz." });

            try
            {
                var formatliVeri = model.Kayitlar.Select(k => (k.OgrenciId, k.DurumId));

                await _servisService.ServisYoklamaKaydet(
                    formatliVeri,
                    tokenServisId,
                    model.Periyot
                );

                return Ok(new { message = "Servis yoklaması başarıyla kaydedildi." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Servis yoklaması kaydedilirken bir hata oluştu." });
            }
        }

        /// <summary>
        /// Servis rolü ve servisId eşleşmesini kontrol eder.
        /// </summary>
        private IActionResult? ServisErisimKontrol(int servisId)
        {
            var rol = User.FindFirst("rol")?.Value;
            if (rol != "Servis")
                return Forbid();

            var servisIdStr = User.FindFirst("servisId")?.Value;
            if (!int.TryParse(servisIdStr, out var tokenServisId) || tokenServisId != servisId)
                return Forbid();

            return null;
        }
    }
}
