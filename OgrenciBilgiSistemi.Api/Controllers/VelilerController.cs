using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgrenciBilgiSistemi.Api.Services;

namespace OgrenciBilgiSistemi.Api.Controllers
{
    [ApiController]
    [Route("api/veliler")]
    [Authorize(Policy = "AdminOnly")]
    public class VelilerController : ControllerBase
    {
        private readonly VeliListeService _veliListeService;

        public VelilerController(VeliListeService veliListeService)
        {
            _veliListeService = veliListeService;
        }

        [HttpGet("aktif")]
        public async Task<IActionResult> AktifVeliler()
        {
            var liste = await _veliListeService.AktifVelileriGetirAsync();
            return Ok(liste);
        }
    }
}
