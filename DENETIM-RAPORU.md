# OgrenciBilgiSistemi — Uçtan Uca Mimari/Güvenlik Denetimi

## Önemli düzeltmeler (siz başlamadan)

- **CLAUDE.md "ORM: EF Core" diyor.** Doğru ama eksik: yalnızca **MVC projesi EF Core** kullanır. **API ham ADO.NET (`SqlConnection`/`SqlCommand`)** ile aynı veritabanına yazıyor. İki yazma yolunun olması, gelecekteki şema değişikliklerinde tutarsızlık riski yaratır (örn. EF migration ile eklenen yeni kolon, ADO.NET sorgularına manuel yansıtılmazsa runtime hatası).
- **"Production secret'ları repo'da" iddiası YANLIŞ.** `git check-ignore` ile doğrulandı: hem `OgrenciBilgiSistemi/appsettings.json` hem `OgrenciBilgiSistemi.Api/appsettings.Production.json` `.gitignore`'da, hiçbiri git'te tracked değil. Yine de **yerel dosyalarda plaintext** duruyor (developer makinesi sızarsa risk vardır).
- Aşağıdaki bulgular paralel okuma ajanlarından geldi. Yüksek-şiddet olanlar için file:line referansları verilen her satırı kendiniz açıp doğrulamanız önerilir — şiddet derecelendirmesi tartışmaya açıktır.

---

## 1. Hatalar ve Buglar

### Yüksek
- **`async void` event handler'da try/catch yok** — `OgrenciBilgiSistemi.Mobil/App.xaml.cs:59-64` (`OturumSuresiDolduHandler`). İçinde bir exception olursa **tüm uygulama crash olur** (MAUI `SynchronizationContext` async void exception'ı yakalamaz). Aynı pattern: `RandevuOlusturView.OnOlusturClicked` ve birçok `OnXClicked` handler'ı.
- **Fire-and-forget Task'lar** — `App.xaml.cs:72`, `Views/OgrenciListeView.xaml.cs:28` (`_ = LoadStudents()`), `Views/RandevuDetayView.xaml.cs:19`. Exception'lar `UnobservedTaskException`'a düşer; production'da CrashLog yoksa tanı imkansız.
- **`TemelApiService.OturumSuresiDoldu` static event leak** — `App.xaml.cs:13-14` her açılışta abonelik ekliyor; başka yerlerde ise `-=` ile çıkış yok. Static event Page referansı tutarsa **bellek sızıntısı + duplicate handler tetikleme**.
- **`.NET process timezone` ile `SQL GETDATE()` karışımı** — `OgrenciService.cs:217` SQL `GETDATE()` (server local), aynı sorguda `.NET DateTime.Now` ile karşılaştırma var. Sunucu UTC, app local timezone ise yoklama/randevu kayıtları **3 saat kayabilir**.

### Orta
- **`User.FindFirst("rol")!.Value` null-forgiving operatörü** — `RandevularController.cs:23-24`, `OgretmenRandevuController.cs:20-21`, `ServisController.cs:90-91`, `BildirimlerController.cs:19`. Token claim eksikse `NullReferenceException` → 500. Token doğrulanmış sayılsa bile, claim setine veri eklenip çıkarılırken kırılma kaynağı olur.
- **`Application.Current.MainPage.Handler.MauiContext.Services.GetService<T>()`** — `RandevuOlusturView.xaml.cs:29-33`. Service Locator anti-pattern; `MainPage.Handler` startup race condition'da null olabilir. Constructor injection (DI) yerine kullanılması fragile.
- **Mobile `Debug.WriteLine` ile yutulmuş hatalar** — `Services/` altında 30+ yerde. `Debug.WriteLine` Release build'de **hiçbir yere yazmaz**. Production'da bir API hatası tanı imkansız.

### Düşük
- **Mobil `Models/Ogrenci.cs`'de `OgrenciAdSoyad` non-nullable string** ama `string.Empty` initializer'ı yok, deserialization'da JSON'da alan yoksa null kalır → ilk `.ToUpper()` çağrısında NRE. Modellerde `string` → `string = ""` veya `string?`.

---

## 2. Kod Kalitesi

### Yüksek
- **TemelApiService 401 handler 4 kez tekrar ediliyor** — GET/POST/PUT/DELETE her birinde aynı 401-refresh-retry bloğu yapışık. Generic `IsleVeYenile<T>(...)` wrapper ile tek metoda indirilebilir.
- **MVC `OgrencilerController.Guncelle` tarzı endpoint'lerde resource ownership kontrolü yok** — Veli rolündeki kullaıcı `?id={baska_ogrenci_id}` URL'i ile başka öğrencileri düzenleme/görüntüleme deneyebilir (IDOR). API tarafında `OgrencilerController.OgrenciyeErisimKontrol` doğru çalışıyor (API:127-150) ama MVC'de bu sistemli kontrol yok. (Bunu Yüksek olarak Güvenlik bölümünde tekrar listeliyorum.)

### Orta
- **API: `GirisIstegiDto`** — `Dtos/GirisIstegiDto.cs:4` data annotation yok (`[Required]`, `[StringLength]`). Validasyon controller içinde manuel.
- **MVC: AppDbContext `[Required]`/`StringLength` modellerde mevcut** ama input akışına geçtikçe DTO/ViewModel'lere de yansıtılmamış (nesne aynı tip değil). Hat boyu birkaç yerde double validation eksikliği.
- **MAUI'de MVVM pattern eksik** — `OgrenciDetayView.xaml.cs:68-76` direkt `LblClass.Text = detay.BirimAd`. Code-behind'de bind yerine değer atama. CLAUDE.md "ViewModel suffix `GorunumModel`" diyor ama view code-behind'leri bunu dışlıyor.

### Düşük
- **Çok geniş `catch (Exception ex)` blokları** — Network timeout, OutOfMemory, NRE aynı şekilde yutuluyor.
- **CollectionView UI refresh hack** — `BildirimListeView.xaml.cs:47-48` `ItemsSource = null; ItemsSource = listesi;`. `ObservableCollection` + `INotifyPropertyChanged` kullanılmalı.

---

## 3. Performans Problemleri

### Yüksek (5000 eşzamanlı veli ölçeğinde)
- **`GET /api/ogrenciler/tumu` sayfalama yok** — `OgrencilerController.cs:80-96`. 1000 öğrenci × 5000 eş zamanlı veli = en kötü senaryoda saniyede yüzlerce tam liste fetch'i. Hafıza ve ağ darboğazı.
- **Mobil veli randevu akışı tüm öğrencileri çekip client-side filtreliyor** — `RandevuOlusturView.OgretmenIcinHazirla` → `TumOgrencileriGetirAsync()`. Öğretmen başına tam liste; 1000 öğrenci × öğretmen sayısı kadar trafik.
- **`OnAppearing()` her seferinde fetch** — `RandevuListeView`, `BildirimListeView`, `OgrenciListeView`. Shell navigation'da geri gel/git her seferinde tam GET. Client cache + `If-Modified-Since` veya basit timestamp guard yok.

### Orta
- **API'de SqlException → InvalidOperationException dönüştürülürken stack çiftleniyor** — `OgrenciService.cs` ve diğerleri. Zarar değil ama log gürültüsü artar.
- **Çoklu `await using SqlConnection`** — Her SQL sorgusunda yeni connection açılıyor; pooling default açık ama `MultipleActiveResultSets=true` connection string'de var, neden gerekli olduğu belirsiz (MARS'ın olmaması daha iyi performans).
- **Image yükleme cache yok** — Mobile `Image.Source = url` direkt; FFImageLoading veya `CachedImage` eklenmemiş. Liste açılışlarında her seferinde resim re-download.

### Düşük
- **CollectionView `ItemsLayout` tanımlı ama virtualisation override'ı yok** — `RandevuListeView.xaml`. 100+ item için sorun olmaz ama bir öğrenciye 200 randevu birikirse fark eder.

---

## 4. Güvenlik Açıkları

### Yüksek
- **HTTP cleartext etkin** — `Platforms/Android/AndroidManifest.xml:7` `usesCleartextTraffic="true"` + `Platforms/iOS/Info.plist` `NSAllowsArbitraryLoads=true`. Login sırasında JWT, öğrenci verisi açık iletilebilir → MITM. **App Store/Play Store onay sürecinde de problem.**
- **`Constants.cs:7,10`** — Default API URL `http://81.214.75.22:5196/api/` ve `http://www.netronyazilim.com/okullar.json`. Mobile'in default'unun HTTP olması, kullanıcı çevrimdışı moda düşüp ilk yüklemede zafiyetli endpoint'e gidebilir.
- **MVC IDOR riski** — `OgrencilerController.Guncelle` (MVC) per-resource ownership kontrolü yok; sadece global `MenuYetkiFilter`. Veli rolü başka öğrenciye erişebilir.
- **Logout sunucu tarafında garantili değil** — `KullaniciOturum.LogoutApiCagirAsync` hata olursa local temizleyip sessizce dönüyor; sunucu refresh token tablosundan silmediyse token aktif kalmaya devam eder. (API'de `KimlikDogrulamaController.CikisYap` doğru implementasyon, sorun mobil tarafta yarım kalan istek senaryosu.)
- **Telefon numarası loga yazılıyor** — MVC `SmsGonderimService.cs:93-94` `_logger.LogInformation("... Telefon: {Tel}", veliTelefon)` ham olarak. KVKK ihlali; mask'leyin (`+9053****1234`).

### Orta
- **`appsettings.Production.json` yerel diskte plaintext DB şifresi + JWT secret'ı** — git'te yok ama developer makinesi/server backup'ı kompromize olursa direkt erişim. **Üretim'de Azure Key Vault, AWS Secrets Manager, Docker Secrets veya en azından environment variable** olmalı.
- **`TrustServerCertificate=True` connection string'de** — SQL Server TLS sertifikası doğrulanmıyor. Aynı LAN'da ARP spoofing riski.
- **Cookie SameSite/Secure eksik (MVC)** — `Program.cs:126-134` `AddCookie` çağrısında `Cookie.SameSite` ve `Cookie.Secure` belirtilmemiş. CSRF + cookie sızması riski.
- **`KartOkuController.UsbKartOkundu` `[IgnoreAntiforgeryToken]`** — Cihaz GUID kontrolü mitigasyon olarak var ama anti-forgery yerine geçmez. Cihaz IP whitelist + mTLS daha güvenli.
- **Swagger Production'da koşulsuz aktif** — API `Program.cs:124-125`. `if (app.Environment.IsDevelopment())` guard'ı eksik. Endpoint enumeration kolaylaşır.
- **CORS politikasında `AllowAnyMethod` + `AllowAnyHeader`** — API `Program.cs:39-56`. `WithOrigins` doğru sınırlanmış ama method/header sınırı yok. Spesifik liste verin.
- **JWT secret ~40 karakter** — `appsettings.Production.json:10`. HMAC-SHA256 için 32 byte yeterli ama görsel olarak zayıf rastgele; `RandomNumberGenerator.GetBytes(32)` ile üretip Base64 encode edin (≥44 karakter).
- **MVC'de `AllowedHosts: "*"`** — Host header poisoning. `"yourdomain.com,*.yourdomain.com"` yapın.

### Düşük
- **API `GET /api/randevular/cakisma-kontrolu`** (yeni eklenen endpoint) `karsiTarafKullaniciId` parametre'sini doğrulamıyor — kullanıcı, kendisinin hiç ilgisinin olmadığı bir veliId/ogretmenId vererek o kişinin randevu programının "boşluk haritasını" çıkarabilir. Bilgi sızıntısı düşük ama gerçek; düzeltme: sadece kendi takvimini kontrol et veya karşı tarafı doğrula.

---

## 5. Mimari Değerlendirme

### Olumlu
- **Katmanlı yapı net:** Models → AppDbContext → Services → Controllers → Views (MVC); Dtos → Controllers → Services → Models (API). Klasör organizasyonu CLAUDE.md ile tutarlı.
- **DI tutarlılığı yüksek (MVC):** Services Scoped, DbContext per-request, ZKTeco Singleton (cihaz state'i için doğru karar). `Program.cs:89-108` bütün kayıtlar açık ve tutarlı.
- **Soft-delete global query filter** — `AppDbContext.cs:47-48` ve sonrası, hiyerarşik olarak doğru kurulmuş.
- **Magic-byte file validation** — `LocalFileStorage.cs:19-30` JPEG/PNG header kontrolü + GUID dosya adı + 2MB limit + uzantı whitelist'i. Dosya yükleme açısından OWASP referans implementasyonu seviyesinde.
- **Multi-tenant `TenantBaglami` deseni** — JWT'deki `okulKodu` → server'da config lookup. Connection string client'tan değil, doğrulanmış claim'den çözülüyor.

### Olumsuz
- **MVC + API çift veri-erişim kanalı:** Aynı tabloya hem EF Core (MVC) hem ADO.NET (API) yazıyor. Migration eklendiğinde API'deki ham SQL'in unutulma riski yüksek; tek-yazıcı kuralı tavsiye edilir veya API'nin de EF Core'a geçirilmesi.
- **Repository pattern yok** — Service katmanı DbContext'i doğrudan kullanıyor; test edilebilirlik için `IRepository<T>` veya en azından servis interface'lerinin mock'lanabilir hâle getirilmesi (zaten `I*Service` var ama servis içinde `_db` reach-through).
- **Test projesi yok** — Repo'da `*.Tests.csproj` görünmüyor. 1000 öğrenci + 5000 veli ölçeğindeki bir sistem için unit test eksikliği release sonrası regresyon riskini katlar.
- **MAUI service locator** — DI yerine `MainPage.Handler.MauiContext.Services.GetService<T>()`. Constructor injection ile her View'e service'ler enjekte edilmeli (MAUI Shell ile uyumlu).
- **Refresh token bellek-içi** — `RefreshTokenService` (API) sunucu restart'ında tüm session'lar kırılır + horizontal scaling imkansız. Redis veya DB'ye taşınmalı.
- **Loglama altyapısı Microsoft.Extensions.Logging default** — Structured logging (Serilog), centralized aggregation (ELK/App Insights), correlation ID middleware yok. 5000 eşzamanlı kullanıcıda incident analizi çok yavaşlar.

---

## 6. DTO & Model Uyumluluğu

- **API DTO ↔ Mobil Model:** Random örnekler (Randevu, Ogrenci, RandevuSlot) eşleşiyor. JSON `PropertyNameCaseInsensitive=true` ayarı mobile'de korumayı güçlendiriyor (`TemelApiService` `_jsonOptions`).
- **Bulgu:** Mobil `Models/Ogrenci.cs`'de bazı alanlar API'de döndürülüyor olabilir ama mobile'de yok (örn. `OgrenciKartNo`, `OgrenciCikisDurumu`). API'nin response model'i `OgrenciModel` (`/Models`) ile mobile'in `Ogrenci`'sini periyodik karşılaştıran bir mekanizma yok — değişikliklerde sessiz drift olur.
- **Bulgu:** Mobile `Models/GecisKayit.cs`'de bazı alanlarda `[JsonPropertyName]` var, `Models/Ogrenci.cs`'de yok. Tutarsız; bir gün API serializer ayarı değişirse bazı modeller kırılır.
- **Bulgu:** `DateTime` alanlarda hiçbir model UTC anlaşması yapmamış. API SQL `GETDATE()` kullanıyor (server local), JSON serializer ISO 8601 ile gönderiyor; istemci `DateTime.Parse` ile yerel zaman olarak alıyor. Sınırı UTC'ye çekin.
- **Bulgu:** `DurumAdi` gibi türetilebilir alanlar hem server hem client'ta üretiliyor (`RandevuModel.DurumAdi` API tarafında, `RandevuDetayView.xaml.cs:82` switch'inde mobile tarafında). Tek kaynak (Shared enum + extension method) yapın.
- **Öneri:** `OgrenciBilgiSistemi.Shared` projesi şu an sadece enum'lar içeriyor. DTO'ları da Shared'e taşıyıp her üç projede (MVC, API, Mobile) referanslayabilirsiniz — drift'i sıfırlar.

---

## 7. İyileştirme Önerileri (örnek kod ile)

### a) Async void handler crash koruması (Yüksek, Mobile)

```csharp
// Önce
private static async void OturumSuresiDolduHandler()
{
    await MainThread.InvokeOnMainThreadAsync(async () => { ... });
}

// Sonra
private static async void OturumSuresiDolduHandler()
{
    try
    {
        await MainThread.InvokeOnMainThreadAsync(async () => { ... });
    }
    catch (Exception ex)
    {
        // Crashlytics / AppCenter / Sentry'e gönder
        System.Diagnostics.Debug.WriteLine($"[FATAL] {ex}");
    }
}
```

### b) Null-safe claim okuma (Orta, API)

```csharp
// Önce
private string Rol => User.FindFirst("rol")!.Value;

// Sonra
private string Rol => User.FindFirst("rol")?.Value ?? string.Empty;
private bool RolKontrol(params string[] izinli) => izinli.Contains(Rol);
```

### c) TLS zorunlu (Yüksek, Mobile)

```xml
<!-- Platforms/Android/AndroidManifest.xml -->
<application android:usesCleartextTraffic="false" ...>

<!-- Platforms/iOS/Info.plist -->
<!-- NSAppTransportSecurity bloğunu tamamen sil veya: -->
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <false/>
</dict>
```

### d) PII maskeleme (Yüksek, MVC)

```csharp
// Services/SmsGonderimService.cs
private static string MaskTelefon(string t) =>
    string.IsNullOrEmpty(t) ? "-" : (t.Length < 7 ? "***" : $"{t[..3]}****{t[^3..]}");

_logger.LogInformation("SMS gönderildi. Öğrenci: {OgrId}, Tel: {Tel}",
    ogrenciId, MaskTelefon(veliTelefon));
```

### e) Sayfalama (Yüksek, API)

```csharp
[HttpGet("tumu")]
public async Task<IActionResult> TumOgrencileriGetir(
    [FromQuery] int sayfaNo = 1, [FromQuery] int sayfaBoyut = 100)
{
    if (sayfaBoyut > 500) sayfaBoyut = 500;  // hard cap
    var ogrenciler = await _ogrenciService.AktifOgrencileriSayfaliGetirAsync(sayfaNo, sayfaBoyut);
    return Ok(ogrenciler);
}
```

### f) Production-safe Swagger (Orta, API)

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### g) Cookie güvenlik bayrakları (Orta, MVC)

```csharp
.AddCookie(o =>
{
    o.LoginPath = "/Hesaplar/Giris";
    o.ExpireTimeSpan = TimeSpan.FromHours(8);
    o.SlidingExpiration = true;
    o.Cookie.HttpOnly = true;
    o.Cookie.Secure = true;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.IsEssential = true;
});
```

### h) Refresh token kalıcı saklama (Yüksek, API)

In-memory `RefreshTokenService` → SQL Server tablosu (`RefreshTokenlar(TokenHash, KullaniciId, GecerlilikSonu, IptalEdildi)`). Token rotation aynı kalır, sunucu restart hayatta kalır, horizontal scaling açılır.

---

## 8. Önceliklendirme

### P0 — Üretime Çıkmadan Önce (Yüksek)
1. **Mobil HTTPS zorunlu** (Android cleartext + iOS ATS) — *2 satır config + production URL'yi `https://` yap.*
2. **`async void` handler'larda try/catch** — *Tüm event handler'ları toplu tarayıp wrap et.*
3. **Telefon numarası loga maskeleme** — KVKK uyum.
4. **MVC IDOR — Veli için resource ownership kontrolü** — `OgrencilerController.Guncelle/Sil/Detay` gibi tüm action'larda.
5. **Refresh token kalıcı saklama** — In-memory'den SQL'e.
6. **JWT secret ≥32 byte rastgele Base64** — local dosyada bile olsa. Üretim'e çıkmadan generate edip rotate edin.
7. **`appsettings.Production.json` üzerine ENV-VAR override** — `Production` ortamında dosya yerine `ConnectionStrings__Default`, `Jwt__SecretKey` env var.

### P1 — İlk Sprint (Orta)
8. Null-safe claim okuma (4 controller).
9. Cookie SameSite + Secure + HttpOnly (MVC).
10. Sayfalama: `/api/ogrenciler/tumu` ve mobile'de tüm öğrenci çekip filtreleme.
11. `OnAppearing()` cache stratejisi (timestamp + sliding TTL).
12. Swagger production guard'ı.
13. Global rate limiter (sadece `arama` değil).
14. CORS `AllowAnyMethod`/`AllowAnyHeader` daraltılması.
15. DateTime'ları UTC'ye taşı (SQL'de `GETUTCDATE()`, .NET'te `DateTime.UtcNow`).
16. Mobile static event leak: `App.xaml.cs`'de `OturumSuresiDoldu` aboneliğini `OnSleep`/`OnResume`'da yönet.
17. Crash reporting entegrasyonu (Sentry / AppCenter / Crashlytics).
18. Mobile `Debug.WriteLine` → `ILogger` veya in-memory ring buffer + uzaktan toplama.

### P2 — Backlog
19. Refresh token TTL 8s → 7 gün, access token 30dk (mobile UX için).
20. Health check endpoint (`/health`).
21. Structured logging (Serilog) + correlation ID middleware.
22. Repository pattern + unit test projesi.
23. DTO'ları `Shared` projeye taşı (drift'i bitir).
24. Image cache (FFImageLoading veya `CachedImage`).
25. ZKTeco `TryDisposeCom`'un gerçekten `Marshal.ReleaseComObject` çağırdığını doğrula.
26. MARS bağımlılığını gerçekten gerekiyor mu diye gözden geçir.

---

## Öne çıkan üç eylem

İlk hafta için somut hedef: **(1) HTTPS zorunlu**, **(2) async void handler crash koruması**, **(3) MVC IDOR resource ownership** — bu üçü kapatılmadan üretime çıkmamak (sırasıyla App Store red riski, müşteri-gözünde crash, kişisel veri erişim ihlali).
