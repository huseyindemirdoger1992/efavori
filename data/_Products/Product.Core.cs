using System;

namespace data._Products
{
    /// <summary>
    /// ANA ÜRÜN kaydı — dilden ve para biriminden bağımsız çekirdek.
    ///
    /// Sorumluluk ayrımı (Amazon/eBay ölçeği için):
    ///  • Çevrilebilir TÜM metinler (ad, açıklama, SEO)  → <see cref="ProductTranslations"/>
    ///  • Fiyatlar (4 para birimi)                        → <see cref="ProductPrices"/>
    ///  • Satılabilir birimler (SKU/stok/barkod)          → <see cref="ProductVariants"/>
    ///  • Teknik özellikler (Attribute System V3 değeri)  → <see cref="ProductAttributeValues"/>
    ///  • Medya (görsel/video/3B)                         → <see cref="ProductMedia"/>
    ///  • Kategori atamaları (çoklu)                      → <see cref="ProductCategoryLinks"/>
    ///
    /// STOK KURALI: Stok her zaman VARYANT üzerindedir. Varyantsız "basit" ürünlerde
    /// bile sistem tek bir varsayılan varyant (IsDefault = true) oluşturur; böylece
    /// stok/fiyat/barkod tek modelden yönetilir ve ileride varyant eklemek kırılmaz.
    /// </summary>
    public class Products : ProductEntityBase
    {
        // ── Sahiplik ──────────────────────────────────────────────────────────
        /// <summary>Ürünün ait olduğu mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Ürünü giren satıcı kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        // ── Kimlik / sınıflandırma ────────────────────────────────────────────
        /// <summary>Marka (Brands.Id — null olabilir: markasız ürün).</summary>
        public Guid? BrandId { get; set; }

        /// <summary>
        /// Birincil kategori (CategoriesProduct.Id). Breadcrumb, SEO ve attribute
        /// çözümlemesi bu kategoriden yapılır. Ek kategoriler
        /// <see cref="ProductCategoryLinks"/> ile atanır (birincil orada da işaretlidir;
        /// bu alan hızlı erişim için denormalize tutulur).
        /// </summary>
        public Guid PrimaryCategoryId { get; set; }

        /// <summary>Satıcının kendi iç ürün kodu / model kodu (ör. "TSH-2024-A1").</summary>
        public string? ModelCode { get; set; }

        /// <summary>Üretici parça numarası (MPN — Manufacturer Part Number).</summary>
        public string? Mpn { get; set; }

        /// <summary>
        /// Ürün ailesinin GTIN'i (EAN/UPC/ISBN). Varyant bazlı barkodlar
        /// <see cref="ProductVariants"/> üzerindedir; bu alan varyantsız senaryonun
        /// ve Google Merchant "gtin" alanının ürün seviyesindeki karşılığıdır.
        /// </summary>
        public string? Gtin { get; set; }

        /// <summary>Fiziksel durum (Yeni/Kullanılmış/Yenilenmiş/Kutusu açık).</summary>
        public ProductCondition Condition { get; set; } = ProductCondition.New;

        /// <summary>Menşei ülke (ISO 3166-1 alpha-2, ör. "TR"). Gümrük/Merchant feed için.</summary>
        public string? CountryOfOriginCode { get; set; }

        /// <summary>GTİP / HS kodu (uluslararası gönderim ve gümrük beyanı için).</summary>
        public string? HsCode { get; set; }

        /// <summary>Garanti süresi (ay). Null = garanti bilgisi yok.</summary>
        public int? WarrantyMonths { get; set; }

        // ── Durum + ZAMANLANMIŞ YAYINLAMA (Publish Scheduling) ────────────────
        /// <summary>Moderasyon / yaşam döngüsü durumu.</summary>
        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        /// <summary>Reddedilme gerekçesi (Status = Rejected iken dolu).</summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Yayın anahtarı. Vitrinde görünme koşulu (BackgroundService/sorgu tarafında):
        /// Status == Approved && IsPublished == true
        /// && (PublishStartDate == null || PublishStartDate &lt;= UtcNow)
        /// && (PublishEndDate   == null || PublishEndDate   &gt;  UtcNow)
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Zamanlanmış yayın başlangıcı (UTC). Null = hemen. Gelecek bir tarih
        /// verilirse ürün o tarihte otomatik yayına girer.
        /// </summary>
        public DateTime? PublishStartDate { get; set; }

        /// <summary>
        /// Zamanlanmış yayın bitişi (UTC). Null = süresiz. Tarih geldiğinde ürün
        /// otomatik yayından kalkar (ör. sezonluk/kampanya ürünleri).
        /// </summary>
        public DateTime? PublishEndDate { get; set; }

        /// <summary>İlk kez yayına girdiği an (UTC) — "Yeni ürün" rozetleri/sıralama için.</summary>
        public DateTime? FirstPublishedAtUtc { get; set; }

        // ── Satış kuralları ───────────────────────────────────────────────────
        /// <summary>KDV oranı (yüzde, ör. 20.00). Fiyatlar KDV dâhil girilir; oran fatura için tutulur.</summary>
        public decimal VatRate { get; set; }

        /// <summary>Bir siparişte alınabilecek minimum adet.</summary>
        public int MinOrderQuantity { get; set; } = 1;

        /// <summary>Bir siparişte alınabilecek maksimum adet (null = sınırsız).</summary>
        public int? MaxOrderQuantity { get; set; }

        /// <summary>Adet artış katı (ör. 6'lı koli → 6). Varsayılan 1.</summary>
        public int QuantityStep { get; set; } = 1;

        // ── Varsayılan fiziksel / kargo bilgileri ─────────────────────────────
        //  Varyantlar bu değerleri override edebilir (ProductVariants'taki aynı
        //  adlı nullable alanlar; null = üründeki varsayılan geçerli).
        /// <summary>Paketli ağırlık değeri.</summary>
        public decimal? WeightValue { get; set; }

        /// <summary>Ağırlık birimi.</summary>
        public WeightUnit WeightUnitType { get; set; } = WeightUnit.Kg;

        /// <summary>Paket uzunluğu (derinlik).</summary>
        public decimal? LengthValue { get; set; }

        /// <summary>Paket genişliği.</summary>
        public decimal? WidthValue { get; set; }

        /// <summary>Paket yüksekliği.</summary>
        public decimal? HeightValue { get; set; }

        /// <summary>Boyut birimi (uzunluk/genişlik/yükseklik için ortak).</summary>
        public LengthUnit DimensionUnitType { get; set; } = LengthUnit.Cm;

        /// <summary>Desi (hacimsel ağırlık — kargo fiyatlandırması için).</summary>
        public decimal? Desi { get; set; }

        /// <summary>Kargo ücretsiz mi? (kampanya değil, ürün kuralı).</summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>Kargoya veriliş süresi — minimum iş günü.</summary>
        public int? ShippingPreparationDayMin { get; set; }

        /// <summary>Kargoya veriliş süresi — maksimum iş günü.</summary>
        public int? ShippingPreparationDayMax { get; set; }

        /// <summary>Gönderimin yapıldığı depo (WareHouse.Id — null = mağaza adresi).</summary>
        public Guid? ShipsFromWarehouseId { get; set; }

        /// <summary>Ayrı paketle gönderilmesi zorunlu mu? (büyük/hassas ürünler).</summary>
        public bool RequiresSeparateShipping { get; set; }

        // ── Varyant düzeni ────────────────────────────────────────────────────
        /// <summary>Ürünün birden çok varyantı var mı? (false = tek varsayılan varyant).</summary>
        public bool HasVariants { get; set; }

        /// <summary>
        /// Ürün sayfası ilk açıldığında seçili gelecek varyant
        /// (ProductVariants.Id — null = IsDefault işaretli varyant kullanılır).
        /// </summary>
        public Guid? DefaultVariantId { get; set; }
    }
}
