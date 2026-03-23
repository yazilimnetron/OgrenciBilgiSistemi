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
        /// Belirtilen şoföre (KullaniciId) atanmış öğrencileri getirir.
        /// </summary>
        [HttpGet("{servisId}/ogrenciler")]
        public async Task<IActionResult> ServisOgrencileriGetir(int servisId)
        {
            var ogrenciler = await _servisService.ServisOgrencileriGetir(servisId);
            return Ok(ogrenciler);
        }

        /// <summary>
        /// Belirtilen şoförün servis profil bilgilerini getirir.
        /// </summary>
        [HttpGet("{servisId}")]
        public async Task<IActionResult> ServisProfilGetir(int servisId)
        {
            var profil = await _servisService.ServisProfilGetir(servisId);
            if (profil == null)
                return NotFound("Servis profili bulunamadı.");

            return Ok(profil);
        }

        /// <summary>
        /// Belirtilen şoförün bugünkü yoklamasını periyoda göre getirir.
        /// </summary>
        [HttpGet("{servisId}/yoklama/{periyot}")]
        public async Task<IActionResult> ServisYoklamaGetir(int servisId, int periyot)
        {
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
        /// </summary>
        [HttpPost("yoklama-kaydet")]
        public async Task<IActionResult> ServisYoklamaKaydet([FromBody] ServisYoklamaKaydetDto model)
        {
            if (model.Kayitlar == null || model.Kayitlar.Count == 0)
                return BadRequest(new { error = "Yoklama kaydı listesi boş olamaz." });

            try
            {
                var formatliVeri = model.Kayitlar.Select(k => (k.OgrenciId, k.DurumId));

                await _servisService.ServisYoklamaKaydet(
                    formatliVeri,
                    model.KullaniciId,
                    model.Periyot
                );

                return Ok(new { message = "Servis yoklaması başarıyla kaydedildi." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Servis yoklaması kaydedilirken bir hata oluştu." });
            }
        }
    }
}
