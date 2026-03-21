namespace OgrenciBilgiSistemi.Shared.Enums
{
    public enum DonanimTipi : byte
    {
        UsbRfid = 1,
        ZKTeco = 2,
        QrOkuyucu = 3,
        Diger = 9
    }

    public enum IstasyonTipi : short
    {
        Bilinmiyor = 0,
        AnaKapi = 10,
        Yemekhane = 20,
    }
}
