using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Infrastructure
{
    public class MvcTenantCozumleyiciMiddleware
    {
        private readonly RequestDelegate _next;

        public MvcTenantCozumleyiciMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            TenantBaglami tenantBaglami,
            OkulYapilandirmaServisi okulServisi)
        {
            if (!(context.User.Identity?.IsAuthenticated ?? false))
            {
                await _next(context);
                return;
            }

            string? okulKodu = null;

            // Genel Admin: session'daki seçili okul
            if (context.User.IsInRole("GenelAdmin"))
            {
                okulKodu = context.Session.GetString("SeciliOkulKodu");
            }
            else
            {
                // Normal kullanıcı: cookie claim'deki okul kodu
                okulKodu = context.User.FindFirst("okulKodu")?.Value;
            }

            if (!string.IsNullOrEmpty(okulKodu))
            {
                var okul = okulServisi.OkulGetir(okulKodu);
                if (okul is not null)
                {
                    tenantBaglami.OkulKodu = okul.OkulKodu;
                    tenantBaglami.ConnectionString = okul.ConnectionString;
                    tenantBaglami.OkulAdi = okul.OkulAdi;
                }
            }

            await _next(context);
        }
    }
}
