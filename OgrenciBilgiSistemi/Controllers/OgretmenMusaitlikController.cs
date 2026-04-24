using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;
using OgrenciBilgiSistemi.Shared.Enums;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class OgretmenMusaitlikController : Controller
    {
        private readonly IOgretmenMusaitlikService _musaitlikService;
        private readonly AppDbContext _db;
        private readonly ILogger<OgretmenMusaitlikController> _logger;

        public OgretmenMusaitlikController(
            IOgretmenMusaitlikService musaitlikService,
            AppDbContext db,
            ILogger<OgretmenMusaitlikController> logger)
        {
            _musaitlikService = musaitlikService;
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? ogretmenId, CancellationToken ct = default)
        {
            ViewData["OgretmenId"] = ogretmenId;
            ViewData["Ogretmenler"] = await OgretmenListesi(ct);

            if (ogretmenId.HasValue)
            {
                var liste = await _musaitlikService.OgretmeneGoreListele(ogretmenId.Value, ct);
                return View(liste);
            }

            return View(new List<OgretmenMusaitlikModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Ekle(int? ogretmenId, CancellationToken ct = default)
        {
            ViewData["Ogretmenler"] = await OgretmenListesi(ct);
            var model = new OgretmenMusaitlikModel();
            if (ogretmenId.HasValue)
                model.OgretmenKullaniciId = ogretmenId.Value;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(OgretmenMusaitlikModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Ogretmenler"] = await OgretmenListesi(ct);
                return View(model);
            }

            try
            {
                await _musaitlikService.Ekle(model, ct);
                return RedirectToAction(nameof(Index), new { ogretmenId = model.OgretmenKullaniciId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müsaitlik eklenemedi.");
                ModelState.AddModelError("", "Müsaitlik eklenirken bir hata oluştu.");
                ViewData["Ogretmenler"] = await OgretmenListesi(ct);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id, int ogretmenId, CancellationToken ct = default)
        {
            try
            {
                await _musaitlikService.Sil(id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müsaitlik silinemedi. Id={Id}", id);
                TempData["Hata"] = "Müsaitlik silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index), new { ogretmenId });
        }

        private async Task<List<SelectListItem>> OgretmenListesi(CancellationToken ct)
        {
            return await _db.Kullanicilar
                .AsNoTracking()
                .Where(k => k.Rol == KullaniciRolu.Ogretmen && k.KullaniciDurum)
                .OrderBy(k => k.KullaniciAdi)
                .Select(k => new SelectListItem(k.KullaniciAdi, k.KullaniciId.ToString()))
                .ToListAsync(ct);
        }
    }
}
