using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OgrenciBilgiSistemi.Models.Options;
using OgrenciBilgiSistemi.Services.Interfaces;

namespace OgrenciBilgiSistemi.Services.Implementations;

public sealed class SmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly SmsAyarlari _ayar;

    public SmsService(HttpClient http, IOptions<SmsAyarlari> ayar)
    {
        _http = http;
        _ayar = ayar.Value;
        _http.Timeout = TimeSpan.FromSeconds(_ayar.ZamanAsimiSaniye > 0 ? _ayar.ZamanAsimiSaniye : 30);
    }

    public async Task<SmsGonderimSonucu> Gonder(string telefon, string mesaj, CancellationToken ct = default)
    {
        if (!_ayar.Aktif)
            return new SmsGonderimSonucu(true, HamCevap: "SMS devre dışı.");

        if (string.IsNullOrWhiteSpace(telefon))
            return new SmsGonderimSonucu(false, Hata: "Telefon boş.");

        if (string.IsNullOrWhiteSpace(mesaj))
            return new SmsGonderimSonucu(false, Hata: "Mesaj boş.");

        var gsm05 = TelefonNormalize(telefon);
        if (gsm05 is null)
            return new SmsGonderimSonucu(false, Hata: "Geçersiz telefon formatı.");

        var metin = _ayar.TurkceKarakter ? mesaj : TurkceTemizle(mesaj);

        var telMesajlarJson = JsonSerializer.Serialize(
            new[] { new { mesaj = metin, telefon = gsm05 } });

        var form = new[]
        {
            new KeyValuePair<string, string>("apiUsername", _ayar.KullaniciAdi),
            new KeyValuePair<string, string>("apiPassword", _ayar.Sifre),
            new KeyValuePair<string, string>("baslik", _ayar.Baslik),
            new KeyValuePair<string, string>("tur", _ayar.TurkceKarakter ? "turkce" : "normal"),
            new KeyValuePair<string, string>("telMesajlar", telMesajlarJson)
        };

        using var content = new FormUrlEncodedContent(form);
        content.Headers.ContentType!.CharSet = "utf-8";

        HttpResponseMessage resp;
        try
        {
            resp = await _http.PostAsync(_ayar.ApiUrl, content, ct);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            return new SmsGonderimSonucu(false, Hata: "İstek zaman aşımı.", HamCevap: ex.Message);
        }
        catch (Exception ex)
        {
            return new SmsGonderimSonucu(false, Hata: ex.Message, HamCevap: ex.ToString());
        }

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new SmsGonderimSonucu(false, Hata: $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}", HamCevap: body);

        return new SmsGonderimSonucu(true, HamCevap: body);
    }

    /// <summary>
    /// Telefon numarasını 05XXXXXXXXX formatına normalize eder.
    /// </summary>
    private static string? TelefonNormalize(string input)
    {
        var d = new string(input.Where(char.IsDigit).ToArray());

        // 5XXXXXXXXX -> 05XXXXXXXXX
        if (d.Length == 10 && d.StartsWith('5')) return "0" + d;
        // 05XXXXXXXXX
        if (d.Length == 11 && d.StartsWith("05")) return d;
        // 905XXXXXXXXX -> 05XXXXXXXXX
        if (d.Length == 12 && d.StartsWith("90") && d[2] == '5') return "0" + d[2..];
        // 0905XXXXXXXXX -> 05XXXXXXXXX
        if (d.Length == 13 && d.StartsWith("090") && d[3] == '5') return "0" + d[3..];

        return null;
    }

    private static string TurkceTemizle(string s) =>
        s.Replace('ç', 'c').Replace('ğ', 'g').Replace('ı', 'i').Replace('ö', 'o').Replace('ş', 's').Replace('ü', 'u')
         .Replace('Ç', 'C').Replace('Ğ', 'G').Replace('İ', 'I').Replace('Ö', 'O').Replace('Ş', 'S').Replace('Ü', 'U');
}
