using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Dtos;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Shared.Services;
using System.Security.Claims;

namespace OgrenciBilgiSistemi.Controllers
{
    public class HesaplarController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OkulYapilandirmaServisi _okulServisi;
        private readonly IConfiguration _configuration;

        public HesaplarController(
            AppDbContext context,
            OkulYapilandirmaServisi okulServisi,
            IConfiguration configuration)
        {
            _context = context;
            _okulServisi = okulServisi;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Giris()
        {
            ViewBag.Okullar = _okulServisi.TumOkullariGetir();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(GirisIstegiDto model)
        {
            // --- Genel Admin kontrolü (ModelState'den önce, okul seçimi gerekmez) ---
            var genelAdminKullaniciAdi = _configuration["GenelAdmin:KullaniciAdi"];
            var genelAdminSifreHash = _configuration["GenelAdmin:SifreHash"];

            if (!string.IsNullOrEmpty(genelAdminKullaniciAdi) &&
                !string.IsNullOrWhiteSpace(model.KullaniciAdi) &&
                !string.IsNullOrWhiteSpace(model.Sifre) &&
                model.KullaniciAdi == genelAdminKullaniciAdi)
            {
                if (string.IsNullOrEmpty(genelAdminSifreHash))
                {
                    ModelState.AddModelError(string.Empty, "Genel admin yapılandırması eksik.");
                    ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                    return View(model);
                }

                var genelHasher = new PasswordHasher<object>();
                var genelResult = genelHasher.VerifyHashedPassword(null!, genelAdminSifreHash, model.Sifre);
                if (genelResult == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                    ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                    return View(model);
                }

                var genelClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, genelAdminKullaniciAdi),
                    new Claim(ClaimTypes.NameIdentifier, "0"),
                    new Claim(ClaimTypes.Role, "GenelAdmin")
                };

                var genelIdentity = new ClaimsIdentity(genelClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(genelIdentity),
                    new AuthenticationProperties { IsPersistent = model.BeniHatirla });

                return RedirectToAction("Index", "OkulSecim");
            }

            // --- Normal kullanıcı (okul bazlı) login ---
            if (!ModelState.IsValid)
            {
                ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.OkulKodu))
            {
                ModelState.AddModelError(string.Empty, "Okul seçimi gereklidir.");
                ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                return View(model);
            }

            var okul = _okulServisi.OkulGetir(model.OkulKodu);
            if (okul is null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz okul kodu.");
                ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                return View(model);
            }

            // Seçilen okulun DB'sine bağlanarak kullanıcı doğrulama
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(okul.ConnectionString);
            await using var tempContext = new AppDbContext(optionsBuilder.Options);

            var user = await tempContext.Kullanicilar
                .Where(k => k.KullaniciDurum)
                .SingleOrDefaultAsync(u => u.KullaniciAdi == model.KullaniciAdi);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                return View(model);
            }

            var passwordHasher = new PasswordHasher<KullaniciModel>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Sifre, model.Sifre);
            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                ViewBag.Okullar = _okulServisi.TumOkullariGetir();
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.KullaniciAdi),
                new Claim(ClaimTypes.NameIdentifier, user.KullaniciId.ToString()),
                new Claim("userid", user.KullaniciId.ToString()),
                new Claim("KullaniciId", user.KullaniciId.ToString()),
                new Claim("sub", user.KullaniciId.ToString()),
                new Claim(ClaimTypes.Role, user.Rol.ToString()),
                new Claim("okulKodu", okul.OkulKodu),
                new Claim("okulAdi", okul.OkulAdi)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.BeniHatirla
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Cikis()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Giris", "Hesaplar");
        }

        [AllowAnonymous]
        public IActionResult YetkisizGiris() => View();
    }
}
