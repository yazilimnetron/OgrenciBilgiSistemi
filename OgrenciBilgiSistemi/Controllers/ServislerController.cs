using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;
using OgrenciBilgiSistemi.ViewModels;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ServislerController : Controller
    {
        private readonly IServisProfilService _servisProfilService;
        private readonly IKullaniciService _kullaniciService;
        private readonly ILogger<ServislerController> _logger;

        public ServislerController(
            IServisProfilService servisProfilService,
            IKullaniciService kullaniciService,
            ILogger<ServislerController> logger)
        {
            _servisProfilService = servisProfilService;
            _kullaniciService = kullaniciService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int page = 1, CancellationToken ct = default)
        {
            ViewData["CurrentFilter"] = searchString;
            var paged = await _servisProfilService.SearchPagedAsync(searchString, page, 50, ct);
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            ViewBag.Kullanicilar = await _kullaniciService.GetServislerByIdSelectListAsync();
            return View(new ServisProfilModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(ServisProfilModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kullanicilar = await _kullaniciService.GetServislerByIdSelectListAsync();
                return View(model);
            }

            try
            {
                await _servisProfilService.EkleAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis profili eklenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Kayıt sırasında bir hata oluştu.");
                ViewBag.Kullanicilar = await _kullaniciService.GetServislerByIdSelectListAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(int? id)
        {
            if (id == null) return NotFound();

            var profil = await _servisProfilService.GetByIdAsync(id.Value);
            if (profil == null) return NotFound();

            ViewBag.Kullanicilar = await _kullaniciService.GetServislerByIdSelectListAsync(profil.KullaniciId);
            return View(profil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(ServisProfilModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kullanicilar = await _kullaniciService.GetServislerByIdSelectListAsync(model.KullaniciId);
                return View(model);
            }

            try
            {
                await _servisProfilService.GuncelleAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis profili güncellenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
                ViewBag.Kullanicilar = await _kullaniciService.GetServislerByIdSelectListAsync(model.KullaniciId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detay(int id, CancellationToken ct = default)
        {
            var servis = await _servisProfilService.GetByIdAsync(id, ct);
            if (servis == null) return NotFound();

            var ogrenciler = await _servisProfilService.GetOgrencilerAsync(id, ct);

            var vm = new ServisDetayVm
            {
                Servis = servis,
                Ogrenciler = ogrenciler
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            try
            {
                await _servisProfilService.SilAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis profili silinirken hata oluştu.");
                TempData["ErrMessage"] = "Servis profili silinirken bir hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
