using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Services.Interfaces
{
    public interface IOgretmenMusaitlikService
    {
        Task<List<OgretmenMusaitlikModel>> OgretmeneGoreListele(int ogretmenKullaniciId, CancellationToken ct = default);
        Task Ekle(OgretmenMusaitlikModel model, CancellationToken ct = default);
        Task Sil(int musaitlikId, CancellationToken ct = default);
    }
}
