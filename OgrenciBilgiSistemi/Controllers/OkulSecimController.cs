using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Roles = "GenelAdmin")]
    public class OkulSecimController : Controller
    {
        private readonly OkulYapilandirmaServisi _okulServisi;

        public OkulSecimController(OkulYapilandirmaServisi okulServisi)
        {
            _okulServisi = okulServisi;
        }

        public IActionResult Index()
        {
            var okullar = _okulServisi.TumOkullariGetir();
            ViewBag.SeciliOkulKodu = HttpContext.Session.GetString("SeciliOkulKodu");
            return View(okullar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OkulSec(string okulKodu)
        {
            if (!_okulServisi.OkulVarMi(okulKodu))
                return BadRequest("Geçersiz okul kodu.");

            HttpContext.Session.SetString("SeciliOkulKodu", okulKodu);

            // GenelAdmin kaydı yoksa otomatik oluştur + tüm menüleri ata
            var okul = _okulServisi.OkulGetir(okulKodu);
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(okul!.ConnectionString);

            using var db = new AppDbContext(optionsBuilder.Options);

            var mevcutKayit = await db.Kullanicilar
                .FirstOrDefaultAsync(k => k.Rol == KullaniciRolu.GenelAdmin);

            if (mevcutKayit == null)
            {
                var genelAdmin = new KullaniciModel
                {
                    KullaniciAdi = "GenelAdmin",
                    Sifre = "-",
                    Rol = KullaniciRolu.GenelAdmin,
                    KullaniciDurum = true
                };
                db.Kullanicilar.Add(genelAdmin);
                await db.SaveChangesAsync();

                var tumMenuler = await db.MenuOgeler
                    .Select(m => m.Id)
                    .ToListAsync();

                foreach (var menuId in tumMenuler)
                {
                    db.KullaniciMenuOgeler.Add(new KullaniciMenuModel
                    {
                        KullaniciId = genelAdmin.KullaniciId,
                        MenuOgeId = menuId
                    });
                }
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
