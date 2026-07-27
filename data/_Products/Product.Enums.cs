using System;

namespace data._Products
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Manuel Ürün Yönetim Sistemi (Product System V1)
    //  Enum kataloğu — dilden bağımsız, tinyint (byte) olarak saklanan sabitler.
    //  Not: Enum değerlerine ASLA arada değer eklemeyin; yalnızca SONA ekleyin.
    //       Aksi hâlde veritabanındaki mevcut kayıtların anlamı değişir.
    //  Dil enum'ı için mevcut data._Attribute.Language (10 dil) KULLANILIR,
    //  yeniden tanımlanmaz.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Desteklenen 4 para birimi. <see cref="ProductPrices"/> tablosunda saklanır.
    /// Kur çevrimi için mevcut <see cref="MoneyExchangeRate"/> tablosu kullanılır.
    /// </summary>
    public enum CurrencyCode : byte
    {
        /// <summary>ABD Doları.</summary>
        Usd = 1,
        /// <summary>Türk Lirası.</summary>
        Try = 2,
        /// <summary>Euro.</summary>
        Eur = 3,
        /// <summary>Azerbaycan Manatı.</summary>
        Azn = 4
    }

    /// <summary>
    /// Ürünün moderasyon / yaşam döngüsü durumu. Yayın görünürlüğü bundan AYRIDIR:
    /// bir ürün Approved olsa bile zamanlanmış yayın alanları
    /// (PublishStartDate/PublishEndDate) izin vermiyorsa vitrinde görünmez.
    /// </summary>
    public enum ProductStatus : byte
    {
        /// <summary>Taslak — satıcı hâlâ dolduruyor.</summary>
        Draft = 1,
        /// <summary>Satıcı gönderdi, admin onayı bekliyor.</summary>
        PendingReview = 2,
        /// <summary>Admin onayladı — yayınlanabilir.</summary>
        Approved = 3,
        /// <summary>Admin reddetti (gerekçe: RejectionReason).</summary>
        Rejected = 4,
        /// <summary>Satıcı/Admin tarafından pasife alındı.</summary>
        Suspended = 5,
        /// <summary>Arşivlendi (satıştan kalktı, kayıt korunuyor).</summary>
        Archived = 6
    }

    /// <summary>Ürünün fiziksel durumu (Amazon "Condition" karşılığı).</summary>
    public enum ProductCondition : byte
    {
        /// <summary>Sıfır / yeni.</summary>
        New = 1,
        /// <summary>Kullanılmış.</summary>
        Used = 2,
        /// <summary>Yenilenmiş (refurbished).</summary>
        Refurbished = 3,
        /// <summary>Kutusu açılmış (open box).</summary>
        OpenBox = 4
    }

    /// <summary>
    /// Çevirinin kaynağı / üretim yöntemi.
    /// Yapay zekâ çeviri entegrasyonu (hizmet bedeli karşılığı otomatik çeviri)
    /// bu alana göre hangi satırların AI tarafından üretildiğini bilir.
    /// </summary>
    public enum TranslationSource : byte
    {
        /// <summary>Satıcı/Admin tarafından elle girildi.</summary>
        Manual = 1,
        /// <summary>Yapay zekâ tarafından üretildi.</summary>
        AIGenerated = 2
    }

    /// <summary>
    /// Çeviri satırının yaşam döngüsü durumu. AI çeviri BackgroundService'i
    /// Pending kayıtları çeker, işler ve Completed / NeedsReview yapar.
    /// </summary>
    public enum TranslationStatus : byte
    {
        /// <summary>Çeviri bekliyor (AI kuyruğunda).</summary>
        Pending = 1,
        /// <summary>Çeviri tamamlandı, kullanılabilir.</summary>
        Completed = 2,
        /// <summary>Çeviri üretildi ama insan incelemesi gerekiyor.</summary>
        NeedsReview = 3,
        /// <summary>Şu anda işleniyor (worker kilidi).</summary>
        InProgress = 4,
        /// <summary>Çeviri denemesi hata ile sonuçlandı (ErrorMessage dolu).</summary>
        Failed = 5
    }

    /// <summary>Ürün medya bağlantısının türü.</summary>
    public enum ProductMediaType : byte
    {
        /// <summary>Görsel (Media tablosundaki dosya).</summary>
        Image = 1,
        /// <summary>Sunucuda barındırılan video (Media tablosundaki dosya).</summary>
        Video = 2,
        /// <summary>Dış video (YouTube/Vimeo embed URL'i — ExternalUrl dolu).</summary>
        VideoEmbed = 3,
        /// <summary>3B model dosyası (ör. .glb — 360° görüntüleme).</summary>
        Model3D = 4,
        /// <summary>Doküman (kullanım kılavuzu PDF vb.).</summary>
        Document = 5
    }

    /// <summary>Stok bittiğinde sipariş alma politikası (varyant bazında).</summary>
    public enum BackorderPolicy : byte
    {
        /// <summary>Stok yoksa satılamaz.</summary>
        Deny = 1,
        /// <summary>Stok yokken de sipariş alınabilir (ön sipariş).</summary>
        Allow = 2,
        /// <summary>Stok yokken sipariş alınabilir; müşteri bilgilendirilir.</summary>
        AllowNotify = 3
    }

    /// <summary>Ağırlık birimi (fiziksel/kargo alanları için).</summary>
    public enum WeightUnit : byte
    {
        /// <summary>Kilogram.</summary>
        Kg = 1,
        /// <summary>Gram.</summary>
        G = 2,
        /// <summary>Libre (pound).</summary>
        Lb = 3
    }

    /// <summary>Uzunluk birimi (boyut alanları için).</summary>
    public enum LengthUnit : byte
    {
        /// <summary>Santimetre.</summary>
        Cm = 1,
        /// <summary>Milimetre.</summary>
        Mm = 2,
        /// <summary>İnç.</summary>
        In = 3
    }
}
