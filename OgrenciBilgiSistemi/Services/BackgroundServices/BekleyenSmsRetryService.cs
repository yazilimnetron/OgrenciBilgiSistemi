using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Shared.Enums;
using OgrenciBilgiSistemi.Sms;

namespace OgrenciBilgiSistemi.Services.BackgroundServices;

/// <summary>
/// Bugün gönderilemeyen Ana Kapı SMS'lerini periyodik olarak yeniden dener.
/// İnternet/SMS sağlayıcı geçici hatalarına karşı dayanıklılık sağlar.
/// (Yemekhane retry'i YemekhanePollingService içinde 1 dk'da bir yapılır.)
/// </summary>
public sealed class BekleyenSmsRetryService : BackgroundService
{
    private static readonly TimeSpan POLL_INTERVAL = TimeSpan.FromMinutes(2);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BekleyenSmsRetryService> _logger;

    public BekleyenSmsRetryService(IServiceScopeFactory scopeFactory, ILogger<BekleyenSmsRetryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bekleyen SMS retry servisi başlatıldı (Ana Kapı).");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AnaKapiBekleyenleriGonder(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana Kapı SMS retry turu sırasında hata.");
            }

            try { await Task.Delay(POLL_INTERVAL, stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task AnaKapiBekleyenleriGonder(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();
        var smsAyar = scope.ServiceProvider.GetRequiredService<IOptions<SmsAyarlari>>().Value;

        if (!smsAyar.Aktif) return;

        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);

        // Bugün, ana kapı, SMS gönderilmemiş kayıtlar
        var bekleyenler = await db.OgrenciDetaylar
            .Where(x => x.IstasyonTipi == IstasyonTipi.AnaKapi
                        && x.OgrenciSmsGonderildi != true
                        && ((x.OgrenciGTarih >= today && x.OgrenciGTarih < tomorrow)
                            || (x.OgrenciCTarih >= today && x.OgrenciCTarih < tomorrow)))
            .ToListAsync(ct);

        if (bekleyenler.Count == 0) return;

        var ogrIdler = bekleyenler.Select(x => x.OgrenciId).Distinct().ToList();
        var ogrenciBilgileri = await db.Ogrenciler.AsNoTracking()
            .Where(o => ogrIdler.Contains(o.OgrenciId) && o.VeliId != null)
            .Select(o => new
            {
                o.OgrenciId,
                o.OgrenciAdSoyad,
                VeliTelefon = o.Veli!.Telefon
            })
            .ToDictionaryAsync(o => o.OgrenciId, ct);

        foreach (var log in bekleyenler)
        {
            ct.ThrowIfCancellationRequested();

            if (!ogrenciBilgileri.TryGetValue(log.OgrenciId, out var bilgi)) continue;
            if (string.IsNullOrWhiteSpace(bilgi.VeliTelefon)) continue;

            var zaman = log.OgrenciGTarih ?? log.OgrenciCTarih ?? DateTime.Now;
            var gecisTipi = log.OgrenciGTarih.HasValue ? "Giriş" : "Çıkış";
            var mesaj = SmsMesajSablonlari.AnaKapiGecis(bilgi.OgrenciAdSoyad, zaman, gecisTipi);

            try
            {
                var sonuc = await smsService.Gonder(bilgi.VeliTelefon!, mesaj, ct);
                if (sonuc.Basarili)
                {
                    log.OgrenciSmsGonderildi = true;
                    _logger.LogInformation("[SMS RETRY OK][AnaKapi] OgrId:{OgrId} Tip:{Tip}", log.OgrenciId, gecisTipi);
                }
                else
                {
                    _logger.LogWarning("[SMS RETRY FAIL][AnaKapi] OgrId:{OgrId} Hata:{Hata}", log.OgrenciId, sonuc.Hata);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SMS RETRY EX][AnaKapi] OgrId:{OgrId}", log.OgrenciId);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
