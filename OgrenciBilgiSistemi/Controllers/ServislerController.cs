using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ServislerController : Controller
    {
        private readonly IServisService _servisService;
        private readonly ILogger<ServislerController> _logger;

        public ServislerController(IServisService servisService, ILogger<ServislerController> logger)
        {
            _servisService = servisService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int page = 1, CancellationToken ct = default)
        {
            ViewData["CurrentFilter"] = searchString;
            var paged = await _servisService.SearchPagedAsync(searchString, page, 20, ct);
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            ViewBag.Kullanicilar = await _servisService.GetSoforSelectListAsync();
            return View(new ServisModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(ServisModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kullanicilar = await _servisService.GetSoforSelectListAsync();
                return View(model);
            }

            try
            {
                await _servisService.EkleAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis eklenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Kayıt sırasında bir hata oluştu.");
                ViewBag.Kullanicilar = await _servisService.GetSoforSelectListAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(int? id)
        {
            if (id == null) return NotFound();

            var servis = await _servisService.GetByIdAsync(id.Value);
            if (servis == null) return NotFound();

            ViewBag.Kullanicilar = await _servisService.GetSoforSelectListAsync(servis.KullaniciId);
            return View(servis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(ServisModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kullanicilar = await _servisService.GetSoforSelectListAsync(model.KullaniciId);
                return View(model);
            }

            try
            {
                await _servisService.GuncelleAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis güncellenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
                ViewBag.Kullanicilar = await _servisService.GetSoforSelectListAsync(model.KullaniciId);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            try
            {
                await _servisService.SilAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis silinirken hata oluştu.");
                TempData["ErrMessage"] = "Servis silinirken bir hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
