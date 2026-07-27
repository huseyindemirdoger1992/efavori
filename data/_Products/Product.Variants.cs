using System;

namespace data._Products
{
    /// <summary>
    /// SATILABİLİR BİRİM (SKU) — sepete eklenen, stoğu tutulan, barkodu okutulan
    /// gerçek kayıt budur. Sınırsız kombinasyon desteklenir: varyantın hangi
    /// eksen değerlerinden (Renk=Siyah, Beden=M, Kapasite=128GB ...) oluştuğu
    /// <see cref="ProductVariantAttributeValues"/> satırlarıyla tanımlanır —
    /// eksen sayısında üst sınır YOKTUR.
    ///
    /// Her varyantın KENDİ stoğu (StockQuantity), KENDİ barkodu (Barcode/Gtin),
    /// KENDİ fiyatı (ProductPrices.VariantId üzerinden, 4 para birimi) ve
    /// KENDİ görselleri (ProductMedia.VariantId üzerinden) olabilir.
    ///
    /// Varyantsız "basit" üründe bile tek bir IsDefault = true varyant oluşturulur.
    /// </summary>
    public class ProductVariants : ProductEntityBase
    {
        /// <summary>Bağlı ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        // ── Kimlik ────────────────────────────────────────────────────────────
        /// <summary>Stok kodu (SKU) — sistem genelinde TEKİL (soft-delete hariç).</summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>Barkod (raf/kasa barkodu — EAN-13, Code128 vb. serbest format).</summary>
        public string? Barcode { get; set; }

        /// <summary>Bu varyantın GTIN'i (EAN/UPC/ISBN — Merchant feed'de "gtin").</summary>
        public string? Gtin { get; set; }

        /// <summary>Bu varyantın üretici parça numarası (MPN — üründen farklıysa).</summary>
        public string? Mpn { get; set; }

        // ── Stok ──────────────────────────────────────────────────────────────
        /// <summary>Stok takibi yapılsın mı? (false = sınırsız satılabilir; dijital ürünler vb.).</summary>
        public bool TrackStock { get; set; } = true;

        /// <summary>Eldeki toplam stok adedi.</summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Ödemesi süren/onay bekleyen siparişlerce rezerve edilmiş adet.
        /// Satılabilir stok = StockQuantity - ReservedQuantity (sorgu tarafında hesaplanır).
        /// </summary>
        public int ReservedQuantity { get; set; }

        /// <summary>Kritik stok eşiği — altına düşünce satıcıya uyarı üretilir (null = uyarı yok).</summary>
        public int? StockAlertThreshold { get; set; }

        /// <summary>Stok bittiğindeki sipariş politikası.</summary>
        public BackorderPolicy Backorder { get; set; } = BackorderPolicy.Deny;

        // ── Fiziksel override'lar (null = Products'taki varsayılan geçerli) ──
        /// <summary>Bu varyantın paketli ağırlığı (null = ürün varsayılanı).</summary>
        public decimal? WeightValue { get; set; }

        /// <summary>Bu varyantın paket uzunluğu (null = ürün varsayılanı).</summary>
        public decimal? LengthValue { get; set; }

        /// <summary>Bu varyantın paket genişliği (null = ürün varsayılanı).</summary>
        public decimal? WidthValue { get; set; }

        /// <summary>Bu varyantın paket yüksekliği (null = ürün varsayılanı).</summary>
        public decimal? HeightValue { get; set; }

        /// <summary>Bu varyantın desisi (null = ürün varsayılanı).</summary>
        public decimal? Desi { get; set; }

        /// <summary>Bu varyantın fiziksel durumu (null = ürün varsayılanı; ör. outlet varyant = OpenBox).</summary>
        public ProductCondition? ConditionOverride { get; set; }

        // ── Görünüm ───────────────────────────────────────────────────────────
        /// <summary>Ürün sayfasında varsayılan seçili varyant mı? (ürün başına bir adet true).</summary>
        public bool IsDefault { get; set; }

        /// <summary>Varyant seçicideki sıralama.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Varyant satışa açık mı? (false = geçici olarak satılamaz, kombinasyon korunur).</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Varyantın EKSEN DEĞERİ — bu varyantın hangi kombinasyondan oluştuğunu
    /// tanımlar. Ör. "Siyah / M / 128GB" varyantı için 3 satır:
    ///   (Renk → Siyah), (Beden → M), (Kapasite → 128GB).
    ///
    /// Eksenler Attribute System V3'e ID ile bağlanır: AttributeDefinitionId
    /// varyant üreten attribute (IsVariant), AttributeOptionId ise seçilen kanonik
    /// option'dır — böylece varyant değerleri de 10 dilde otomatik çevrilir
    /// (AttributeOptionTranslations) ve filtrelerle aynı sözlüğü paylaşır.
    ///
    /// (ProductVariantId, AttributeDefinitionId) benzersizdir — bir varyantta aynı
    /// eksen iki kez yer alamaz. Ürün içinde aynı option kombinasyonunun tekilliği
    /// uygulama katmanında (kayıt öncesi kombinasyon hash kontrolü) doğrulanır.
    /// </summary>
    public class ProductVariantAttributeValues : ProductEntityBase
    {
        /// <summary>Bağlı varyant (ProductVariants.Id).</summary>
        public Guid ProductVariantId { get; set; }

        /// <summary>Denormalize ürün referansı (Products.Id) — ürün bazlı sorguları hızlandırır.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Eksen attribute'ı (AttributeDefinition.Id; ör. Renk, Beden).</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Seçilen kanonik değer (AttributeOption.Id; ör. Siyah, M).</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Eksenin varyant seçicideki sıralaması (Renk önce, Beden sonra gibi).</summary>
        public int DisplayOrder { get; set; }
    }
}
