using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Services;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [Route("api/ogretmenler")]
    [ApiController]
    [Authorize]
    public class OgretmenlerController : ControllerBase
    {
        private readonly OgretmenListeService _ogretmenListeService;

        public OgretmenlerController(OgretmenListeService ogretmenListeService)
        {
            _ogretmenListeService = ogretmenListeService;
        }

        [HttpGet("aktif")]
        public async Task<IActionResult> AktifOgretmenler()
        {
            var liste = await _ogretmenListeService.AktifOgretmenleriGetir();
            return Ok(liste);
        }
    }
}
