## Dosya: AttributeTemplateCategories.cs
```csharp
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
```

## Dosya: AttributeTemplateItems.cs
```csharp
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
```

## Dosya: AttributeTemplates.cs
```csharp
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
```

## Dosya: Brands.cs
```csharp
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

        public string? Name { get; set; } // Marka adı (Örn: Samsung)
        public string? Slug { get; set; } // SEO dostu marka kodu (Örn: samsung)

        public bool IsApprovedByAdmin { get; set; } = false; // Satıcı markası admin onayından geçti mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
```

## Dosya: MarketplaceAttributeMappings.cs
```csharp
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
```

## Dosya: MarketplaceCategoryMappings.cs
```csharp
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
```

## Dosya: Marketplaces.cs
```csharp
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
```

## Dosya: ProductAttributeMappings.cs
```csharp
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
```

## Dosya: ProductAttributes.cs
```csharp
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
```

## Dosya: ProductAttributeValues.cs
```csharp
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
```

## Dosya: ProductBundleItems.cs
```csharp
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
```

## Dosya: ProductCategories.cs
```csharp
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
```

## Dosya: ProductDigitalAssets.cs
```csharp
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
```

## Dosya: ProductExternalMedias.cs
```csharp
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
```

## Dosya: ProductImageVariantGroups.cs
```csharp
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
```

## Dosya: ProductImportProfiles.cs
```csharp
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
```

## Dosya: ProductMarketplaceListings.cs
```csharp
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
```

## Dosya: ProductPrices.cs
```csharp
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
```

## Dosya: ProductReview.cs
```csharp
﻿using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace data._Product
{
    public class ProductReview
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public string CommentText { get; set; }

        // Eğer bu alan null ise ana yorumdur. 
        // Bir değer içeriyorsa, yanıt verilen üst yorumun ID'sini tutar.
        public Guid? ParentReviewId { get; set; }

        // Yapay zeka yapılan yorumu kontrol eder ve eğer koşullar sağlanıyorsa true döner. Aksi takdirde false döner.
        public bool? ConfirmedByAi { get; set; }

        // Yapay zeka eğer yorumu onaylamaz ise nedenini burada belirtir. Eğer yorum onaylanmışsa bu alan null olur.
        public string? WhyDidAiNotApproveIt { get; set; }

        // Yorumun oluşturulma tarihi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Yorumun AI tarafından kontrol edilme tarihi. Bu tarih, yorum oluşturulduktan sonra AI tarafından kontrol edildiğinde güncellenir.
        public DateTime? AIControlDate { get; set; }
        public IsDeleted IsDeleted { get; set; }

    }
}
```

## Dosya: Products.cs
```csharp
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
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? AttributeTemplateId { get; set; }
        public Guid? ShippingProfileId { get; set; }

        // === Ürün Tipi ===
        public string? ProductType { get; set; } = "Simple";

        // === İçerik Alanları ===
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string? Tags { get; set; }

        // === Medya ===
        public Guid? CoverMediaId { get; set; }

        // === Harici Ürün (External) Alanları ===
        public string? ExternalUrl { get; set; }
        public string? ExternalButtonText { get; set; }

        // === Yayın Durumu ===
        public bool? PublishStatus { get; set; }
        public bool? IsApprovedByAdmin { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // === AI İçerik Yönetimi ===
        // Satıcı ürün eklerken "AI Yönetimine İzin Ver" toggle'ını açarsa true olur.
        public bool? IsAiManaged { get; set; } = false; // Satıcı AI yönetimine izin veriyor mu

        // null → AI yönetimi yapılmadı (IsAiManaged=false) | true → AI tarafından başarıyla oluşturuldu | false → AI işlemi başarısız oldu
        public bool? AiContentStatus { get; set; } // AI işlem durumu

        // Satıcının orijinal girişi — AI kapatılırsa geri yüklenir
        public string? AiOriginalName { get; set; }
        public string? AiOriginalShortDescription { get; set; }
        public string? AiOriginalFullDescription { get; set; }
        public string? AiOriginalTags { get; set; }

        public string? AiErrorMessage { get; set; }          // Son hata mesajı (Failed durumunda)
        public int AiRetryCount { get; set; } = 0;           // Tekrar deneme sayısı
        public DateTime? AiProcessedAt { get; set; }         // AI'ın işlemi tamamladığı an

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? InteractionCounts { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}```

## Dosya: ProductSeo.cs
```csharp
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
```

## Dosya: ProductSpecifications.cs
```csharp
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
```

## Dosya: ProductStocks.cs
```csharp
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
```

## Dosya: ProductVariants.cs
```csharp
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
```

## Dosya: ProductVariantValues.cs
```csharp
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
```

## Dosya: Warehouse.cs
```csharp
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
```

## Dosya: ProductImportJobs.cs
```csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product.BulkWordPressProductImport
{
    /// <summary>
    /// İçe aktarım OTURUMU (job/run). WooCommerce "Dışa Aktar" CSV dosyasının aktarım
    /// sürecini baştan sona temsil eder ve KALDIĞI YERDEN DEVAM (resume) yeteneğinin kalbidir.
    ///
    /// HAFIZA MANTIĞI:
    ///   - SourceMediaId  → galeriden seçilen CSV dosyası (Media.Id). Sayfa kapansa bile job
    ///                      hangi dosyayla çalıştığını hatırlar.
    ///   - LastProcessedRowIndex → işlenen son satır. Yeniden başlatıldığında buradan devam eder.
    ///   - FieldMappingJson / AnalysisReportJson → analiz ve alan eşleşmeleri job içinde saklanır,
    ///     böylece kullanıcı her açışında baştan eşleştirme yapmaz.
    ///
    /// IDEMPOTENT/RESUMABLE: Gerçek satır-bazlı ilerleme ProductImportRows tablosunda tutulur;
    /// bu tablo job seviyesindeki özet sayaçları ve durumu yönetir.
    /// </summary>
    public class ProductImportJobs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === İlişkiler (ID bazlı, navigation property kullanılmaz) ===
        public Guid UserId { get; set; }                 // İçe aktarımı başlatan kullanıcı (Users.Id)
        public Guid StoreId { get; set; }                // Hedef mağaza (Store.Id)
        public Guid? ProfileId { get; set; }             // İsteğe bağlı eşleme profili (ProductImportProfiles.Id)
        public Guid SourceMediaId { get; set; }          // Galeriden seçilen kaynak CSV dosyası (Media.Id) — HAFIZA

        // === Tanım ===
        public string? Name { get; set; }                // Job adı (Örn: "Başer Elektrik - Aralık Aktarımı")

        // "CSV", "WooCommerce", "Trendyol", "XML", "JSON" ... — bu akışta varsayılan WooCommerce
        public string? SourceType { get; set; } = "WooCommerce";
        public string? Delimiter { get; set; } = ",";    // CSV ayıracı (WooCommerce dışa aktarımı virgül kullanır)
        public string? Encoding { get; set; } = "UTF-8"; // Karakter seti (BOM'lu UTF-8)

        // === Durum Makinesi ===
        // "Draft"     → oluşturuldu, henüz hazır değil (analiz/eşleştirme sürüyor)
        // "Ready"     → tüm eşleştirmeler onaylandı, başlatılabilir
        // "Running"   → aktif olarak satır işliyor
        // "Paused"    → kullanıcı duraklattı (resume edilebilir)
        // "Completed" → tüm satırlar işlendi
        // "Failed"    → kurtarılamaz hata ile durdu
        // "Cancelled" → kullanıcı iptal etti
        public string? Status { get; set; } = "Draft";

        // === Canlı Sayaçlar (Dashboard panelini besler) ===
        public int TotalCount { get; set; } = 0;         // Toplam Ürün (CSV satır sayısı)
        public int ImportedCount { get; set; } = 0;      // Aktarılan
        public int FailedCount { get; set; } = 0;        // Hatalı
        public int SkippedCount { get; set; } = 0;       // Atlanan (duplicate / kapsam dışı)
        // Bekleyen = TotalCount - ImportedCount - FailedCount - SkippedCount (uygulama katmanında hesaplanır)
        public decimal SuccessRate { get; set; } = 0m;   // Başarı Oranı (%) — denormalize, raporlama kolaylığı

        // === Kaldığı Yerden Devam (Resume Cursor) ===
        public int LastProcessedRowIndex { get; set; } = 0; // İşlenen son satırın sıfır-tabanlı indeksi
        public int BatchSize { get; set; } = 500;           // Tek seferde işlenecek satır sayısı (bellek dostu)

        // === Eşleşme & Analiz Hafızası (JSON) ===
        public string? FieldMappingJson { get; set; }       // Kolon → sistem alanı eşlemeleri (onaylanmış)
        public string? AnalysisReportJson { get; set; }     // AŞAMA-1 kolon/veri tipi/boş alan analiz raporu
        public string? StrategyReportJson { get; set; }     // AŞAMA-13 import strateji raporu

        // === Varsayılanlar (eşlenemeyen veriler için) ===
        public Guid? DefaultWarehouseId { get; set; }       // Stok yazılacak varsayılan depo (Warehouse.Id)
        public int? DefaultCategoryId { get; set; }         // Eşlenemeyen ürünler için varsayılan kategori (CategoriesTr.Id)
        public string? DefaultCurrency { get; set; } = "TRY"; // CSV fiyatlarının para birimi

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? StartedAt { get; set; }                    // İlk başlatılma tarihi
        public DateTime? LastRunAt { get; set; }                    // Son batch çalıştırma tarihi
        public DateTime? CompletedAt { get; set; }                  // Tamamlanma/iptal tarihi
        public DateTime? UpdatedAt { get; set; }                    // Son güncelleme tarihi

        public string? LastErrorMessage { get; set; }              // Job seviyesinde son kritik hata

        public IsDeleted? IsDeleted { get; set; } = new();         // Silinme durumu (soft delete)
    }
}
```
.
## Dosya: ProductImportMappings.cs
```csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product.BulkWordPressProductImport
{
    /// <summary>
    /// BİRLEŞİK EŞLEŞME HAFIZASI. WooCommerce alan/kategori/marka/özellik değerlerinin
    /// sistemdeki karşılıklarını TEK tabloda, tip ayraçlı (MappingType) olarak saklar.
    ///
    /// "KATEGORİ EŞLEŞTİRME HAFIZASI" KURALI:
    ///   Bir eşleşme yapıldıktan ve onaylandıktan sonra (IsConfirmedByUser = true) aynı kaynak
    ///   değer tekrar kullanıcıya SORULMAZ. SourceKeyHash üzerinden hızlı sözlük araması yapılır.
    ///
    /// KAPSAM (JobId):
    ///   - JobId dolu  → yalnızca o aktarıma özel eşleşme.
    ///   - JobId null  → kullanıcı/mağaza genelinde KALICI eşleşme (sonraki aktarımlarda yeniden kullanılır).
    ///
    /// GÜVEN SEVİYESİ: ConfidenceLevel + ConfidenceScore ile her eşleşmenin ne kadar kesin
    /// olduğu işaretlenir; belirsiz olanlar (Uncertain/Unmapped) kullanıcı onayına düşer.
    /// </summary>
    public class ProductImportMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? JobId { get; set; }   // Bağlı aktarım (ProductImportJobs.Id) — null = kalıcı/global eşleşme
        public Guid UserId { get; set; }   // Sahip kullanıcı (Users.Id)
        public Guid StoreId { get; set; }  // Sahip mağaza (Store.Id)

        // "Field"          → CSV kolonu → sistem alanı     (Örn: "Normal fiyat" → Price)
        // "Category"       → WooCommerce kategori yolu → CategoriesTr.Id
        // "Brand"          → WooCommerce marka → Brands.Id
        // "Attribute"      → WooCommerce attribute başlığı → ProductAttributes.Id (bilgi dağarcığı)
        // "AttributeValue" → WooCommerce attribute değeri → ProductAttributeValues.Id
        // "StockStatus"    → instock/outofstock/onbackorder → sistem stok durumu
        // "TaxClass"       → vergi sınıfı eşlemesi
        public string? MappingType { get; set; }

        // === Kaynak (WooCommerce tarafı) ===
        public string? SourceKey { get; set; }      // Ham kaynak değer (kolon adı / kategori yolu / marka / attribute)
        public string? SourceKeyHash { get; set; }  // SourceKey'in normalize+hash hali — hızlı/benzersiz arama
        public string? SourceParentKey { get; set; } // Hiyerarşik kaynaklarda üst düğüm (kategori ağacı için)
        public int? SourceItemCount { get; set; }   // Bu kaynağa ait ürün adedi (kategori ağacı raporu: "65 ürün")

        // === Hedef (sistem tarafı) ===
        // "ProductField" | "CategoryId" | "BrandId" | "AttributeId" | "AttributeValueId" | "Literal"
        public string? TargetType { get; set; }
        public string? TargetValue { get; set; }    // Hedef değer (alan adı, Guid veya int — string olarak saklanır)
        public string? TargetDisplayName { get; set; } // UI'da gösterilecek hedef adı (Örn: "Uydu Ekipmanları")

        // === Güven / Onay ===
        // "Exact"    → kesin eşleşme (Örn: Weight → WeightKg)
        // "Likely"   → büyük olasılıkla (Örn: Normal fiyat → Price)
        // "Uncertain"→ belirsiz, kullanıcı onayı şart (Örn: Meta: custom_field_12)
        // "Unmapped" → henüz eşlenmedi
        public string? ConfidenceLevel { get; set; } = "Unmapped";
        public int ConfidenceScore { get; set; } = 0; // 0–100 güven puanı (otomatik öneri sıralaması)
        public bool IsConfirmedByUser { get; set; } = false; // Kullanıcı onayladı mı (true → tekrar sorulmaz)
        public bool CreateIfMissing { get; set; } = false;   // Hedef yoksa yeni oluşturulsun mu (marka/değer için)

        public string? SuggestionsJson { get; set; } // Akıllı öneri listesi (JSON) — UI'da kullanıcıya sunulan adaylar

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; }                    // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new();          // Silinme durumu (soft delete)
    }
}
```
.
## Dosya: ProductImportRows.cs
```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product.BulkWordPressProductImport
{
    /// <summary>
    /// SATIR BAZLI İLERLEME ve HATA RAPORU. Her CSV satırının aktarım durumunu tutar.
    /// IDEMPOTENT ve RESUMABLE çalışmanın temelidir:
    ///   - SourceExternalId (WooCommerce "Kimlik") benzersiz anahtardır → aynı satır iki kez
    ///     işlenmez, DUPLICATE ürün oluşmaz. Yeniden çalıştırmada "Imported" satırlar atlanır.
    ///   - RawRowJson, satırın HAM halini saklar (veri kaybı sigortası) → ileride yeniden işlenebilir.
    ///
    /// AŞAMA-12 HATA RAPORLAMA: Eksik kategori, bozuk görsel, hatalı fiyat, eksik/yinelenen SKU,
    /// geçersiz varyasyon gibi durumlar ErrorCode/ErrorMessage ile satır seviyesinde raporlanır.
    /// </summary>
    public class ProductImportRows
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid JobId { get; set; }    // Bağlı aktarım (ProductImportJobs.Id)
        public Guid UserId { get; set; }   // Sahip kullanıcı (Users.Id) — denormalize
        public Guid StoreId { get; set; }  // Sahip mağaza (Store.Id) — denormalize

        public int SourceRowIndex { get; set; }        // CSV içindeki sıfır-tabanlı satır indeksi (resume için)
        public string? SourceExternalId { get; set; }  // WooCommerce "Kimlik" — IDEMPOTENCY anahtarı
        public string? SourceSku { get; set; }         // Kaynak SKU (varsa)
        public string? ParentExternalId { get; set; }  // Varyasyon ise üst ürünün Kimlik'i ("Ebeveyn")

        // "simple" | "variable" | "variation" | "grouped" | "external"
        public string? SourceProductType { get; set; }

        // "Pending"   → henüz işlenmedi
        // "Imported"  → başarıyla aktarıldı
        // "Failed"    → hata ile sonuçlandı (ErrorCode/ErrorMessage dolu)
        // "Skipped"   → kapsam dışı / kullanıcı atladı
        // "Duplicate" → daha önce aktarılmış (idempotency engeli)
        public string? RowStatus { get; set; } = "Pending";

        // === Üretilen Kayıtlar (geri izleme + rollback için) ===
        public Guid? CreatedProductId { get; set; }    // Oluşan ürün (Products.Id)
        public Guid? CreatedVariantId { get; set; }    // Oluşan varyant (ProductVariants.Id)

        // === Hata Detayı (AŞAMA-12) ===
        // "MissingCategory" | "MissingBrand" | "BrokenImage" | "InvalidPrice" | "MissingSku"
        // "DuplicateSku" | "InvalidVariation" | "OrphanVariation" | "MediaProcessError" | "Unknown"
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }      // İnsan-okur hata açıklaması
        public string? WarningsJson { get; set; }      // Bloklamayan uyarılar (JSON) — Örn: "görsel bulunamadı, atlandı"

        public string? RawRowJson { get; set; }        // Satırın ham CSV verisi (JSON) — veri kaybı sigortası

        public DateTime? ProcessedAt { get; set; }     // İşlenme tarihi
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Kayıt tarihi
    }
}
```
.
## Dosya: ProductHistory.cs
```csharp
﻿using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Product.ProductHistory
{
    public class ProductHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductSlug { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public IsDeleted? IsDeleted { get; set; } = new(); 
    }
}
```
.
