using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ana ürün tablosu — İÇERİKSİZ ÇEKİRDEK.
    /// REVİZYON: Dile bağlı tüm içerik alanları (Name, ShortDescription, FullDescription, Tags,
    /// AiOriginal*) ProductTranslations tablosuna taşındı. Products yalnızca dil-bağımsız veriyi tutar.
    ///
    /// OKUMA KURALI: İçerik DAİMA ProductTranslations üzerinden okunur.
    /// KAYNAK DİL KURALI: İnsan girişi / içe aktarılan orijinal içerik SourceLanguageCode
    /// dilindeki çeviri satırındadır. Türkçe girişte "tr", Amazon.com aktarımında "en",
    /// Mercado Libre aktarımında "es" olabilir. Diğer diller AI ile bu kaynaktan üretilir.
    ///
    /// Tüm ürün tipleri (basit, varyantlı, dijital, hizmet, paket, harici) bu tablo üzerinden
    /// yönetilir. Basit ürünler dahi en az bir ProductVariants kaydına sahiptir
    /// (birleşik tek-varyant modeli) — fiyat/stok her zaman varyant üzerinden okunur.
    /// </summary>
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === İlişkiler (ID bazlı, navigation property kullanılmaz) ===
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? AttributeTemplateId { get; set; }
        public Guid? ShippingProfileId { get; set; }

        // === Ürün Tipi ===
        // "Simple", "Variable", "Digital", "Service", "Bundle", "External"
        public string? ProductType { get; set; } = "Simple";

        // === Kaynak Dil ===
        // Ürünün orijinal (insan girişi veya içe aktarılan) içeriğinin dili.
        // İçerik gerçeği bu dilin ProductTranslations satırındadır; AI çevirileri buradan üretilir.
        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string SourceLanguageCode { get; set; } = "tr";

        // === Durum / Menşe (Marketplace zorunlulukları) ===
        // "New", "Refurbished", "UsedLikeNew", "UsedVeryGood", "UsedGood", "UsedAcceptable"
        // eBay/Amazon ikinci el ve yenilenmiş ürün aktarımlarında veri kaybını önler.
        public string? Condition { get; set; } = "New";
        public string? ConditionNote { get; set; } // Durum açıklaması (Örn: "Kutusu hasarlı, ürün sıfır")

        public string? CountryOfOriginCode { get; set; } // Menşe ülke (ISO 3166-1 alpha-2, Örn: "TR","CN") — EU GPSR ve gümrük zorunluluğu
        public string? HsCode { get; set; } // GTIP / Harmonize Sistem kodu — sınır ötesi satış ve gümrük beyanı için

        public bool IsAdultProduct { get; set; } = false; // Yetişkin ürünü işareti (Google/Amazon feed reddini önler)

        // === Medya ===
        public Guid? CoverMediaId { get; set; }

        // === Harici Ürün (External) Alanları ===
        // Buton metni dile bağlı olduğu için ProductTranslations.ExternalButtonText'e taşındı.
        public string? ExternalUrl { get; set; }

        // === Yayın Durumu ===
        public bool? PublishStatus { get; set; }
        public bool? IsApprovedByAdmin { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // === AI İçerik Yönetimi ===
        // Satıcı "AI Yönetimine İzin Ver" toggle'ını açarsa true olur.
        // İşlem durumu/hata/orijinal yedek alanları artık satır bazında ProductTranslations'tadır.
        public bool? IsAiManaged { get; set; } = false;

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? InteractionCounts { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
