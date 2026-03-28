using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class OgretmenlerController : Controller
    {
        private readonly IOgretmenProfilService _ogretmenProfilService;
        private readonly IKullaniciService _kullaniciService;
        private readonly IBirimService _birimService;
        private readonly ILogger<OgretmenlerController> _logger;

        public OgretmenlerController(
            IOgretmenProfilService ogretmenProfilService,
            IKullaniciService kullaniciService,
            IBirimService birimService,
            ILogger<OgretmenlerController> logger)
        {
            _ogretmenProfilService = ogretmenProfilService;
            _kullaniciService = kullaniciService;
            _birimService = birimService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int page = 1, CancellationToken ct = default)
        {
            ViewData["CurrentFilter"] = searchString;
            var paged = await _ogretmenProfilService.SearchPagedAsync(searchString, page, 50, ct);
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            await DropdownDoldur();
            return View(new OgretmenProfilModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(OgretmenProfilModel model)
        {
            if (!ModelState.IsValid)
            {
                await DropdownDoldur();
                return View(model);
            }

            try
            {
                await _ogretmenProfilService.EkleAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Öğretmen profili eklenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Kayıt sırasında bir hata oluştu.");
                await DropdownDoldur();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(int? id)
        {
            if (id == null) return NotFound();

            var profil = await _ogretmenProfilService.GetByIdAsync(id.Value);
            if (profil == null) return NotFound();

            await DropdownDoldur();
            return View(profil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(OgretmenProfilModel model)
        {
            if (!ModelState.IsValid)
            {
                await DropdownDoldur();
                return View(model);
            }

            try
            {
                await _ogretmenProfilService.GuncelleAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Öğretmen profili güncellenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
                await DropdownDoldur();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            try
            {
                await _ogretmenProfilService.SilAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Öğretmen profili silinirken hata oluştu.");
                TempData["ErrMessage"] = "Öğretmen profili silinirken bir hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task DropdownDoldur()
        {
            ViewBag.Kullanicilar = await _kullaniciService.GetPersonellerSelectListAsync();
            ViewBag.Birimler = await _kullaniciService.GetBirimlerSelectListAsync();
        }
    }
}
