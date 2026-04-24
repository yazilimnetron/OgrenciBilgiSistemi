using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Dtos;
using OgrenciBilgiSistemi.Api.Services;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [Route("api/musaitlik")]
    [ApiController]
    [Authorize]
    public class MusaitlikController : ControllerBase
    {
        private readonly MusaitlikService _musaitlikService;

        public MusaitlikController(MusaitlikService musaitlikService)
        {
            _musaitlikService = musaitlikService;
        }

        private int KullaniciId => int.Parse(User.FindFirst("kullaniciId")!.Value);
        private string Rol => User.FindFirst("rol")!.Value;

        [HttpGet("benim")]
        public async Task<IActionResult> Benim()
        {
            if (Rol != "Ogretmen") return Forbid();
            var liste = await _musaitlikService.OgretmeninMusaitlikleriniGetir(KullaniciId);
            return Ok(liste);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] MusaitlikEkleDto dto)
        {
            if (Rol != "Ogretmen") return Forbid();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!TimeSpan.TryParse(dto.BaslangicSaati, out var baslangic) ||
                !TimeSpan.TryParse(dto.BitisSaati, out var bitis))
                return BadRequest("Geçersiz saat formatı.");

            if (bitis <= baslangic)
                return BadRequest("Bitiş saati başlangıçtan büyük olmalıdır.");

            var id = await _musaitlikService.Ekle(KullaniciId, dto.Gun, baslangic, bitis);
            return Ok(new { musaitlikId = id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Sil(int id)
        {
            if (Rol != "Ogretmen") return Forbid();
            var basarili = await _musaitlikService.Sil(id, KullaniciId);
            if (!basarili) return NotFound();
            return Ok(new { mesaj = "Müsaitlik silindi." });
        }

        [HttpGet("ogretmen/{ogretmenId}/slotlar")]
        public async Task<IActionResult> MusaitSlotlar(int ogretmenId, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis)
        {
            var baslangicTarih = baslangic ?? DateTime.Today;
            var bitisTarih = bitis ?? DateTime.Today.AddDays(14);

            var slotlar = await _musaitlikService.MusaitSlotlariGetir(ogretmenId, baslangicTarih, bitisTarih);
            return Ok(slotlar);
        }
    }
}
