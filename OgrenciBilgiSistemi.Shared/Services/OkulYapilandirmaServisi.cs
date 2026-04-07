using Microsoft.Extensions.Options;
using OgrenciBilgiSistemi.Shared.Models;

namespace OgrenciBilgiSistemi.Shared.Services
{
    /// <summary>
    /// appsettings.json'daki Okullar bölümünü okuyarak okul bilgilerine erişim sağlar.
    /// Singleton olarak kayıt edilmelidir.
    /// </summary>
    public class OkulYapilandirmaServisi
    {
        private readonly Dictionary<string, OkulBilgiAyari> _okullar;

        public OkulYapilandirmaServisi(IOptions<List<OkulBilgiAyari>> options)
        {
            _okullar = options.Value.ToDictionary(
                o => o.OkulKodu,
                StringComparer.OrdinalIgnoreCase);
        }

        public OkulBilgiAyari? OkulGetir(string okulKodu)
            => _okullar.GetValueOrDefault(okulKodu);

        public List<OkulBilgiAyari> TumOkullariGetir()
            => _okullar.Values.ToList();

        public bool OkulVarMi(string okulKodu)
            => _okullar.ContainsKey(okulKodu);
    }
}
