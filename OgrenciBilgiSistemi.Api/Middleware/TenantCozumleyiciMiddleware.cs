using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Api.Middleware
{
    public class TenantCozumleyiciMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantCozumleyiciMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            TenantBaglami tenantBaglami,
            OkulYapilandirmaServisi okulServisi)
        {
            // Anonymous endpointler (login, okul listesi) tenant çözümlemesi gerektirmez
            if (!(context.User.Identity?.IsAuthenticated ?? false))
            {
                await _next(context);
                return;
            }

            var okulKodu = context.User.FindFirst("okulKodu")?.Value;
            if (string.IsNullOrEmpty(okulKodu))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("JWT token'da okulKodu claim'i bulunamadı.");
                return;
            }

            var okul = okulServisi.OkulGetir(okulKodu);
            if (okul is null)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Geçersiz okul kodu.");
                return;
            }

            tenantBaglami.OkulKodu = okul.OkulKodu;
            tenantBaglami.ConnectionString = okul.ConnectionString;
            tenantBaglami.OkulAdi = okul.OkulAdi;

            await _next(context);
        }
    }
}
