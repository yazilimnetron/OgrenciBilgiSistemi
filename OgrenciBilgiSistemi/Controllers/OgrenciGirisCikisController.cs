using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.ViewModels;

namespace OgrenciBilgiSistemi.Controllers
{
    public class OgrenciGirisCikisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OgrenciGirisCikisController> _logger;

        public OgrenciGirisCikisController(AppDbContext context, ILogger<OgrenciGirisCikisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Detay(
            string? sortOrder,
            string? searchString,
            int page = 1,
            RaporTipi raporTipi = RaporTipi.Tumu,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken ct = default)
        {
            // Sayfa guard
            if (page < 1) page = 1;

            // Rapor tipinden geçişler için istasyon filtresi türet
            IstasyonTipi? istasyonTipi = raporTipi switch
            {
                RaporTipi.AnaKapiGecisleri => IstasyonTipi.AnaKapi,
                RaporTipi.YemekhaneGecisleri => IstasyonTipi.Yemekhane,
                _ => null
            };

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["RaporTipi"] = ((int)raporTipi).ToString();

            var vm = new OgrenciDetayRaporVm { RaporTipi = raporTipi };

            // === SINIF YOKLAMASI ===
            if (raporTipi == RaporTipi.SinifYoklamasi)
            {
                var sq = _context.SinifYoklamalar
                    .AsNoTracking()
                    .Include(x => x.Ogrenci).ThenInclude(o => o.Birim)
                    .Include(x => x.Kullanici)
                    .AsQueryable();

                if (startDate.HasValue)
                {
                    var s = startDate.Value.Date;
                    sq = sq.Where(x => x.OlusturulmaTarihi >= s);
                }
                if (endDate.HasValue)
                {
                    var e = endDate.Value.Date.AddDays(1);
                    sq = sq.Where(x => x.OlusturulmaTarihi < e);
                }
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    var s = searchString.Trim();
                    sq = sq.Where(x => x.Ogrenci != null && x.Ogrenci.OgrenciAdSoyad != null &&
                        EF.Functions.Like(x.Ogrenci.OgrenciAdSoyad, $"%{s}%"));
                }

                sq = sq.OrderByDescending(x => x.OlusturulmaTarihi);
                vm.SinifYoklamalar = await SayfalanmisListeModel<SinifYoklamaModel>.CreateAsync(sq, page, 50, ct);
                return View(vm);
            }

            // === SERVİS YOKLAMASI ===
            if (raporTipi == RaporTipi.ServisYoklamasi)
            {
                var sq = _context.ServisYoklamalar
                    .AsNoTracking()
                    .Include(x => x.Ogrenci).ThenInclude(o => o.Birim)
                    .Include(x => x.Kullanici)
                    .AsQueryable();

                if (startDate.HasValue)
                {
                    var s = startDate.Value.Date;
                    sq = sq.Where(x => x.OlusturulmaTarihi >= s);
                }
                if (endDate.HasValue)
                {
                    var e = endDate.Value.Date.AddDays(1);
                    sq = sq.Where(x => x.OlusturulmaTarihi < e);
                }
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    var s = searchString.Trim();
                    sq = sq.Where(x => x.Ogrenci != null && x.Ogrenci.OgrenciAdSoyad != null &&
                        EF.Functions.Like(x.Ogrenci.OgrenciAdSoyad, $"%{s}%"));
                }

                sq = sq.OrderByDescending(x => x.OlusturulmaTarihi);
                vm.ServisYoklamalar = await SayfalanmisListeModel<ServisYoklamaModel>.CreateAsync(sq, page, 50, ct);
                return View(vm);
            }
            // === GEÇİŞLER (Tumu / AnaKapı / Yemekhane) ===

            // Ana sorgu
            var q = _context.OgrenciDetaylar
                .AsNoTracking()
                .Include(x => x.Ogrenci).ThenInclude(o => o.Birim)
                .Include(x => x.Cihaz)
                .AsQueryable();

            // Tarih aralığı
            if (startDate.HasValue)
            {
                var s = startDate.Value.Date;
                q = q.Where(x => (x.OgrenciGTarih ?? x.OgrenciCTarih) >= s);
            }
            if (endDate.HasValue)
            {
                var e = endDate.Value.Date.AddDays(1);
                q = q.Where(x => (x.OgrenciGTarih ?? x.OgrenciCTarih) < e);
            }

            // İstasyon filtresi
            if (istasyonTipi.HasValue)
                q = q.Where(x => x.IstasyonTipi == istasyonTipi.Value);

            // Arama: AdSoyad / No / KartNo
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim();

                if (int.TryParse(s, out var no))
                {
                    q = q.Where(x =>
                        (x.Ogrenci != null && x.Ogrenci.OgrenciNo == no) ||
                        (x.Ogrenci != null && x.Ogrenci.OgrenciKartNo != null && (
                            x.Ogrenci.OgrenciKartNo == s ||
                            EF.Functions.Like(x.Ogrenci.OgrenciKartNo, $"%{s}%")
                        )) ||
                        (x.Ogrenci != null && x.Ogrenci.OgrenciAdSoyad != null && (
                            EF.Functions.Like(EF.Functions.Collate(x.Ogrenci.OgrenciAdSoyad, "Turkish_100_CI_AI"), $"%{s}%") ||
                            EF.Functions.Like(EF.Functions.Collate(x.Ogrenci.OgrenciAdSoyad, "Latin1_General_CI_AI"), $"%{s}%")
                        )));
                }
                else
                {
                    q = q.Where(x =>
                        x.Ogrenci != null && (
                            (x.Ogrenci.OgrenciAdSoyad != null && (
                                EF.Functions.Like(EF.Functions.Collate(x.Ogrenci.OgrenciAdSoyad, "Turkish_100_CI_AI"), $"%{s}%") ||
                                EF.Functions.Like(EF.Functions.Collate(x.Ogrenci.OgrenciAdSoyad, "Latin1_General_CI_AI"), $"%{s}%")
                            )) ||
                            (x.Ogrenci.OgrenciKartNo != null &&
                                EF.Functions.Like(x.Ogrenci.OgrenciKartNo, $"%{s}%"))
                        )
                    );
                }
            }

            // Sıralama
            q = sortOrder switch
            {
                "AdSoyad" => q.OrderBy(x => x.Ogrenci!.OgrenciAdSoyad).ThenBy(x => x.Ogrenci!.OgrenciNo),
                "AdSoyad_desc" => q.OrderByDescending(x => x.Ogrenci!.OgrenciAdSoyad).ThenBy(x => x.Ogrenci!.OgrenciNo),
                "No" => q.OrderBy(x => x.Ogrenci!.OgrenciNo).ThenBy(x => x.Ogrenci!.OgrenciAdSoyad),
                "No_desc" => q.OrderByDescending(x => x.Ogrenci!.OgrenciNo).ThenBy(x => x.Ogrenci!.OgrenciAdSoyad),
                "Tarih" => q.OrderBy(x => (x.OgrenciGTarih ?? x.OgrenciCTarih)).ThenBy(x => x.OgrenciDetayId),
                "Tarih_desc" => q.OrderByDescending(x => (x.OgrenciGTarih ?? x.OgrenciCTarih)).ThenByDescending(x => x.OgrenciDetayId),
                _ => q.OrderByDescending(x => (x.OgrenciGTarih ?? x.OgrenciCTarih)).ThenByDescending(x => x.OgrenciDetayId)
            };

            // Sayfalama
            vm.Gecisler = await SayfalanmisListeModel<OgrenciDetayModel>.CreateAsync(q, page, 50, ct);
            return View(vm);
        }

        // Tek öğrencinin giriş-çıkışları
        [HttpGet]
        public async Task<IActionResult> GirisCikisDetay(
            int id,
            DateTime? startDate,
            DateTime? endDate,
            RaporTipi raporTipi = RaporTipi.Tumu,
            int? pageNumber = null,
            CancellationToken ct = default)
        {
            // Rapor tipinden istasyon filtresi türet (geçişler için)
            IstasyonTipi? istasyonTipi = raporTipi switch
            {
                RaporTipi.AnaKapiGecisleri => IstasyonTipi.AnaKapi,
                RaporTipi.YemekhaneGecisleri => IstasyonTipi.Yemekhane,
                _ => null
            };

            // Geçişler bu rapor tiplerinde gösterilir
            bool gecisGoster = raporTipi == RaporTipi.Tumu
                            || raporTipi == RaporTipi.AnaKapiGecisleri
                            || raporTipi == RaporTipi.YemekhaneGecisleri;
            var ogrenci = await _context.Ogrenciler
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OgrenciId == id);

            if (ogrenci is null) return NotFound();

            var q = _context.OgrenciDetaylar
                .AsNoTracking()
                .Include(d => d.Cihaz)
                .Include(d => d.Ogrenci)
                .Where(d => d.OgrenciId == id);

            // --- Tarih filtreleri (Giriş veya Çıkış kapsayıcı) ---
            var hasStart = startDate.HasValue;
            var hasEnd = endDate.HasValue;
            var s = startDate?.Date;
            var eExclusive = endDate?.Date.AddDays(1);

            if (hasStart && hasEnd)
            {
                q = q.Where(d =>
                    (d.OgrenciGTarih.HasValue && d.OgrenciGTarih.Value >= s!.Value && d.OgrenciGTarih.Value < eExclusive!.Value) ||
                    (d.OgrenciCTarih.HasValue && d.OgrenciCTarih.Value >= s!.Value && d.OgrenciCTarih.Value < eExclusive!.Value));
            }
            else if (hasStart)
            {
                q = q.Where(d =>
                    (d.OgrenciGTarih.HasValue && d.OgrenciGTarih.Value >= s!.Value) ||
                    (d.OgrenciCTarih.HasValue && d.OgrenciCTarih.Value >= s!.Value));
            }
            else if (hasEnd)
            {
                q = q.Where(d =>
                    (d.OgrenciGTarih.HasValue && d.OgrenciGTarih.Value < eExclusive!.Value) ||
                    (d.OgrenciCTarih.HasValue && d.OgrenciCTarih.Value < eExclusive!.Value));
            }

            if (istasyonTipi.HasValue)
            {
                q = q.Where(d => d.Cihaz != null && d.Cihaz.IstasyonTipi == istasyonTipi.Value);
            }

            // En az bir tarihi olanlar
            q = q.Where(d => d.OgrenciGTarih.HasValue || d.OgrenciCTarih.HasValue);

            // --- Sıralama (Gün DESC → PairIndex ASC → Giriş önce → Zaman DESC → Id DESC) ---
            q = q
                // 1) Gün (en yeni gün en üstte)
                .OrderByDescending(d => (d.OgrenciGTarih ?? d.OgrenciCTarih)!.Value.Date)

                // 2) Pair index: Aynı gün, kendisinden BÜYÜK tüm Giriş sayısı + 1
                //    Böylece her "Giriş" ile onu izleyen "Çıkış" aynı pairIndex alır.
                .ThenBy(d =>
                    1 + _context.OgrenciDetaylar
                        .Where(x =>
                            x.OgrenciId == d.OgrenciId &&
                            x.OgrenciGTarih.HasValue &&
                            // aynı gün
                            EF.Functions.DateDiffDay(
                                x.OgrenciGTarih.Value,
                                (d.OgrenciGTarih ?? d.OgrenciCTarih)!.Value) == 0 &&
                            // kendisinden büyük Girişler
                            x.OgrenciGTarih.Value > (d.OgrenciGTarih ?? d.OgrenciCTarih)!.Value
                        )
                        .Count()
                )

                // 3) Aynı pair içinde Giriş önce, ardından Çıkış
                .ThenByDescending(d => d.OgrenciGTarih.HasValue)

                // 4) Tür içi zaman DESC (daha yeni üstte)
                .ThenByDescending(d => d.OgrenciGTarih ?? d.OgrenciCTarih)

                // 5) Deterministik tie-breaker
                .ThenByDescending(d => d.OgrenciDetayId);

            var proj = q.Select(h => new OgrenciGirisCikisVm
            {
                OgrenciDetayId = h.OgrenciDetayId,
                OgrenciAdSoyad = h.Ogrenci != null ? h.Ogrenci.OgrenciAdSoyad : "Bilinmiyor",
                OgrenciKartNo = h.Ogrenci != null ? h.Ogrenci.OgrenciKartNo : "-",
                OgrenciGTarih = h.OgrenciGTarih,
                OgrenciCTarih = h.OgrenciCTarih,
                OgrenciGecisTipi = h.OgrenciGecisTipi,
                CihazAdi = h.Cihaz != null ? h.Cihaz.CihazAdi : "Bilinmiyor"
            });

            var pageIndex = pageNumber.GetValueOrDefault(1);
            // Geçişler kapalıysa boş sayfa döndür
            var hareketlerPaged = gecisGoster
                ? await SayfalanmisListeModel<OgrenciGirisCikisVm>.CreateAsync(proj, pageIndex, 25, ct)
                : await SayfalanmisListeModel<OgrenciGirisCikisVm>.CreateAsync(
                    Enumerable.Empty<OgrenciGirisCikisVm>().AsQueryable(), 1, 25, ct);

            // --- Sınıf ve Servis yoklamaları (rapor tipine göre) ---
            var sinifYoklamalar = (raporTipi == RaporTipi.Tumu || raporTipi == RaporTipi.SinifYoklamasi)
                ? await SinifYoklamaSorgusu(id, startDate, endDate)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToListAsync(ct)
                : new List<SinifYoklamaModel>();

            var servisYoklamalar = (raporTipi == RaporTipi.Tumu || raporTipi == RaporTipi.ServisYoklamasi)
                ? await ServisYoklamaSorgusu(id, startDate, endDate)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToListAsync(ct)
                : new List<ServisYoklamaModel>();

            var vm = new OgrenciGirisCikisListViewModel
            {
                Ogrenci = ogrenci,
                Hareketler = hareketlerPaged,
                SinifYoklamalar = sinifYoklamalar,
                ServisYoklamalar = servisYoklamalar,
                RaporTipi = raporTipi
            };

            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["RaporTipi"] = ((int)raporTipi).ToString();

            return View("GirisCikisDetay", vm);
        }


        // Sınıf yoklaması ortak sorgusu (tarih aralığı + öğrenci)
        private IQueryable<SinifYoklamaModel> SinifYoklamaSorgusu(int ogrenciId, DateTime? startDate, DateTime? endDate)
        {
            var q = _context.SinifYoklamalar
                .AsNoTracking()
                .Include(x => x.Kullanici)
                .Where(x => x.OgrenciId == ogrenciId);

            if (startDate.HasValue)
            {
                var s = startDate.Value.Date;
                q = q.Where(x => x.OlusturulmaTarihi >= s);
            }
            if (endDate.HasValue)
            {
                var e = endDate.Value.Date.AddDays(1);
                q = q.Where(x => x.OlusturulmaTarihi < e);
            }
            return q;
        }

        // Servis yoklaması ortak sorgusu (tarih aralığı + öğrenci)
        private IQueryable<ServisYoklamaModel> ServisYoklamaSorgusu(int ogrenciId, DateTime? startDate, DateTime? endDate)
        {
            var q = _context.ServisYoklamalar
                .AsNoTracking()
                .Include(x => x.Kullanici)
                .Where(x => x.OgrenciId == ogrenciId);

            if (startDate.HasValue)
            {
                var s = startDate.Value.Date;
                q = q.Where(x => x.OlusturulmaTarihi >= s);
            }
            if (endDate.HasValue)
            {
                var e = endDate.Value.Date.AddDays(1);
                q = q.Where(x => x.OlusturulmaTarihi < e);
            }
            return q;
        }

        // Tek öğrenci için Excel: rapor tipine göre 1 veya 3 sayfa
        [HttpGet]
        public async Task<IActionResult> DetayExportToExcel(
            int id,
            string? searchName,
            DateTime? startDate,
            DateTime? endDate,
            RaporTipi raporTipi = RaporTipi.Tumu,
            CancellationToken ct = default)
        {
            var ogrenci = await _context.Ogrenciler
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OgrenciId == id, ct);
            if (ogrenci is null) return NotFound();

            IstasyonTipi? istasyonTipi = raporTipi switch
            {
                RaporTipi.AnaKapiGecisleri => IstasyonTipi.AnaKapi,
                RaporTipi.YemekhaneGecisleri => IstasyonTipi.Yemekhane,
                _ => null
            };

            bool gecisGoster = raporTipi == RaporTipi.Tumu
                            || raporTipi == RaporTipi.AnaKapiGecisleri
                            || raporTipi == RaporTipi.YemekhaneGecisleri;
            bool sinifGoster = raporTipi == RaporTipi.Tumu || raporTipi == RaporTipi.SinifYoklamasi;
            bool servisGoster = raporTipi == RaporTipi.Tumu || raporTipi == RaporTipi.ServisYoklamasi;

            // --- Geçişler ---
            List<OgrenciDetayModel> gecisler = new();
            if (gecisGoster)
            {
                var gq = _context.OgrenciDetaylar
                    .AsNoTracking()
                    .Include(d => d.Cihaz)
                    .Where(d => d.OgrenciId == id);

                if (startDate.HasValue)
                {
                    var s = startDate.Value.Date;
                    gq = gq.Where(d =>
                        (d.OgrenciGTarih.HasValue && d.OgrenciGTarih.Value >= s) ||
                        (d.OgrenciCTarih.HasValue && d.OgrenciCTarih.Value >= s));
                }
                if (endDate.HasValue)
                {
                    var e = endDate.Value.Date.AddDays(1);
                    gq = gq.Where(d =>
                        (d.OgrenciGTarih.HasValue && d.OgrenciGTarih.Value < e) ||
                        (d.OgrenciCTarih.HasValue && d.OgrenciCTarih.Value < e));
                }
                if (istasyonTipi.HasValue)
                {
                    gq = gq.Where(d => d.Cihaz != null && d.Cihaz.IstasyonTipi == istasyonTipi.Value);
                }

                gecisler = await gq
                    .OrderByDescending(d => d.OgrenciGTarih ?? d.OgrenciCTarih)
                    .ToListAsync(ct);
            }

            var sinifList = sinifGoster
                ? await SinifYoklamaSorgusu(id, startDate, endDate)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToListAsync(ct)
                : new List<SinifYoklamaModel>();

            var servisList = servisGoster
                ? await ServisYoklamaSorgusu(id, startDate, endDate)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToListAsync(ct)
                : new List<ServisYoklamaModel>();

            using var workbook = new XLWorkbook();

            // === Sayfa 1: Geçişler ===
            if (gecisGoster)
            {
            var ws1 = workbook.Worksheets.Add("Geçişler");
            ws1.Cell(1, 1).Value = "#";
            ws1.Cell(1, 2).Value = "Giriş Tarihi";
            ws1.Cell(1, 3).Value = "Çıkış Tarihi";
            ws1.Cell(1, 4).Value = "Geçiş Tipi";
            ws1.Cell(1, 5).Value = "Cihaz Adı";
            BasligiBicimle(ws1.Range(1, 1, 1, 5));

            int r = 2;
            foreach (var g in gecisler)
            {
                ws1.Cell(r, 1).Value = g.OgrenciDetayId;
                var c2 = ws1.Cell(r, 2);
                if (g.OgrenciGTarih.HasValue) { c2.Value = g.OgrenciGTarih.Value; c2.Style.DateFormat.Format = "dd.MM.yyyy HH:mm"; }
                else c2.Value = "-";
                var c3 = ws1.Cell(r, 3);
                if (g.OgrenciCTarih.HasValue) { c3.Value = g.OgrenciCTarih.Value; c3.Style.DateFormat.Format = "dd.MM.yyyy HH:mm"; }
                else c3.Value = "-";
                ws1.Cell(r, 4).Value = g.OgrenciGecisTipi ?? "-";
                ws1.Cell(r, 5).Value = g.Cihaz?.CihazAdi ?? "-";
                r++;
            }
            SayfayiSonlandir(ws1, 5, r, $"Toplam {gecisler.Count} kayıt");
            }

            // === Sayfa 2: Sınıf Yoklaması ===
            if (sinifGoster)
            {
            var ws2 = workbook.Worksheets.Add("Sınıf Yoklaması");
            ws2.Cell(1, 1).Value = "Tarih";
            for (int i = 1; i <= 8; i++) ws2.Cell(1, 1 + i).Value = $"Ders {i}";
            ws2.Cell(1, 10).Value = "Kaydeden";
            BasligiBicimle(ws2.Range(1, 1, 1, 10));

            int r = 2;
            foreach (var sy in sinifList)
            {
                var ct2 = ws2.Cell(r, 1);
                ct2.Value = sy.OlusturulmaTarihi;
                ct2.Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

                ws2.Cell(r, 2).Value = sy.Ders1?.ToString() ?? "-";
                ws2.Cell(r, 3).Value = sy.Ders2?.ToString() ?? "-";
                ws2.Cell(r, 4).Value = sy.Ders3?.ToString() ?? "-";
                ws2.Cell(r, 5).Value = sy.Ders4?.ToString() ?? "-";
                ws2.Cell(r, 6).Value = sy.Ders5?.ToString() ?? "-";
                ws2.Cell(r, 7).Value = sy.Ders6?.ToString() ?? "-";
                ws2.Cell(r, 8).Value = sy.Ders7?.ToString() ?? "-";
                ws2.Cell(r, 9).Value = sy.Ders8?.ToString() ?? "-";
                ws2.Cell(r, 10).Value = sy.Kullanici?.KullaniciAdi ?? "-";
                r++;
            }
            SayfayiSonlandir(ws2, 10, r, $"Toplam {sinifList.Count} kayıt");
            }

            // === Sayfa 3: Servis Yoklaması ===
            if (servisGoster)
            {
            var ws3 = workbook.Worksheets.Add("Servis Yoklaması");
            ws3.Cell(1, 1).Value = "Tarih";
            ws3.Cell(1, 2).Value = "Periyot";
            ws3.Cell(1, 3).Value = "Durum";
            ws3.Cell(1, 4).Value = "SMS";
            ws3.Cell(1, 5).Value = "Şoför";
            BasligiBicimle(ws3.Range(1, 1, 1, 5));

            int r = 2;
            foreach (var sv in servisList)
            {
                var ct3 = ws3.Cell(r, 1);
                ct3.Value = sv.OlusturulmaTarihi;
                ct3.Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

                ws3.Cell(r, 2).Value = sv.Periyot == 1 ? "Sabah" : (sv.Periyot == 2 ? "Akşam" : sv.Periyot.ToString());
                ws3.Cell(r, 3).Value = sv.DurumId == 1 ? "Bindi" : (sv.DurumId == 2 ? "Binmedi" : sv.DurumId.ToString());
                ws3.Cell(r, 4).Value = sv.SmsGonderildi ? "Evet" : "Hayır";
                ws3.Cell(r, 5).Value = sv.Kullanici?.KullaniciAdi ?? "-";
                r++;
            }
            SayfayiSonlandir(ws3, 5, r, $"Toplam {servisList.Count} kayıt");
            }

            // En az bir sheet olduğundan emin ol (boş workbook olmasın)
            if (workbook.Worksheets.Count == 0)
                workbook.Worksheets.Add("Bos");

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var raporAdi = raporTipi switch
            {
                RaporTipi.AnaKapiGecisleri => "AnaKapiGecisleri",
                RaporTipi.YemekhaneGecisleri => "YemekhaneGecisleri",
                RaporTipi.SinifYoklamasi => "SinifYoklamasi",
                RaporTipi.ServisYoklamasi => "ServisYoklamasi",
                _ => "OgrenciDetay"
            };
            var fileName = $"{raporAdi}_{ogrenci.OgrenciAdSoyad}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // Başlık satırı stili
        private static void BasligiBicimle(IXLRange headerRange)
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        // Sayfa sonu: filtre, donmuş satır, özet, sütun genişliği
        private static void SayfayiSonlandir(IXLWorksheet ws, int colCount, int nextRow, string ozet)
        {
            ws.SheetView.FreezeRows(1);
            ws.Range(1, 1, Math.Max(1, nextRow - 1), colCount).SetAutoFilter();

            int summaryRow = nextRow + 1;
            if (colCount > 1)
                ws.Range(summaryRow, 1, summaryRow, colCount - 1).Merge();
            var totalCell = ws.Cell(summaryRow, colCount);
            totalCell.Value = ozet;
            totalCell.Style.Font.Bold = true;
            totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            totalCell.Style.Border.TopBorder = XLBorderStyleValues.Thin;

            ws.Columns(1, colCount).AdjustToContents(1, summaryRow);
            foreach (var col in ws.Columns(1, colCount))
                if (col.Width < 12) col.Width = 12;
        }

        [HttpGet]
        public async Task<IActionResult> DetayExcel(
            string? searchString,
            DateTime? startDate,
            DateTime? endDate,
            RaporTipi raporTipi = RaporTipi.Tumu,
            CancellationToken ct = default)
        {
            using var workbook = new XLWorkbook();
            var searchName = searchString;

            // === SINIF YOKLAMASI ===
            if (raporTipi == RaporTipi.SinifYoklamasi)
            {
                var sq = _context.SinifYoklamalar
                    .AsNoTracking()
                    .Include(x => x.Ogrenci).ThenInclude(o => o.Birim)
                    .Include(x => x.Kullanici)
                    .AsQueryable();

                if (startDate.HasValue) { var s = startDate.Value.Date; sq = sq.Where(x => x.OlusturulmaTarihi >= s); }
                if (endDate.HasValue) { var e = endDate.Value.Date.AddDays(1); sq = sq.Where(x => x.OlusturulmaTarihi < e); }
                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    var s = searchName.Trim();
                    sq = sq.Where(x => x.Ogrenci != null && x.Ogrenci.OgrenciAdSoyad != null &&
                        EF.Functions.Like(x.Ogrenci.OgrenciAdSoyad, $"%{s}%"));
                }

                var liste = await sq.OrderByDescending(x => x.OlusturulmaTarihi).ToListAsync(ct);

                var ws2 = workbook.Worksheets.Add("Sınıf Yoklaması");
                ws2.Cell(1, 1).Value = "Ad Soyad";
                ws2.Cell(1, 2).Value = "Sınıf/Birim";
                ws2.Cell(1, 3).Value = "Tarih";
                for (int i = 1; i <= 8; i++) ws2.Cell(1, 3 + i).Value = $"Ders {i}";
                ws2.Cell(1, 12).Value = "Kaydeden";
                BasligiBicimle(ws2.Range(1, 1, 1, 12));

                int r = 2;
                foreach (var sy in liste)
                {
                    ws2.Cell(r, 1).Value = sy.Ogrenci?.OgrenciAdSoyad ?? "-";
                    ws2.Cell(r, 2).Value = sy.Ogrenci?.Birim?.BirimAd ?? "-";
                    var ct2 = ws2.Cell(r, 3);
                    ct2.Value = sy.OlusturulmaTarihi;
                    ct2.Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
                    ws2.Cell(r, 4).Value = sy.Ders1?.ToString() ?? "-";
                    ws2.Cell(r, 5).Value = sy.Ders2?.ToString() ?? "-";
                    ws2.Cell(r, 6).Value = sy.Ders3?.ToString() ?? "-";
                    ws2.Cell(r, 7).Value = sy.Ders4?.ToString() ?? "-";
                    ws2.Cell(r, 8).Value = sy.Ders5?.ToString() ?? "-";
                    ws2.Cell(r, 9).Value = sy.Ders6?.ToString() ?? "-";
                    ws2.Cell(r, 10).Value = sy.Ders7?.ToString() ?? "-";
                    ws2.Cell(r, 11).Value = sy.Ders8?.ToString() ?? "-";
                    ws2.Cell(r, 12).Value = sy.Kullanici?.KullaniciAdi ?? "-";
                    r++;
                }
                SayfayiSonlandir(ws2, 12, r, $"Toplam {liste.Count} kayıt");

                using var ms = new MemoryStream();
                workbook.SaveAs(ms);
                return File(ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"SinifYoklamasi_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }

            // === SERVİS YOKLAMASI ===
            if (raporTipi == RaporTipi.ServisYoklamasi)
            {
                var sq = _context.ServisYoklamalar
                    .AsNoTracking()
                    .Include(x => x.Ogrenci).ThenInclude(o => o.Birim)
                    .Include(x => x.Kullanici)
                    .AsQueryable();

                if (startDate.HasValue) { var s = startDate.Value.Date; sq = sq.Where(x => x.OlusturulmaTarihi >= s); }
                if (endDate.HasValue) { var e = endDate.Value.Date.AddDays(1); sq = sq.Where(x => x.OlusturulmaTarihi < e); }
                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    var s = searchName.Trim();
                    sq = sq.Where(x => x.Ogrenci != null && x.Ogrenci.OgrenciAdSoyad != null &&
                        EF.Functions.Like(x.Ogrenci.OgrenciAdSoyad, $"%{s}%"));
                }

                var liste = await sq.OrderByDescending(x => x.OlusturulmaTarihi).ToListAsync(ct);

                var ws3 = workbook.Worksheets.Add("Servis Yoklaması");
                ws3.Cell(1, 1).Value = "Ad Soyad";
                ws3.Cell(1, 2).Value = "Sınıf/Birim";
                ws3.Cell(1, 3).Value = "Tarih";
                ws3.Cell(1, 4).Value = "Periyot";
                ws3.Cell(1, 5).Value = "Durum";
                ws3.Cell(1, 6).Value = "SMS";
                ws3.Cell(1, 7).Value = "Şoför";
                BasligiBicimle(ws3.Range(1, 1, 1, 7));

                int r = 2;
                foreach (var sv in liste)
                {
                    ws3.Cell(r, 1).Value = sv.Ogrenci?.OgrenciAdSoyad ?? "-";
                    ws3.Cell(r, 2).Value = sv.Ogrenci?.Birim?.BirimAd ?? "-";
                    var ct3 = ws3.Cell(r, 3);
                    ct3.Value = sv.OlusturulmaTarihi;
                    ct3.Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
                    ws3.Cell(r, 4).Value = sv.Periyot == 1 ? "Sabah" : (sv.Periyot == 2 ? "Akşam" : sv.Periyot.ToString());
                    ws3.Cell(r, 5).Value = sv.DurumId == 1 ? "Bindi" : (sv.DurumId == 2 ? "Binmedi" : sv.DurumId.ToString());
                    ws3.Cell(r, 6).Value = sv.SmsGonderildi ? "Evet" : "Hayır";
                    ws3.Cell(r, 7).Value = sv.Kullanici?.KullaniciAdi ?? "-";
                    r++;
                }
                SayfayiSonlandir(ws3, 7, r, $"Toplam {liste.Count} kayıt");

                using var ms = new MemoryStream();
                workbook.SaveAs(ms);
                return File(ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ServisYoklamasi_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }

            // === GEÇİŞLER (Tumu / AnaKapı / Yemekhane) ===
            IstasyonTipi? istasyonTipi = raporTipi switch
            {
                RaporTipi.AnaKapiGecisleri => IstasyonTipi.AnaKapi,
                RaporTipi.YemekhaneGecisleri => IstasyonTipi.Yemekhane,
                _ => null
            };

            var loglar = _context.OgrenciDetaylar
                .Include(l => l.Ogrenci)
                    .ThenInclude(o => o.Birim)
                .Include(l => l.Cihaz)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var s = searchName.Trim();
                loglar = loglar.Where(l => l.Ogrenci != null &&
                                           (EF.Functions.Like(l.Ogrenci.OgrenciAdSoyad, $"%{s}%")
                                            || (l.Ogrenci.Birim != null &&
                                                EF.Functions.Like(l.Ogrenci.Birim.BirimAd, $"%{s}%"))));
            }

            if (istasyonTipi.HasValue)
            {
                loglar = loglar.Where(l => l.Cihaz != null &&
                                           l.Cihaz.IstasyonTipi == istasyonTipi.Value);
            }

            if (startDate.HasValue)
            {
                var s = startDate.Value.Date;
                loglar = loglar.Where(l =>
                    (l.OgrenciGTarih.HasValue && l.OgrenciGTarih.Value >= s) ||
                    (l.OgrenciCTarih.HasValue && l.OgrenciCTarih.Value >= s));
            }

            if (endDate.HasValue)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);
                loglar = loglar.Where(l =>
                    (l.OgrenciGTarih.HasValue && l.OgrenciGTarih.Value < endExclusive) ||
                    (l.OgrenciCTarih.HasValue && l.OgrenciCTarih.Value < endExclusive));
            }

            var filteredLogs = loglar
                .OrderByDescending(l => l.OgrenciCTarih ?? l.OgrenciGTarih)
                .ToList();

            var ws = workbook.Worksheets.Add("Öğrenci Giriş-Çıkış");

            // Başlıklar (8 sütun)
            ws.Cell(1, 1).Value = "#";
            ws.Cell(1, 2).Value = "Ad Soyad";
            ws.Cell(1, 3).Value = "Sınıf/Birim";
            ws.Cell(1, 4).Value = "Kart No";
            ws.Cell(1, 5).Value = "Giriş Tarihi";
            ws.Cell(1, 6).Value = "Çıkış Tarihi";
            ws.Cell(1, 7).Value = "Geçiş Tipi";
            ws.Cell(1, 8).Value = "Cihaz Adı";

            // Başlık stili
            var headerRange = ws.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Veriler
            int row = 2;
            foreach (var log in filteredLogs)
            {
                ws.Cell(row, 1).Value = log.OgrenciDetayId;
                ws.Cell(row, 2).Value = log.Ogrenci?.OgrenciAdSoyad ?? "-";
                ws.Cell(row, 3).Value = log.Ogrenci?.Birim?.BirimAd ?? "-";
                ws.Cell(row, 4).Value = log.Ogrenci?.OgrenciKartNo ?? "-";

                var cGiris = ws.Cell(row, 5);
                if (log.OgrenciGTarih.HasValue)
                {
                    cGiris.Value = log.OgrenciGTarih.Value;
                    cGiris.Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
                }
                else cGiris.Value = "-";

                var cCikis = ws.Cell(row, 6);
                if (log.OgrenciCTarih.HasValue)
                {
                    cCikis.Value = log.OgrenciCTarih.Value;
                    cCikis.Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
                }
                else cCikis.Value = "-";

                ws.Cell(row, 7).Value = log.OgrenciGecisTipi.ToString();
                ws.Cell(row, 8).Value = log.Cihaz?.CihazAdi ?? "-";
                row++;
            }

            // Üst başlığı sabitle
            ws.SheetView.FreezeRows(1);

            // Filtre ekle
            ws.Range(1, 1, Math.Max(1, row - 1), 8).SetAutoFilter();

            // Alt toplam satırı
            int summaryRow = row + 1;
            ws.Range(summaryRow, 1, summaryRow, 7).Merge(); // ilk 7 sütun birleşik kalır
            var totalCell = ws.Cell(summaryRow, 8);         // toplam bilgi 8. sütunda
            totalCell.Value = $"Toplam {filteredLogs.Count} kayıt";
            totalCell.Style.Font.Bold = true;
            totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            totalCell.Style.Border.TopBorder = XLBorderStyleValues.Thin;

            // Otomatik sütun genişliği
            ws.Columns(1, 8).AdjustToContents(1, summaryRow);
            foreach (var col in ws.Columns(1, 8))
                if (col.Width < 15) col.Width = 15;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "OgrenciGirisCikis.xlsx");
        }
    }
}