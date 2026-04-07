namespace OgrenciBilgiSistemi.Shared.Services
{
    /// <summary>
    /// Scoped servis: mevcut HTTP isteğinin hangi okula ait olduğunu tutar.
    /// Middleware tarafından doldurulur, servisler tarafından okunur.
    /// </summary>
    public class TenantBaglami
    {
        public string OkulKodu { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string OkulAdi { get; set; } = string.Empty;
    }
}
