using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Controllers
{
    public class PersonellerController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<PersonellerController> _logger;
        private readonly IPersonelService _personelService;
        private readonly IBirimService _birimService;
        private readonly IKullaniciService _kullaniciService;


        public PersonellerController(
            AppDbContext db,
            ILogger<PersonellerController> logger,
            IPersonelService personelService,
            IBirimService birimService,
            IKullaniciService kullaniciService)
        {
            _db = db;
            _logger = logger;
            _personelService = personelService;
            _birimService = birimService;
            _kullaniciService = kullaniciService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchString,
            int pageNumber = 1,
            PersonelFiltre durum = PersonelFiltre.Aktif,
            CancellationToken ct = default)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["Durum"] = durum;

            var model = await _personelService.SearchPagedAsync(
                searchString: searchString,
                page: pageNumber,
                pageSize: 50,
                filtre: durum,
                ct: ct);

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Ekle(CancellationToken ct = default)
        {
            var model = new PersonelModel();
            await PersonelDropdownDoldur(model, ct);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(PersonelModel model, IFormFile? PersonelGorselFile, CancellationToken ct = default)
        {
            // Kullanıcı seçildiyse adını PersonelAdSoyad'a yaz (validation öncesi)
            if (model.KullaniciId.HasValue)
            {
                var kullanici = await _db.Kullanicilar.FindAsync([model.KullaniciId.Value], ct);
                if (kullanici != null)
                {
                    model.PersonelAdSoyad = kullanici.KullaniciAdi;
                    ModelState.Remove(nameof(model.PersonelAdSoyad));
                }
            }

            if (!ModelState.IsValid)
            {
                await PersonelDropdownDoldur(model, ct);
                return View(model);
            }

            try
            {
                var personelId = await _personelService.AddAsync(model, PersonelGorselFile, ct);

                // Kullanıcı-Personel bağlantısını kur
                if (model.KullaniciId.HasValue)
                {
                    var kullanici = await _db.Kullanicilar.FindAsync([model.KullaniciId.Value], ct);
                    if (kullanici != null)
                    {
                        kullanici.PersonelId = personelId;
                        await _db.SaveChangesAsync(ct);
                    }
                }

                TempData["Mesaj"] = "Personel eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel eklenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, ex.Message);
                await PersonelDropdownDoldur(model, ct);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(int id, CancellationToken ct = default)
        {
            var p = await _db.Personeller.FindAsync(new object?[] { id }, ct);
            if (p is null) return NotFound();

            // Mevcut kullanıcı bağlantısını bul
            var bagli = await _db.Kullanicilar
                .FirstOrDefaultAsync(k => k.PersonelId == id && k.KullaniciDurum, ct);
            p.KullaniciId = bagli?.KullaniciId;

            await PersonelDropdownDoldur(p, ct);
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(PersonelModel model, IFormFile? PersonelGorselFile, CancellationToken ct = default)
        {
            // Kullanıcı seçildiyse adını PersonelAdSoyad'a yaz (validation öncesi)
            if (model.KullaniciId.HasValue)
            {
                var kullanici = await _db.Kullanicilar.FindAsync([model.KullaniciId.Value], ct);
                if (kullanici != null)
                {
                    model.PersonelAdSoyad = kullanici.KullaniciAdi;
                    ModelState.Remove(nameof(model.PersonelAdSoyad));
                }
            }

            if (!ModelState.IsValid)
            {
                await PersonelDropdownDoldur(model, ct);
                return View(model);
            }

            try
            {
                await _personelService.UpdateAsync(model, PersonelGorselFile, ct);

                // Eski bağlantıyı temizle
                var eskiBagli = await _db.Kullanicilar
                    .FirstOrDefaultAsync(k => k.PersonelId == model.PersonelId && k.KullaniciDurum, ct);
                if (eskiBagli != null && eskiBagli.KullaniciId != model.KullaniciId)
                    eskiBagli.PersonelId = null;

                // Yeni bağlantıyı kur
                if (model.KullaniciId.HasValue)
                {
                    var yeniBagli = await _db.Kullanicilar.FindAsync([model.KullaniciId.Value], ct);
                    if (yeniBagli != null)
                        yeniBagli.PersonelId = model.PersonelId;
                }

                await _db.SaveChangesAsync(ct);

                TempData["Mesaj"] = "Personel güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel güncellenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, ex.Message);
                await PersonelDropdownDoldur(model, ct);
                return View(model);
            }
        }

        private async Task PersonelDropdownDoldur(PersonelModel model, CancellationToken ct)
        {
            ViewBag.Birimler = await GetBirimlerSelectListAsync(model.BirimId, includeAllOption: false, ct);
            model.Kullanicilar = await _kullaniciService.GetKullanicilarByRolSelectListAsync(
                KullaniciRolu.Ogretmen, ct);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id, CancellationToken ct = default)
        {
            try
            {
                await _personelService.DeleteAsync(id, ct);
                TempData["Mesaj"] = "Personel pasif hale getirildi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel silinirken hata oluştu.");
                TempData["Mesaj"] = "Silme sırasında bir hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TopluPersonelGonder(int cihazId, CancellationToken ct)
        {
            var ok = await _personelService.CihazaGonderAsync(cihazId, sadeceAktifler: true, ct);
            TempData["Mesaj"] = ok
                ? "Tüm (aktif) personeller başarıyla cihaza gönderildi."
                : "Bazı personeller cihaza eklenemedi. Lütfen logları kontrol edin.";
            return RedirectToAction("Index", "Cihazlar");
        }


        private async Task<List<SelectListItem>> GetBirimlerSelectListAsync(
            int? selectedId,
            bool includeAllOption,
            CancellationToken ct)
        {
            var list = await _birimService.GetSelectListAsync(
                selectedId: selectedId,
                sinifMi: null,
                filtre: BirimFiltre.Aktif,
                ct: ct);

            return list;
        }
    }
}