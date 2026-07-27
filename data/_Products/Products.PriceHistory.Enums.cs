namespace data._Products
{
    // ============================================================================================
    // FİYAT GEÇMİŞİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // ============================================================================================

    /// <summary>
    /// Fiyat değişiminin kaynağı (ProductPriceHistory.ChangeSource).
    ///
    /// TASARIM KARARI — neden mevcut ContentSource GENİŞLETİLMEDİ:
    /// ContentSource, metin/içerik üretiminin kökenini (Manual/Ai/Import...) anlatan bir kavramdır ve
    /// AttributeDefinition/ProductTranslations gibi İÇERİK tablolarında kullanılır. Fiyat değişiminin
    /// kaynağı ise FİNANSAL bir olay tipidir (kur dönüşümü, kampanya motoru, toplu güncelleme).
    /// Ortak enum kullanmak: (a) ContentSource'a fiyata özgü değerler ekleyip içerik tarafında anlamsız
    /// seçenekler doğurur, (b) "araya değer eklenmez" kuralı yüzünden iki alanın değer uzayını kalıcı
    /// olarak birbirine kilitler, (c) ileride kampanya/kur kaynaklarını genişletmeyi zorlaştırır.
    /// Bu nedenle AYRI ve dar kapsamlı bir enum tanımlanmıştır.
    /// </summary>
    public enum PriceChangeSource : byte
    {
        /// <summary>Bilinmiyor / eski kayıt (geriye dönük dolgu satırları).</summary>
        Unknown = 0,

        /// <summary>Admin veya satıcı panelinden elle yapılan değişiklik.</summary>
        Manual = 1,

        /// <summary>Kur dönüşümü ile otomatik hesaplanan fiyat (ProductPrices.IsAutoConverted=true satırları;
        /// MoneyExchangeRate güncellemesini takip eden BackgroundService yazar).</summary>
        AutoConversion = 2,

        /// <summary>Toplu içe aktarım (data._BulkImportProducts akışı; ImportRowId iz alanına yazılır).</summary>
        Import = 3,

        /// <summary>Kampanya motoru (Faz 6 Campaigns) tarafından yazılan indirimli fiyat.</summary>
        Campaign = 4,

        /// <summary>Kampanya bitişinde indirimin geri alınması (DiscountedPrice temizlenmesi).</summary>
        CampaignExpired = 5,

        /// <summary>Pazaryeri/entegrasyon kaynaklı fiyat senkronu (IntegrationPlatform üzerinden gelen dış fiyat).</summary>
        IntegrationSync = 6,

        /// <summary>Fiyat satırının aktiflik durumu değişti (IsActive true/false) — tutar değişmemiş olabilir.</summary>
        ActivationChange = 7,

        /// <summary>Fiyat satırı ilk kez oluşturuldu (baz kayıt; eski değerler null'dır).</summary>
        InitialCreate = 8
    }

    /// <summary>
    /// Fiyat alarmı durumu (PriceAlerts.Status).
    /// </summary>
    public enum PriceAlertStatus : byte
    {
        /// <summary>Aktif; hedef fiyat izleniyor.</summary>
        Active = 0,

        /// <summary>Hedef fiyata ulaşıldı, bildirim üretildi (Faz 6 Notifications).</summary>
        Triggered = 1,

        /// <summary>Kullanıcı alarmı iptal etti.</summary>
        CancelledByUser = 2,

        /// <summary>Ürün/varyant satıştan kalktı veya silindi; alarm otomatik kapatıldı.</summary>
        Expired = 3
    }
}
