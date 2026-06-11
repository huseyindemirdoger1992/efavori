using System.ComponentModel.DataAnnotations;

namespace data
{
    public class CategoriesTr
    {
        [Key]
        public int Id { get; set; } // Veritabanının oluşturacağı otomatik artan ID
        public string? Name { get; set; } // Kategori adı (Örn: Korkuluklar)

        public string? DepartmentName { get; set; } // En üst grup adı (Örn: Antikalar)

        public int? ExternalId { get; set; } // CSV'den gelen KategoriDeğeri (Örn: 162925)

        public int? ParentCategoryId { get; set; } // Varsa üst kategorinin Id'si, yoksa null
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    public class ItemGallery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemType { get; set; } // InShooting, OutShooting, Document
        public Guid? MediaId { get; set; }
        public DateTime? ItemAddDate { get; set; }
        public bool? IsDelete { get; set; }


    }
}
using data._Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    /// <summary>
    /// Medya dosyalarının meta verilerini ve depolama bilgilerini tutan sınıf.
    /// </summary>
    public class Media
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public string? FileName { get; set; }

        public string? FileStoredName { get; set; }

        public string? FileUrl { get; set; }

        public string? FilePhysicalPathRoad { get; set; }

        public string? OrjFileUrl { get; set; }

        public string? OrjFilePhysicalPathRoad { get; set; }

        public string? FileExtensionType { get; set; }

        public string? ContentType { get; set; }

        public long? OriginalSize { get; set; }

        public long? CompressedSize { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDeletedStatu { get; set; } = false;

        public bool? AiAvif { get; set; }    

        public DateTime? DeletedAtDate { get; set; }
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
namespace data
{
    // <summary>
    // Satıcının (Vendor/Seller) açtığı mağazayı temsil eder.
    // Çoklu satıcılı (Marketplace) yapı için kullanılır.
    // </summary>
    public class Store
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } //
        public string? Name { get; set; } = string.Empty; //
        public string? Description { get; set; } = string.Empty; //

        // Admin Kontrolleri
        public bool? IsActiveStateAdmin { get; set; } = false; //
        public DateTime? IsActiveDateAdmin { get; set; } = new DateTime(); //


        // Satıcı kontrolleri
        public bool? IsActiveStateVendor { get; set; } = true; //
        public DateTime? IsActiveDateVendor { get; set; } = DateTime.Now; //

        public ContactInformation? ContactInformation { get; set; } = new(); 
        public ProfileCoverGallery? ProfileCoverGallery { get; set; } = new();
        public AddressInfo? AddressInfo { get; set; } = new(); 
        public WorkingHours? WorkingHours { get; set; } = new(); 

        // Medya Slotları - Belge GUID Tanımlamaları
        public Guid? CertificateOfIncorporation { get; set; }
        public Guid? ActivityCertificate { get; set; }
        public Guid? TaxRegistration { get; set; }
        public Guid? TradeRegistryGazette { get; set; }
        public Guid? SignatureCircular { get; set; }
        public Guid? AuthorizedPersonId { get; set; }
        public Guid? ProofOfBusinessAddress { get; set; }
        public Guid? BankStatement { get; set; }
        public Guid? BankAccountConfirmation { get; set; }
        public Guid? TrademarkCertificate { get; set; }
        public Guid? LetterOfAuthorization { get; set; }
        public Guid? QualityCertificates { get; set; }
        public Guid? CustomsRegistration { get; set; }
        public Guid? SocialSecurityRegistration { get; set; }
        public Guid? ProofOfOwnership { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
using data._Shared;
using System.ComponentModel.DataAnnotations;

namespace data
{
    public class Users
    {
        // === Temel Kimlik Bilgileri ===
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Birincil anahtar, otomatik artan benzersiz
        public Guid? WorkstationEmployeeGroupId { get; set; } // Bağlı olduğu İş istasyonu + 
        public bool? UserIsEmployee { get; set; } // Kullanıcı Çalışan Mı + 
        // Şifrele ***-***-***
        public string? UserSponsorEmail { get; set; } // Kullanıcının Sponsor Email Adresi + 
        // Şifrele ***-***-***
        public string? FirstName { get; set; } // Kullanıcının adı + 
        // Şifrele ***-***-***
        public string? LastName { get; set; } // Kullanıcının soyadı + 
        // Şifrele ***-***-***
        public string? Password { get; set; } // Şifrenin hashlenmiş (şifrelenmiş) hali + 

        // === Tarih Bilgileri ===
        public DateTime? RegistrationDate { get; set; } // Sisteme ilk kayıt olduğu tarih ve saat + 
        public bool AccountActivationMailStatu { get; set; } // Hesap aktivasyon durumu +
        public int AccountActivationMailCode { get; set; } // Hesap aktivasyon kodu +
        public int AccountPasswordResetMailCode { get; set; } // Hesap şifre sıfırlama kodu +
        public DateTime? AccountActivationMailDeadline { get; set; } // Hesap aktivasyon geçerlilik süresi +
        public DateTime? DateOfBirth { get; set; } // Doğum Tarihi + 

        // === Kullanıcı Tercihleri ===
        // Şifrele ***-***-***
        public string? Language { get; set; } = "en"; // Arayüz dili tercihi (Varsayılan: İngilizce) (en,tr,az,de,es,fr,hi,pt,ru,zh seçenekleri olabilir) + 
        // Şifrele ***-***-***
        public string? Currency { get; set; } = "USD"; // Kullanıcının tercih ettiği para birimi (USD, EUR, TRY, AZN seçenekleri olabilir) + 
        // Şifrele ***-***-***
        public string? UsersType { get; set; } = "Customer"; // Kullanıcı Tipi (sadece "Customer", "SuperAdmin" seçenekleri olabilir) + 
        // Şifrele ***-***-***

        // === Durum ve Yetki Bilgileri ===
        public bool? IsActive { get; set; } = true; // Kullanıcı hesabı aktif mi dondurulmuş mu?
        public bool? IsActiveVendorStatu { get; set; } = false; // Mağaza açma yetkisi var mı?
        public bool? TermsOfUse { get; set; } // Kullanım koşullarını ve gizlilik sözleşmesini kabul etti mi?
        //public bool? IsDeleted { get; set; } = false; // Veritabanından silmek yerine "silindi" işaretlemek için (Soft Delete)

        // === Profil Bilgileri ===
        // Şifrele ***-***-***
        public string? BackgroundImagePath { get; set; } // Web site arka plan resmi +

        // === İlişkili Tablolar ===
        public ContactInformation? ContactInformation { get; set; }
        public ProfileCoverGallery? ProfileCoverGallery { get; set; }
        public IsPrivateOrPublic? IsPrivateOrPublic { get; set; }
        public UserRolesAccessPermissions? UserRolesAccessPermissions { get; set; }
        public IsDeleted? IsDeleted { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data._Shared
{
    [Owned]
    public class IsDeleted
    {
        // Silinme durumu (Soft Delete)
        public bool? IsDeletedStatu { get; set; } = false;

        // Silinme tarihi
        public DateTime? DeletedAtDate { get; set; }
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Depo/lojistik noktası tanımı. Mağaza bazlıdır.
    /// Adres bilgisi için mevcut AddressInfo owned type'ı yeniden kullanılır.
    /// </summary>
    public class Warehouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Depoyu ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Deponun bağlı olduğu mağazanın ID bilgisi

        public bool IsDefault { get; set; } = false; // Varsayılan depo mu
        public bool IsActive { get; set; } = true; // Depo aktif mi

        public AddressInfo? AddressInfo { get; set; } = new(); // Depo adres bilgisi (mevcut owned type)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant kombinasyon tablosu. Her varyantın hangi özellik-değer ikililerinden
    /// oluştuğunu tutar. Örn: KZK-KRM-S varyantı için iki satır:
    ///   (VariantId, Renk, Kırmızı) ve (VariantId, Beden, S)
    /// </summary>
    public class ProductVariantValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid AttributeId { get; set; } // Özellik (ProductAttributes.Id) — sorgu kolaylığı için denormalize
        public Guid AttributeValueId { get; set; } // Özellik değeri (ProductAttributeValues.Id)
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün varyantları. Birleşik tek-varyant modeli gereği basit ürünlerde de
    /// IsDefault = true olan tek bir varyant kaydı bulunur.
    /// Fiyat (ProductPrices) ve stok (ProductStocks) daima varyant üzerinden tutulur.
    /// </summary>
    public class ProductVariants
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Bağlı olduğu ürün (Products.Id)

        // === Kimlik / Tanımlayıcı Alanlar (Marketplace entegrasyon standartları) ===
        public string? Sku { get; set; } // Stok kodu (Örn: KZK-KRM-S)
        public string? Barcode { get; set; } // Barkod
        public string? Gtin { get; set; } // Global Trade Item Number
        public string? Upc { get; set; } // Universal Product Code
        public string? Ean { get; set; } // European Article Number
        public string? Isbn { get; set; } // Kitaplar için ISBN
        public string? Mpn { get; set; } // Manufacturer Part Number (Üretici parça numarası)

        // === Durum ===
        public bool IsDefault { get; set; } = false; // Varsayılan varyant mı (basit üründe tek kayıt true)
        public bool IsActive { get; set; } = true; // Varyant satışta mı
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası

        // === Fiziksel Özellikler (kargo/desi hesabı ShippingProfile bölen değerleri ile yapılır) ===
        public decimal? WeightKg { get; set; } // Ağırlık (kg)
        public decimal? WidthCm { get; set; } // Genişlik (cm)
        public decimal? HeightCm { get; set; } // Yükseklik (cm)
        public decimal? LengthCm { get; set; } // Uzunluk (cm)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant + Depo bazlı stok tablosu.
    /// Bir varyant birden fazla depoda tutulabilir (çoklu depo desteği);
    /// toplam stok = varyantın tüm depo satırlarının toplamıdır.
    /// </summary>
    public class ProductStocks
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id) — denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid WarehouseId { get; set; } // Depo (Warehouse.Id)

        public bool TrackStock { get; set; } = true; // Stok takibi yapılsın mı (dijital/hizmet ürünlerde false)

        public int Quantity { get; set; } = 0; // Mevcut stok adedi
        public int? MinStockLevel { get; set; } // Minimum stok miktarı
        public int? CriticalStockLevel { get; set; } // Kritik stok seviyesi (uyarı eşiği)
        public int? MaxOrderQuantity { get; set; } // Tek siparişte alınabilecek maksimum adet

        // "InStock", "OutOfStock", "PreOrder", "Backorder"
        public string? StockStatus { get; set; } = "InStock";

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow; // Son stok güncelleme tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürünün teknik özellik/spesifikasyon DEĞERLERİ (varyant üretmeyen bilgi alanları).
    /// İki kullanım şekli desteklenir:
    ///   1) Tanımlı özellik: AttributeId (+ AttributeValueId veya serbest CustomValue)
    ///      Örn: Ekran Boyutu → 55"
    ///   2) Tamamen serbest satır: CustomName + CustomValue (AttributeId null)
    ///      Örn: "Kutu İçeriği" → "TV, Kumanda, Duvar Aparatı" (yalnızca bu üründe geçerli)
    /// </summary>
    public class ProductSpecifications
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        public Guid? AttributeId { get; set; } // Tanımlı özellik (ProductAttributes.Id) — serbest satırda null
        public Guid? AttributeValueId { get; set; } // Önceden tanımlı değer seçildiyse (ProductAttributeValues.Id)

        public string? CustomName { get; set; } // Serbest özellik adı (AttributeId null ise kullanılır)
        public string? CustomValue { get; set; } // Serbest/metinsel değer (Text-Number tipli özelliklerde de kullanılır)

        public int DisplayOrder { get; set; } = 0; // Gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün SEO bilgileri. Products tablosundan ayrı tutulur — listeleme sorgularını
    /// şişirmez ve ileride dil bazlı SEO satırlarına genişletilebilir.
    /// </summary>
    public class ProductSeo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        public string? Slug { get; set; } // SEO dostu URL (Örn: samsung-55-oled-tv) — benzersiz olmalı
        public string? SeoTitle { get; set; } // SEO başlık (meta title)
        public string? SeoDescription { get; set; } // SEO açıklama (meta description)
        public string? SeoKeywords { get; set; } // SEO anahtar kelimeler (virgülle ayrılmış)
        public string? CanonicalUrl { get; set; } // Canonical adres (kopya içerik önleme)
        public string? MetaJson { get; set; } // Ek meta veriler (JSON: OpenGraph, schema.org vb.)

        public string? LanguageCode { get; set; } = "tr"; // Dil kodu — ileride çoklu dil SEO satırları için

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ana ürün tablosu. Tüm ürün tipleri (basit, varyantlı, dijital, hizmet, paket, harici)
    /// bu tablo üzerinden yönetilir. Basit ürünler dahi en az bir ProductVariants kaydına sahiptir
    /// (birleşik tek-varyant modeli) — böylece fiyat/stok her zaman varyant üzerinden okunur.
    /// </summary>
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === İlişkiler (ID bazlı, navigation property kullanılmaz) ===
        public Guid UserId { get; set; } // Ürün sahibi kullanıcı (Users.Id)
        public Guid StoreId { get; set; } // Ürün sahibi mağaza (Store.Id)
        public Guid? BrandId { get; set; } // Marka (Brands.Id)
        public Guid? AttributeTemplateId { get; set; } // Ürünün oluşturulduğu özellik şablonu SÜRÜMÜ (AttributeTemplates.Id) — versiyon sabitlenir, şablon güncellense bile ürün verisi bozulmaz
        public Guid? ShippingProfileId { get; set; } // Kargo/desi profili (ShippingProfile.Id)

        // === Ürün Tipi ===
        // "Simple", "Variant", "Digital", "Service", "Bundle", "External"
        // Yeni tipler için enum yerine string tercih edildi (genişletilebilirlik)
        public string? ProductType { get; set; } = "Simple";

        // === İçerik Alanları ===
        public string? Name { get; set; } // Ürün adı
        public string? ShortDescription { get; set; } // Kısa açıklama
        public string? FullDescription { get; set; } // Detaylı açıklama (HTML destekli)
        public string? Tags { get; set; } // Etiketler (virgülle ayrılmış)

        // === Medya ===
        public Guid? CoverMediaId { get; set; } // Kapak görseli (Media.Id)
        // Galeri görselleri: ItemGallery (ItemType = "ProductGallery", ItemId = Products.Id)
        // Varyant görselleri: ItemGallery (ItemType = "VariantGallery", ItemId = ProductImageVariantGroups.Id)

        // === Harici Ürün (External) Alanları ===
        public string? ExternalUrl { get; set; } // Harici ürün bağlantısı
        public string? ExternalButtonText { get; set; } // Harici ürün buton metni (Örn: "Satıcı sitesinde gör")

        // === Yayın Durumu ===
        // "Draft", "PendingApproval", "Published", "Rejected", "Archived"
        public string? PublishStatus { get; set; } = "Draft";
        public bool? IsApprovedByAdmin { get; set; } // Admin onay durumu
        public DateTime? ApprovedDate { get; set; } // Admin onay tarihi
        public Guid? ApprovedByUserId { get; set; } // Onaylayan admin (Users.Id)
        public bool IsActive { get; set; } = true; // Satıcı tarafından aktif/pasif

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant bazlı, çoklu para birimli fiyat tablosu (APPEND-ONLY tarih geçmişi).
    /// Her para birimi (TRY/USD/EUR/AZN) için ayrı satır tutulur — kur dönüşümü YAPILMAZ,
    /// değerler bağımsız olarak veri tabanında saklanır.
    /// Fiyat değişiminde eski satır güncellenmez: EffectiveTo kapatılır, yeni satır açılır.
    /// Güncel fiyat sorgusu: EffectiveTo == null olan satır.
    /// </summary>
    public class ProductPrices
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id) — raporlama kolaylığı için denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id) — basit üründe varsayılan varyant

        public string? Currency { get; set; } = "TRY"; // Para birimi ("TRY", "USD", "EUR", "AZN")

        public decimal Price { get; set; } // Normal satış fiyatı
        public decimal? DiscountedPrice { get; set; } // İndirimli satış fiyatı (kampanya) — null ise indirim yok
        public decimal? CostPrice { get; set; } // Maliyet fiyatı (yalnızca satıcı görür)

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow; // Geçerlilik başlangıcı
        public DateTime? EffectiveTo { get; set; } // Geçerlilik bitişi (null = güncel/aktif fiyat)

        public Guid? CreatedByUserId { get; set; } // Fiyatı giren kullanıcı (denetim için)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Kayıt tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün/varyantın dış platformlardaki karşılığı ve senkronizasyon durumu.
    /// İçe aktarımda platformdan gelen HAM VERİ RawSourceData alanında JSON olarak saklanır —
    /// böylece modelimizde karşılığı olmayan alanlar dahi VERİ KAYBI olmadan korunur
    /// ve ileride yeniden işlenebilir.
    /// </summary>
    public class ProductMarketplaceListings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid? VariantId { get; set; } // Varyant bazlı eşleme gerekiyorsa (ProductVariants.Id)
        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid StoreId { get; set; } // Mağaza (Store.Id) — hangi mağazanın entegrasyonu

        public string? ExternalProductId { get; set; } // Platformdaki ürün ID'si (Örn: Amazon ASIN, Trendyol contentId)
        public string? ExternalSku { get; set; } // Platformdaki SKU (merchantSku vb.)
        public string? ExternalUrl { get; set; } // Platformdaki ürün sayfası adresi

        // "Pending", "Synced", "Error", "Paused"
        public string? SyncStatus { get; set; } = "Pending";
        public DateTime? LastSyncDate { get; set; } // Son başarılı senkronizasyon tarihi
        public string? LastErrorMessage { get; set; } // Son hata mesajı

        public string? RawSourceData { get; set; } // İçe aktarımda gelen ham kaynak verisi (JSON) — veri kaybı sigortası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// CSV / XML / JSON / API içe aktarım profilleri.
    /// Satıcı, kaynak dosya kolonlarının sistem alanlarına nasıl eşleneceğini bir kez tanımlar
    /// (FieldMappingJson), sonraki aktarımlarda profili yeniden kullanır.
    /// Trendyol, Amazon, WooCommerce, N11 gibi hazır kaynak tipleri desteklenir.
    /// </summary>
    public class ProductImportProfiles
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Profili oluşturan kullanıcı (Users.Id)
        public Guid StoreId { get; set; } // Bağlı mağaza (Store.Id)

        public string? Name { get; set; } // Profil adı (Örn: "Trendyol Aylık Aktarım")

        // "CSV", "XML", "JSON", "RestApi", "SoapApi", "Amazon", "Alibaba", "Ebay", "MercadoLibre",
        // "Jd", "Trendyol", "Hepsiburada", "N11", "CicekSepeti", "Etsy", "Shopify", "WooCommerce"
        public string? SourceType { get; set; } = "CSV";

        public string? FieldMappingJson { get; set; } // Kolon → alan eşleme tanımı (JSON)
        public string? Delimiter { get; set; } = ";"; // CSV ayıracı
        public string? Encoding { get; set; } = "UTF-8"; // Dosya karakter seti
        public string? SourceUrl { get; set; } // API/XML feed adresi (dosya dışı kaynaklarda)

        public Guid? DefaultWarehouseId { get; set; } // Aktarılan ürünlerin varsayılan deposu (Warehouse.Id)
        public int? DefaultCategoryId { get; set; } // Eşlenemeyen ürünler için varsayılan kategori (CategoriesTr.Id)

        public bool IsActive { get; set; } = true; // Profil kullanımda mı
        public DateTime? LastRunDate { get; set; } // Son çalıştırma tarihi
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant görsel grupları.
    /// Görseller varyant kombinasyonuna DEĞİL, görsel-varyant özelliğinin DEĞERİNE bağlanır:
    ///   Örn: Renk görsel varyantı ise → Kırmızı için 1 grup, Bordo için 1 grup, Siyah için 1 grup.
    ///   Kırmızı-S ve Kırmızı-M aynı (Kırmızı) görsel grubunu paylaşır.
    /// Görseller ItemGallery üzerinden bağlanır:
    ///   ItemGallery.ItemType = "VariantGallery", ItemGallery.ItemId = ProductImageVariantGroups.Id
    /// Bir gruba birden fazla görsel eklenebilir (ItemGallery zaten çoklu kayıt destekler).
    /// </summary>
    public class ProductImageVariantGroups
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid AttributeId { get; set; } // Görsel varyant özelliği (ProductAttributes.Id — Örn: Renk)
        public Guid AttributeValueId { get; set; } // Görselin bağlandığı değer (ProductAttributeValues.Id — Örn: Kırmızı)

        public Guid? CoverMediaId { get; set; } // Bu değerin kapak görseli (Media.Id) — listelemede gösterilir

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürünün galeri dışı medya kaynakları: video URL'leri, 360 derece görünüm,
    /// embed içerikler ve PDF/doküman dosyaları.
    /// Sistemde yüklü dosyalar MediaId ile (Media.Id), harici kaynaklar Url ile bağlanır.
    /// </summary>
    public class ProductExternalMedias
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        // "VideoUrl", "View360", "Embed", "Document"
        public string? MediaType { get; set; } = "VideoUrl";

        public string? Url { get; set; } // Harici kaynak adresi (YouTube, Vimeo, 360 viewer vb.)
        public Guid? MediaId { get; set; } // Sistemde yüklü dosya (Media.Id) — PDF/doküman için

        public string? Title { get; set; } // Gösterim başlığı (Örn: "Kurulum Kılavuzu")
        public int DisplayOrder { get; set; } = 0; // Gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Dijital ürün dosyaları (ProductType = "Digital").
    /// Satın alma sonrası indirilebilir dosyalar ve indirme kuralları burada tutulur.
    /// </summary>
    public class ProductDigitalAssets
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid? VariantId { get; set; } // Dosya belirli bir varyanta özelse (ProductVariants.Id)

        public Guid MediaId { get; set; } // İndirilebilir dosya (Media.Id)
        public string? DisplayName { get; set; } // Müşteriye gösterilecek dosya adı

        public int? DownloadLimit { get; set; } // Maksimum indirme sayısı (null = sınırsız)
        public int? ExpirationDays { get; set; } // Satın alma sonrası geçerlilik süresi - gün (null = süresiz)
        public bool RequiresLicenseKey { get; set; } = false; // Lisans anahtarı üretilsin mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün - Kategori çoklu ilişki tablosu.
    /// Bir ürün birden fazla kategoriye bağlanabilir; IsPrimary = true olan kayıt ana kategoridir.
    /// </summary>
    public class ProductCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id — int tip)

        public bool IsPrimary { get; set; } = false; // Ana kategori mi (ürün başına yalnızca 1 adet true olmalı)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Paket/Bundle ürün içerik tablosu.
    /// ProductType = "Bundle" olan ürünün hangi alt ürün/varyantlardan oluştuğunu tutar.
    /// </summary>
    public class ProductBundleItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BundleProductId { get; set; } // Paket ürün (Products.Id, ProductType = "Bundle")
        public Guid ChildProductId { get; set; } // Pakete dahil ürün (Products.Id)
        public Guid? ChildVariantId { get; set; } // Belirli bir varyant zorunluysa (ProductVariants.Id), null = müşteri seçer

        public int Quantity { get; set; } = 1; // Paketteki adet
        public decimal? DiscountRate { get; set; } // Paket içi indirim oranı (%) — opsiyonel

        public int DisplayOrder { get; set; } = 0; // Paket içeriği gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Özelliklerin önceden tanımlı değerleri (Örn: Renk → Kırmızı, Bordo, Siyah).
    /// Sahiplik kuralı ProductAttributes ile aynıdır:
    ///   UserId/StoreId null → sistem değeri, dolu → yalnızca o satıcıda görünen özel değer.
    /// Böylece sistem özelliği olan "Renk"e bir satıcı kendi özel rengini ekleyebilir
    /// ama bu değer diğer satıcılara yansımaz.
    /// </summary>
    public class ProductAttributeValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeId { get; set; } // Bağlı olduğu özellik (ProductAttributes.Id)

        public Guid? UserId { get; set; } // Null = sistem değeri, dolu = satıcıya özel değer
        public Guid? StoreId { get; set; } // Null = sistem değeri, dolu = mağazaya özel değer

        public string? Value { get; set; } // Değer metni (Örn: Kırmızı, 16 GB, 55")
        public string? Code { get; set; } // Teknik kod (Örn: kirmizi) — entegrasyon eşlemede kullanılır
        public string? ColorHex { get; set; } // Renk özellikleri için HEX kodu (Örn: #FF0000) — UI renk kutucuğu
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası
        public bool IsActive { get; set; } = true; // Değer kullanımda mı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Özellik (Attribute) tanımları — tamamen dinamik özellik sistemi.
    /// KRİTİK SAHİPLİK KURALI:
    ///   UserId = null ve StoreId = null → SİSTEM özelliği (yalnızca admin oluşturur/düzenler, şablonlarda kullanılabilir)
    ///   UserId/StoreId dolu             → SATICIYA ÖZEL özellik (yalnızca o satıcının ürünlerinde görünür,
    ///                                      asla şablonlara eklenmez, diğer satıcılara gösterilmez)
    /// </summary>
    public class ProductAttributes
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem özelliği, dolu = satıcıya özel
        public Guid? StoreId { get; set; } // Null = sistem özelliği, dolu = mağazaya özel

        public string? Name { get; set; } // Özellik adı (Örn: Renk, RAM, Ekran Boyutu)
        public string? Code { get; set; } // Teknik kod / slug (Örn: renk, ram) — entegrasyon eşlemede kullanılır
        public string? Description { get; set; } // Özellik açıklaması

        // Giriş tipi: "Select", "MultiSelect", "Text", "Number", "Bool", "Date"
        public string? InputType { get; set; } = "Select";

        public string? Unit { get; set; } // Birim (Örn: GB, inç, Hz) — Number tipinde kullanışlı
        public bool IsFilterable { get; set; } = false; // Ürün listelerinde filtre olarak gösterilsin mi
        public bool IsActive { get; set; } = true; // Özellik kullanımda mı
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir ürünün hangi özellikleri, hangi rollerde kullandığını tanımlar.
    /// VARYANT GÖRSEL YÖNETİMİNİN KALBİ BURASIDIR:
    ///   IsVariantAttribute      → bu özellik varyant kombinasyonu üretir (Örn: Renk, Beden)
    ///   IsImageVariantAttribute → bu özelliğin DEĞERLERİNE görsel grubu atanır (Örn: yalnızca Renk)
    /// Bu seçim ÜRÜN BAZINDA yapılır: bir üründe Renk görsel varyantı iken,
    /// başka bir üründe Beden görsel varyantı olabilir.
    /// </summary>
    public class ProductAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid AttributeId { get; set; } // Özellik (ProductAttributes.Id)

        public bool IsVariantAttribute { get; set; } = false; // Varyant üreten özellik mi (Renk, Beden vb.)
        public bool IsImageVariantAttribute { get; set; } = false; // Görsel grubu bu özelliğin değerlerine mi bağlanacak
        public bool IsRequired { get; set; } = false; // Ürün kaydında zorunlu mu

        public Guid? SourceTemplateId { get; set; } // Özellik bir şablondan geldiyse şablon sürümü (AttributeTemplates.Id), satıcı manuel eklediyse null

        public int DisplayOrder { get; set; } = 0; // Ürün detayında gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Entegre platform tanımları (Amazon, Trendyol, Hepsiburada, eBay, Shopify vb.).
    /// Admin yönetimli, sistem geneli referans tablosudur.
    /// </summary>
    public class Marketplaces
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } // Platform adı (Örn: Trendyol)
        public string? Code { get; set; } // Teknik kod (Örn: trendyol, amazon, n11) — entegrasyon katmanında anahtar
        public string? ApiBaseUrl { get; set; } // API kök adresi
        public Guid? LogoMediaId { get; set; } // Platform logosu (Media.Id)

        public bool IsActive { get; set; } = true; // Entegrasyon aktif mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem kategorilerinin platform kategorilerine eşlenmesi.
    /// Örn: CategoriesTr "Televizyon" ↔ Trendyol "TV & Görüntü Sistemleri" (id: 1234).
    /// </summary>
    public class MarketplaceCategoryMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public int CategoryId { get; set; } // Sistem kategorisi (CategoriesTr.Id — int tip)

        public string? ExternalCategoryCode { get; set; } // Platformdaki kategori kodu/ID'si
        public string? ExternalCategoryName { get; set; } // Platformdaki kategori adı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem özelliklerinin platform özelliklerine eşlenmesi
    /// (Amazon Attribute Mapping, eBay Item Specifics, Trendyol Attributes vb.).
    /// İçeri/dışarı aktarımda veri kaybını önler: "renk" ↔ Trendyol "color" (id: 47) gibi.
    /// Eşleme kategoriye özel olabilir (platformlar kategori bazlı özellik ister).
    /// </summary>
    public class MarketplaceAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid AttributeId { get; set; } // Sistem özelliği (ProductAttributes.Id)
        public int? CategoryId { get; set; } // Eşleme kategoriye özelse (CategoriesTr.Id), null = genel

        public string? ExternalAttributeCode { get; set; } // Platformdaki özellik kodu/ID'si
        public string? ExternalAttributeName { get; set; } // Platformdaki özellik adı
        public string? ValueMappingJson { get; set; } // Değer eşleme sözlüğü (JSON): {"kirmizi":"Red","bordo":"Maroon"}

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Marka tanımları. UserId/StoreId null ise sistem geneli (admin) markasıdır,
    /// dolu ise yalnızca ilgili satıcıya/mağazaya özeldir.
    /// </summary>
    public class Brands
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem markası, dolu = satıcıya özel marka
        public Guid? StoreId { get; set; } // Null = sistem markası, dolu = mağazaya özel marka

        public string? Name { get; set; } // Marka adı (Örn: Samsung)
        public string? Slug { get; set; } // SEO dostu marka kodu (Örn: samsung)
        public string? Description { get; set; } // Marka açıklaması
        public Guid? LogoMediaId { get; set; } // Marka logosu (Media.Id)
        public string? WebsiteUrl { get; set; } // Marka resmi web sitesi

        public bool IsActive { get; set; } = true; // Marka aktif mi
        public bool IsApprovedByAdmin { get; set; } = false; // Satıcı markası admin onayından geçti mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// SİSTEM ÖZELLİK ŞABLONLARI (yalnızca admin yönetimli).
    /// VERSİYONLAMA MODELİ:
    ///   - Her sürüm ayrı bir satırdır (immutable). Yayınlanan sürüm asla değiştirilmez.
    ///   - Aynı şablonun tüm sürümleri ortak TemplateGroupId değerini paylaşır.
    ///   - Yeni sürüm = aynı TemplateGroupId, Version + 1, yeni AttributeTemplateItems seti.
    ///   - Ürünler Products.AttributeTemplateId ile SÜRÜME sabitlenir; böylece şablon
    ///     güncellendiğinde mevcut ürünlerin veri bütünlüğü bozulmaz.
    ///   - Yeni ürünler her zaman grubun IsPublished = true olan en yüksek sürümünü kullanır.
    /// </summary>
    public class AttributeTemplates
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } = Guid.NewGuid(); // Sürüm grubu kimliği (tüm sürümlerde aynı)
        public int Version { get; set; } = 1; // Sürüm numarası (1, 2, 3...)

        public string? Name { get; set; } // Şablon adı (Örn: Televizyon)
        public string? Description { get; set; } // Şablon açıklaması
        public string? VersionNotes { get; set; } // Bu sürümde yapılan değişikliklerin notu

        public bool IsPublished { get; set; } = false; // Yayında mı (yeni ürünlerde kullanılabilir mi)
        public bool IsActive { get; set; } = true; // Grup genelinde aktiflik (false → yeni ürünlere kapalı)

        public Guid? CreatedByUserId { get; set; } // Şablonu oluşturan admin (Users.Id)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? PublishedAt { get; set; } // Yayınlanma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Şablon sürümü ile sistem özellikleri arasındaki bağlantı.
    /// Yalnızca SİSTEM özellikleri (ProductAttributes.UserId == null) eklenebilir —
    /// satıcı özel özellikleri şablonlara ASLA dahil edilmez (uygulama katmanında doğrulanır).
    /// </summary>
    public class AttributeTemplateItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateId { get; set; } // Şablon SÜRÜMÜ (AttributeTemplates.Id)
        public Guid AttributeId { get; set; } // Sistem özelliği (ProductAttributes.Id)

        public bool IsRequired { get; set; } = false; // Bu şablonda zorunlu mu
        public bool IsVariantSuggested { get; set; } = false; // Varyant özelliği olarak önerilsin mi (Örn: Renk)

        public int DisplayOrder { get; set; } = 0; // Şablondaki gösterim sırası
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Şablon - Kategori atama tablosu.
    /// TemplateGroupId üzerinden bağlanır (sürüm değil) — böylece yeni sürüm yayınlandığında
    /// kategori atamalarını tekrar yapmaya gerek kalmaz; kategori her zaman grubun
    /// yayındaki en güncel sürümünü sunar. Bir şablon birden fazla kategoriye atanabilir.
    /// </summary>
    public class AttributeTemplateCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } // Şablon grubu (AttributeTemplates.TemplateGroupId)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id — int tip)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
