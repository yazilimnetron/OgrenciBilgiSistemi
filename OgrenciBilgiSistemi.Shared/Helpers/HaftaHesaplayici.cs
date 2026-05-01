namespace OgrenciBilgiSistemi.Shared.Helpers;

public static class HaftaHesaplayici
{
    public static DateTime PazartesiBul(DateTime tarih)
    {
        var gunFarki = (7 + (tarih.DayOfWeek - DayOfWeek.Monday)) % 7;
        return tarih.AddDays(-gunFarki).Date;
    }

    public static (DateTime Baslangic, DateTime Bitis) HaftaAraligi(DateTime tarih)
    {
        var pzt = PazartesiBul(tarih);
        return (pzt, pzt.AddDays(6));
    }
}
