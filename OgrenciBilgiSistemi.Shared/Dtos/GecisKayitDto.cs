using OgrenciBilgiSistemi.Shared.Enums;

namespace OgrenciBilgiSistemi.Shared.Dtos;

public class GecisKayitDto
{
    public int OgrenciDetayId { get; set; }
    public int OgrenciId { get; set; }
    public string OgrenciAdSoyad { get; set; } = string.Empty;
    public string? OgrenciKartNo { get; set; }
    public string? BirimAd { get; set; }
    public DateTime? OgrenciGTarih { get; set; }
    public DateTime? OgrenciCTarih { get; set; }
    public string? OgrenciGecisTipi { get; set; }
    public IstasyonTipi IstasyonTipi { get; set; }
    public string? CihazAdi { get; set; }
}
