using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Dtos;
using OgrenciBilgiSistemi.Api.Services;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [Route("api/ogretmen-randevu")]
    [ApiController]
    [Authorize]
    public class OgretmenRandevuController : ControllerBase
    {
        private readonly OgretmenRandevuService _ogretmenRandevuService;

        public OgretmenRandevuController(OgretmenRandevuService ogretmenRandevuService)
        {
            _ogretmenRandevuService = ogretmenRandevuService;
        }

        private int KullaniciId => int.Parse(User.FindFirst("kullaniciId")!.Value);
        private string Rol => User.FindFirst("rol")!.Value;

        [HttpGet("benim")]
        public async Task<IActionResult> Benim()
        {
            if (Rol != "Ogretmen") return Forbid();
            var liste = await _ogretmenRandevuService.OgretmeninRandevuTakviminiGetir(KullaniciId);
            return Ok(liste);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] OgretmenRandevuEkleDto dto)
        {
            if (Rol != "Ogretmen") return Forbid();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!TimeSpan.TryParse(dto.BaslangicSaati, out var baslangic) ||
                !TimeSpan.TryParse(dto.BitisSaati, out var bitis))
                return BadRequest("Geçersiz saat formatı.");

            if (bitis <= baslangic)
                return BadRequest("Bitiş saati başlangıçtan büyük olmalıdır.");

            if (dto.Tarih.Date < DateTime.Today)
                return BadRequest("Geçmiş tarih seçilemez.");

            var id = await _ogretmenRandevuService.Ekle(KullaniciId, dto.Tarih, baslangic, bitis);
            return Ok(new { ogretmenRandevuId = id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Sil(int id)
        {
            if (Rol != "Ogretmen") return Forbid();
            var basarili = await _ogretmenRandevuService.Sil(id, KullaniciId);
            if (!basarili) return NotFound();
            return Ok(new { mesaj = "Randevu takvimi silindi." });
        }

        [HttpGet("ogretmen/{ogretmenId}/slotlar")]
        public async Task<IActionResult> RandevuSlotlar(int ogretmenId, [FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis)
        {
            var baslangicTarih = baslangic ?? DateTime.Today;
            var bitisTarih = bitis ?? DateTime.Today.AddDays(14);

            var slotlar = await _ogretmenRandevuService.RandevuSlotlariGetir(ogretmenId, baslangicTarih, bitisTarih);
            return Ok(slotlar);
        }
    }
}
