namespace OgrenciBilgiSistemi.Shared.Dtos;

public class SinifYoklamaDto
{
    public int SinifYoklamaId { get; set; }
    public int OgrenciId { get; set; }
    public int KullaniciId { get; set; }
    public int? Ders1 { get; set; }
    public int? Ders2 { get; set; }
    public int? Ders3 { get; set; }
    public int? Ders4 { get; set; }
    public int? Ders5 { get; set; }
    public int? Ders6 { get; set; }
    public int? Ders7 { get; set; }
    public int? Ders8 { get; set; }
    public DateTime OlusturulmaTarihi { get; set; }
    public DateTime? GuncellenmeTarihi { get; set; }

    public int? DersGetir(int dersNumarasi) => dersNumarasi switch
    {
        1 => Ders1,
        2 => Ders2,
        3 => Ders3,
        4 => Ders4,
        5 => Ders5,
        6 => Ders6,
        7 => Ders7,
        8 => Ders8,
        _ => null
    };
}
