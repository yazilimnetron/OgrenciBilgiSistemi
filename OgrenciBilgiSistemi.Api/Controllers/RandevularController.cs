using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Dtos;
using OgrenciBilgiSistemi.Api.Services;
using OgrenciBilgiSistemi.Shared.Enums;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [Route("api/randevular")]
    [ApiController]
    [Authorize]
    public class RandevularController : ControllerBase
    {
        private readonly RandevuService _randevuService;
        private readonly BildirimService _bildirimService;

        public RandevularController(RandevuService randevuService, BildirimService bildirimService)
        {
            _randevuService = randevuService;
            _bildirimService = bildirimService;
        }

        private int KullaniciId => int.Parse(User.FindFirst("kullaniciId")!.Value);
        private string Rol => User.FindFirst("rol")!.Value;

        [HttpGet("benim")]
        public async Task<IActionResult> Benim()
        {
            var rol = Rol;
            if (rol != "Ogretmen" && rol != "Veli")
                return Forbid();

            var liste = await _randevuService.KullanicininRandevulariniGetir(KullaniciId, rol);
            return Ok(liste);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Getir(int id)
        {
            var randevu = await _randevuService.RandevuGetir(id);
            if (randevu is null) return NotFound();

            var kullaniciId = KullaniciId;
            if (randevu.OgretmenKullaniciId != kullaniciId && randevu.VeliKullaniciId != kullaniciId)
                return Forbid();

            return Ok(randevu);
        }

        [HttpPost]
        public async Task<IActionResult> Olustur([FromBody] RandevuOlusturDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var rol = Rol;
            var kullaniciId = KullaniciId;
            int randevuId;
            int bildirimAliciId;

            if (rol == "Ogretmen")
            {
                randevuId = await _randevuService.OgretmenRandevuOlustur(
                    kullaniciId, dto.KarsiTarafKullaniciId, dto.OgrenciId,
                    dto.RandevuTarihi, dto.SureDakika, dto.Not);
                bildirimAliciId = dto.KarsiTarafKullaniciId;
            }
            else if (rol == "Veli")
            {
                randevuId = await _randevuService.VeliRandevuOlustur(
                    kullaniciId, dto.KarsiTarafKullaniciId, dto.OgrenciId,
                    dto.RandevuTarihi, dto.SureDakika, dto.Not);
                bildirimAliciId = dto.KarsiTarafKullaniciId;
            }
            else
            {
                return Forbid();
            }

            var tarihStr = dto.RandevuTarihi.ToString("dd.MM.yyyy HH:mm");
            await _bildirimService.Olustur(bildirimAliciId,
                (int)BildirimTuru.RandevuOlusturuldu,
                $"Sizinle {tarihStr} tarihinde bir randevu oluşturuldu.",
                randevuId);

            return CreatedAtAction(nameof(Getir), new { id = randevuId }, new { randevuId });
        }

        [HttpPut("{id}/onayla")]
        public async Task<IActionResult> Onayla(int id)
        {
            if (Rol != "Ogretmen") return Forbid();

            var basarili = await _randevuService.DurumGuncelle(id, KullaniciId, RandevuDurumu.Onaylandi);
            if (!basarili) return NotFound();

            var randevu = await _randevuService.RandevuGetir(id);
            if (randevu is not null)
            {
                var tarihStr = randevu.RandevuTarihi.ToString("dd.MM.yyyy HH:mm");
                await _bildirimService.Olustur(randevu.VeliKullaniciId,
                    (int)BildirimTuru.RandevuOnaylandi,
                    $"{tarihStr} tarihli randevunuz onaylandı.",
                    id);
            }

            return Ok(new { mesaj = "Randevu onaylandı." });
        }

        [HttpPut("{id}/reddet")]
        public async Task<IActionResult> Reddet(int id)
        {
            if (Rol != "Ogretmen") return Forbid();

            var basarili = await _randevuService.DurumGuncelle(id, KullaniciId, RandevuDurumu.Reddedildi);
            if (!basarili) return NotFound();

            var randevu = await _randevuService.RandevuGetir(id);
            if (randevu is not null)
            {
                var tarihStr = randevu.RandevuTarihi.ToString("dd.MM.yyyy HH:mm");
                await _bildirimService.Olustur(randevu.VeliKullaniciId,
                    (int)BildirimTuru.RandevuReddedildi,
                    $"{tarihStr} tarihli randevunuz reddedildi.",
                    id);
            }

            return Ok(new { mesaj = "Randevu reddedildi." });
        }

        [HttpPut("{id}/iptal")]
        public async Task<IActionResult> IptalEt(int id)
        {
            var kullaniciId = KullaniciId;
            var basarili = await _randevuService.IptalEt(id, kullaniciId);
            if (!basarili) return NotFound();

            var randevu = await _randevuService.RandevuGetir(id);
            if (randevu is not null)
            {
                var tarihStr = randevu.RandevuTarihi.ToString("dd.MM.yyyy HH:mm");
                var aliciId = randevu.OgretmenKullaniciId == kullaniciId
                    ? randevu.VeliKullaniciId
                    : randevu.OgretmenKullaniciId;
                await _bildirimService.Olustur(aliciId,
                    (int)BildirimTuru.RandevuIptalEdildi,
                    $"{tarihStr} tarihli randevu iptal edildi.",
                    id);
            }

            return Ok(new { mesaj = "Randevu iptal edildi." });
        }
    }
}
