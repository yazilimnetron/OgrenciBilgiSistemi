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

        [HttpGet("{id}")]
        public async Task<IActionResult> VeliDetay(int id)
        {
            var detay = await _veliListeService.VeliDetayGetirAsync(id);
            if (detay is null)
                return NotFound(new { message = $"{id} numaralı veli bulunamadı." });
            return Ok(detay);
        }
    }
}
