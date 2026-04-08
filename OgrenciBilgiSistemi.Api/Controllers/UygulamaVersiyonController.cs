using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Models;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [Route("api/uygulama-versiyon")]
    [ApiController]
    [AllowAnonymous]
    public class UygulamaVersiyonController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UygulamaVersiyonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Getir()
        {
            var bilgi = _configuration.GetSection("UygulamaVersiyon").Get<UygulamaVersiyonBilgi>()
                        ?? new UygulamaVersiyonBilgi();
            return Ok(bilgi);
        }
    }
}
