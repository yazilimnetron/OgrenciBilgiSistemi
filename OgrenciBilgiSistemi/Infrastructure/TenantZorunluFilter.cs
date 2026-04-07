using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OgrenciBilgiSistemi.Shared.Services;

namespace OgrenciBilgiSistemi.Infrastructure
{
    public class TenantZorunluFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                await next();
                return;
            }

            // Bu controller'lar tenant gerektirmez
            var controller = context.RouteData.Values["controller"]?.ToString();
            if (controller is "Hesaplar" or "OkulSecim")
            {
                await next();
                return;
            }

            // Genel Admin okul seçmemişse → okul seçim sayfasına yönlendir
            if (user.IsInRole("GenelAdmin"))
            {
                var seciliOkul = context.HttpContext.Session.GetString("SeciliOkulKodu");
                if (string.IsNullOrEmpty(seciliOkul))
                {
                    context.Result = new RedirectToActionResult("Index", "OkulSecim", null);
                    return;
                }
            }

            // Tenant bağlamı boşsa login'e yönlendir
            var tenantBaglami = context.HttpContext.RequestServices.GetRequiredService<TenantBaglami>();
            if (string.IsNullOrEmpty(tenantBaglami.ConnectionString))
            {
                context.Result = new RedirectToActionResult("Giris", "Hesaplar", null);
                return;
            }

            await next();
        }
    }
}
