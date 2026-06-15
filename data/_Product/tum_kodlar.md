
## Dosya: AttributeTemplateCategories.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Åablon - Kategori atama tablosu.
    /// TemplateGroupId Ã¼zerinden baÄŸlanÄ±r (sÃ¼rÃ¼m deÄŸil) â€” bÃ¶ylece yeni sÃ¼rÃ¼m yayÄ±nlandÄ±ÄŸÄ±nda
    /// kategori atamalarÄ±nÄ± tekrar yapmaya gerek kalmaz; kategori her zaman grubun
    /// yayÄ±ndaki en gÃ¼ncel sÃ¼rÃ¼mÃ¼nÃ¼ sunar. Bir ÅŸablon birden fazla kategoriye atanabilir.
    /// </summary>
    public class AttributeTemplateCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } // Åablon grubu (AttributeTemplates.TemplateGroupId)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id â€” int tip)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: AttributeTemplateItems.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Åablon sÃ¼rÃ¼mÃ¼ ile sistem Ã¶zellikleri arasÄ±ndaki baÄŸlantÄ±.
    /// YalnÄ±zca SÄ°STEM Ã¶zellikleri (ProductAttributes.UserId == null) eklenebilir â€”
    /// satÄ±cÄ± Ã¶zel Ã¶zellikleri ÅŸablonlara ASLA dahil edilmez (uygulama katmanÄ±nda doÄŸrulanÄ±r).
    /// </summary>
    public class AttributeTemplateItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateId { get; set; } // Åablon SÃœRÃœMÃœ (AttributeTemplates.Id)
        public Guid AttributeId { get; set; } // Sistem Ã¶zelliÄŸi (ProductAttributes.Id)

        public bool IsRequired { get; set; } = false; // Bu ÅŸablonda zorunlu mu
        public bool IsVariantSuggested { get; set; } = false; // Varyant Ã¶zelliÄŸi olarak Ã¶nerilsin mi (Ã–rn: Renk)

        public int DisplayOrder { get; set; } = 0; // Åablondaki gÃ¶sterim sÄ±rasÄ±
    }
}

``n

## Dosya: AttributeTemplates.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// SÄ°STEM Ã–ZELLÄ°K ÅABLONLARI (yalnÄ±zca admin yÃ¶netimli).
    /// VERSÄ°YONLAMA MODELÄ°:
    ///   - Her sÃ¼rÃ¼m ayrÄ± bir satÄ±rdÄ±r (immutable). YayÄ±nlanan sÃ¼rÃ¼m asla deÄŸiÅŸtirilmez.
    ///   - AynÄ± ÅŸablonun tÃ¼m sÃ¼rÃ¼mleri ortak TemplateGroupId deÄŸerini paylaÅŸÄ±r.
    ///   - Yeni sÃ¼rÃ¼m = aynÄ± TemplateGroupId, Version + 1, yeni AttributeTemplateItems seti.
    ///   - ÃœrÃ¼nler Products.AttributeTemplateId ile SÃœRÃœME sabitlenir; bÃ¶ylece ÅŸablon
    ///     gÃ¼ncellendiÄŸinde mevcut Ã¼rÃ¼nlerin veri bÃ¼tÃ¼nlÃ¼ÄŸÃ¼ bozulmaz.
    ///   - Yeni Ã¼rÃ¼nler her zaman grubun IsPublished = true olan en yÃ¼ksek sÃ¼rÃ¼mÃ¼nÃ¼ kullanÄ±r.
    /// </summary>
    public class AttributeTemplates
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } = Guid.NewGuid(); // SÃ¼rÃ¼m grubu kimliÄŸi (tÃ¼m sÃ¼rÃ¼mlerde aynÄ±)
        public int Version { get; set; } = 1; // SÃ¼rÃ¼m numarasÄ± (1, 2, 3...)

        public string? Name { get; set; } // Åablon adÄ± (Ã–rn: Televizyon)
        public string? Description { get; set; } // Åablon aÃ§Ä±klamasÄ±
        public string? VersionNotes { get; set; } // Bu sÃ¼rÃ¼mde yapÄ±lan deÄŸiÅŸikliklerin notu

        public bool IsPublished { get; set; } = false; // YayÄ±nda mÄ± (yeni Ã¼rÃ¼nlerde kullanÄ±labilir mi)
        public bool IsActive { get; set; } = true; // Grup genelinde aktiflik (false â†’ yeni Ã¼rÃ¼nlere kapalÄ±)

        public Guid? CreatedByUserId { get; set; } // Åablonu oluÅŸturan admin (Users.Id)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? PublishedAt { get; set; } // YayÄ±nlanma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: Brands.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Marka tanÄ±mlarÄ±. UserId/StoreId null ise sistem geneli (admin) markasÄ±dÄ±r,
    /// dolu ise yalnÄ±zca ilgili satÄ±cÄ±ya/maÄŸazaya Ã¶zeldir.
    /// </summary>
    public class Brands
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem markasÄ±, dolu = satÄ±cÄ±ya Ã¶zel marka

        public string? Name { get; set; } // Marka adÄ± (Ã–rn: Samsung)
        public string? Slug { get; set; } // SEO dostu marka kodu (Ã–rn: samsung)

        public bool IsApprovedByAdmin { get; set; } = false; // SatÄ±cÄ± markasÄ± admin onayÄ±ndan geÃ§ti mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: MarketplaceAttributeMappings.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem Ã¶zelliklerinin platform Ã¶zelliklerine eÅŸlenmesi
    /// (Amazon Attribute Mapping, eBay Item Specifics, Trendyol Attributes vb.).
    /// Ä°Ã§eri/dÄ±ÅŸarÄ± aktarÄ±mda veri kaybÄ±nÄ± Ã¶nler: "renk" â†” Trendyol "color" (id: 47) gibi.
    /// EÅŸleme kategoriye Ã¶zel olabilir (platformlar kategori bazlÄ± Ã¶zellik ister).
    /// </summary>
    public class MarketplaceAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid AttributeId { get; set; } // Sistem Ã¶zelliÄŸi (ProductAttributes.Id)
        public int? CategoryId { get; set; } // EÅŸleme kategoriye Ã¶zelse (CategoriesTr.Id), null = genel

        public string? ExternalAttributeCode { get; set; } // Platformdaki Ã¶zellik kodu/ID'si
        public string? ExternalAttributeName { get; set; } // Platformdaki Ã¶zellik adÄ±
        public string? ValueMappingJson { get; set; } // DeÄŸer eÅŸleme sÃ¶zlÃ¼ÄŸÃ¼ (JSON): {"kirmizi":"Red","bordo":"Maroon"}

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: MarketplaceCategoryMappings.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem kategorilerinin platform kategorilerine eÅŸlenmesi.
    /// Ã–rn: CategoriesTr "Televizyon" â†” Trendyol "TV & GÃ¶rÃ¼ntÃ¼ Sistemleri" (id: 1234).
    /// </summary>
    public class MarketplaceCategoryMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public int CategoryId { get; set; } // Sistem kategorisi (CategoriesTr.Id â€” int tip)

        public string? ExternalCategoryCode { get; set; } // Platformdaki kategori kodu/ID'si
        public string? ExternalCategoryName { get; set; } // Platformdaki kategori adÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: Marketplaces.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Entegre platform tanÄ±mlarÄ± (Amazon, Trendyol, Hepsiburada, eBay, Shopify vb.).
    /// Admin yÃ¶netimli, sistem geneli referans tablosudur.
    /// </summary>
    public class Marketplaces
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } // Platform adÄ± (Ã–rn: Trendyol)
        public string? Code { get; set; } // Teknik kod (Ã–rn: trendyol, amazon, n11) â€” entegrasyon katmanÄ±nda anahtar
        public string? ApiBaseUrl { get; set; } // API kÃ¶k adresi
        public Guid? LogoMediaId { get; set; } // Platform logosu (Media.Id)

        public bool IsActive { get; set; } = true; // Entegrasyon aktif mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: ProductAttributeMappings.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir Ã¼rÃ¼nÃ¼n hangi Ã¶zellikleri, hangi rollerde kullandÄ±ÄŸÄ±nÄ± tanÄ±mlar.
    /// VARYANT GÃ–RSEL YÃ–NETÄ°MÄ°NÄ°N KALBÄ° BURASIDIR:
    ///   IsVariantAttribute      â†’ bu Ã¶zellik varyant kombinasyonu Ã¼retir (Ã–rn: Renk, Beden)
    ///   IsImageVariantAttribute â†’ bu Ã¶zelliÄŸin DEÄERLERÄ°NE gÃ¶rsel grubu atanÄ±r (Ã–rn: yalnÄ±zca Renk)
    /// Bu seÃ§im ÃœRÃœN BAZINDA yapÄ±lÄ±r: bir Ã¼rÃ¼nde Renk gÃ¶rsel varyantÄ± iken,
    /// baÅŸka bir Ã¼rÃ¼nde Beden gÃ¶rsel varyantÄ± olabilir.
    /// </summary>
    public class ProductAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid AttributeId { get; set; } // Ã–zellik (ProductAttributes.Id)

        public bool IsVariantAttribute { get; set; } = false; // Varyant Ã¼reten Ã¶zellik mi (Renk, Beden vb.)
        public bool IsImageVariantAttribute { get; set; } = false; // GÃ¶rsel grubu bu Ã¶zelliÄŸin deÄŸerlerine mi baÄŸlanacak
        public bool IsRequired { get; set; } = false; // ÃœrÃ¼n kaydÄ±nda zorunlu mu

        public Guid? SourceTemplateId { get; set; } // Ã–zellik bir ÅŸablondan geldiyse ÅŸablon sÃ¼rÃ¼mÃ¼ (AttributeTemplates.Id), satÄ±cÄ± manuel eklediyse null

        public int DisplayOrder { get; set; } = 0; // ÃœrÃ¼n detayÄ±nda gÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductAttributes.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ã–zellik (Attribute) tanÄ±mlarÄ± â€” tamamen dinamik Ã¶zellik sistemi.
    /// KRÄ°TÄ°K SAHÄ°PLÄ°K KURALI:
    ///   UserId = null ve StoreId = null â†’ SÄ°STEM Ã¶zelliÄŸi (yalnÄ±zca admin oluÅŸturur/dÃ¼zenler, ÅŸablonlarda kullanÄ±labilir)
    ///   UserId/StoreId dolu             â†’ SATICIYA Ã–ZEL Ã¶zellik (yalnÄ±zca o satÄ±cÄ±nÄ±n Ã¼rÃ¼nlerinde gÃ¶rÃ¼nÃ¼r,
    ///                                      asla ÅŸablonlara eklenmez, diÄŸer satÄ±cÄ±lara gÃ¶sterilmez)
    /// </summary>
    public class ProductAttributes
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem Ã¶zelliÄŸi, dolu = satÄ±cÄ±ya Ã¶zel
        public Guid? StoreId { get; set; } // Null = sistem Ã¶zelliÄŸi, dolu = maÄŸazaya Ã¶zel

        public string? Name { get; set; } // Ã–zellik adÄ± (Ã–rn: Renk, RAM, Ekran Boyutu)
        public string? Code { get; set; } // Teknik kod / slug (Ã–rn: renk, ram) â€” entegrasyon eÅŸlemede kullanÄ±lÄ±r
        public string? Description { get; set; } // Ã–zellik aÃ§Ä±klamasÄ±

        // GiriÅŸ tipi: "Select", "MultiSelect", "Text", "Number", "Bool", "Date"
        public string? InputType { get; set; } = "Select";

        public string? Unit { get; set; } // Birim (Ã–rn: GB, inÃ§, Hz) â€” Number tipinde kullanÄ±ÅŸlÄ±
        public bool IsFilterable { get; set; } = false; // ÃœrÃ¼n listelerinde filtre olarak gÃ¶sterilsin mi
        public bool IsActive { get; set; } = true; // Ã–zellik kullanÄ±mda mÄ±
        public int DisplayOrder { get; set; } = 0; // Listeleme sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: ProductAttributeValues.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ã–zelliklerin Ã¶nceden tanÄ±mlÄ± deÄŸerleri (Ã–rn: Renk â†’ KÄ±rmÄ±zÄ±, Bordo, Siyah).
    /// Sahiplik kuralÄ± ProductAttributes ile aynÄ±dÄ±r:
    ///   UserId/StoreId null â†’ sistem deÄŸeri, dolu â†’ yalnÄ±zca o satÄ±cÄ±da gÃ¶rÃ¼nen Ã¶zel deÄŸer.
    /// BÃ¶ylece sistem Ã¶zelliÄŸi olan "Renk"e bir satÄ±cÄ± kendi Ã¶zel rengini ekleyebilir
    /// ama bu deÄŸer diÄŸer satÄ±cÄ±lara yansÄ±maz.
    /// </summary>
    public class ProductAttributeValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeId { get; set; } // BaÄŸlÄ± olduÄŸu Ã¶zellik (ProductAttributes.Id)

        public Guid? UserId { get; set; } // Null = sistem deÄŸeri, dolu = satÄ±cÄ±ya Ã¶zel deÄŸer
        public Guid? StoreId { get; set; } // Null = sistem deÄŸeri, dolu = maÄŸazaya Ã¶zel deÄŸer

        public string? Value { get; set; } // DeÄŸer metni (Ã–rn: KÄ±rmÄ±zÄ±, 16 GB, 55")
        public string? Code { get; set; } // Teknik kod (Ã–rn: kirmizi) â€” entegrasyon eÅŸlemede kullanÄ±lÄ±r
        public string? ColorHex { get; set; } // Renk Ã¶zellikleri iÃ§in HEX kodu (Ã–rn: #FF0000) â€” UI renk kutucuÄŸu
        public int DisplayOrder { get; set; } = 0; // Listeleme sÄ±rasÄ±
        public bool IsActive { get; set; } = true; // DeÄŸer kullanÄ±mda mÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: ProductBundleItems.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Paket/Bundle Ã¼rÃ¼n iÃ§erik tablosu.
    /// ProductType = "Bundle" olan Ã¼rÃ¼nÃ¼n hangi alt Ã¼rÃ¼n/varyantlardan oluÅŸtuÄŸunu tutar.
    /// </summary>
    public class ProductBundleItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BundleProductId { get; set; } // Paket Ã¼rÃ¼n (Products.Id, ProductType = "Bundle")
        public Guid ChildProductId { get; set; } // Pakete dahil Ã¼rÃ¼n (Products.Id)
        public Guid? ChildVariantId { get; set; } // Belirli bir varyant zorunluysa (ProductVariants.Id), null = mÃ¼ÅŸteri seÃ§er

        public int Quantity { get; set; } = 1; // Paketteki adet
        public decimal? DiscountRate { get; set; } // Paket iÃ§i indirim oranÄ± (%) â€” opsiyonel

        public int DisplayOrder { get; set; } = 0; // Paket iÃ§eriÄŸi gÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductCategories.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n - Kategori Ã§oklu iliÅŸki tablosu.
    /// Bir Ã¼rÃ¼n birden fazla kategoriye baÄŸlanabilir; IsPrimary = true olan kayÄ±t ana kategoridir.
    /// </summary>
    public class ProductCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id â€” int tip)

        public bool IsPrimary { get; set; } = false; // Ana kategori mi (Ã¼rÃ¼n baÅŸÄ±na yalnÄ±zca 1 adet true olmalÄ±)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductDigitalAssets.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Dijital Ã¼rÃ¼n dosyalarÄ± (ProductType = "Digital").
    /// SatÄ±n alma sonrasÄ± indirilebilir dosyalar ve indirme kurallarÄ± burada tutulur.
    /// </summary>
    public class ProductDigitalAssets
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid? VariantId { get; set; } // Dosya belirli bir varyanta Ã¶zelse (ProductVariants.Id)

        public Guid MediaId { get; set; } // Ä°ndirilebilir dosya (Media.Id)
        public string? DisplayName { get; set; } // MÃ¼ÅŸteriye gÃ¶sterilecek dosya adÄ±

        public int? DownloadLimit { get; set; } // Maksimum indirme sayÄ±sÄ± (null = sÄ±nÄ±rsÄ±z)
        public int? ExpirationDays { get; set; } // SatÄ±n alma sonrasÄ± geÃ§erlilik sÃ¼resi - gÃ¼n (null = sÃ¼resiz)
        public bool RequiresLicenseKey { get; set; } = false; // Lisans anahtarÄ± Ã¼retilsin mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductExternalMedias.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼nÃ¼n galeri dÄ±ÅŸÄ± medya kaynaklarÄ±: video URL'leri, 360 derece gÃ¶rÃ¼nÃ¼m,
    /// embed iÃ§erikler ve PDF/dokÃ¼man dosyalarÄ±.
    /// Sistemde yÃ¼klÃ¼ dosyalar MediaId ile (Media.Id), harici kaynaklar Url ile baÄŸlanÄ±r.
    /// </summary>
    public class ProductExternalMedias
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)

        // "VideoUrl", "View360", "Embed", "Document"
        public string? MediaType { get; set; } = "VideoUrl";

        public string? Url { get; set; } // Harici kaynak adresi (YouTube, Vimeo, 360 viewer vb.)
        public Guid? MediaId { get; set; } // Sistemde yÃ¼klÃ¼ dosya (Media.Id) â€” PDF/dokÃ¼man iÃ§in

        public string? Title { get; set; } // GÃ¶sterim baÅŸlÄ±ÄŸÄ± (Ã–rn: "Kurulum KÄ±lavuzu")
        public int DisplayOrder { get; set; } = 0; // GÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductImageVariantGroups.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant gÃ¶rsel gruplarÄ±.
    /// GÃ¶rseller varyant kombinasyonuna DEÄÄ°L, gÃ¶rsel-varyant Ã¶zelliÄŸinin DEÄERÄ°NE baÄŸlanÄ±r:
    ///   Ã–rn: Renk gÃ¶rsel varyantÄ± ise â†’ KÄ±rmÄ±zÄ± iÃ§in 1 grup, Bordo iÃ§in 1 grup, Siyah iÃ§in 1 grup.
    ///   KÄ±rmÄ±zÄ±-S ve KÄ±rmÄ±zÄ±-M aynÄ± (KÄ±rmÄ±zÄ±) gÃ¶rsel grubunu paylaÅŸÄ±r.
    /// GÃ¶rseller ItemGallery Ã¼zerinden baÄŸlanÄ±r:
    ///   ItemGallery.ItemType = "VariantGallery", ItemGallery.ItemId = ProductImageVariantGroups.Id
    /// Bir gruba birden fazla gÃ¶rsel eklenebilir (ItemGallery zaten Ã§oklu kayÄ±t destekler).
    /// </summary>
    public class ProductImageVariantGroups
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid AttributeId { get; set; } // GÃ¶rsel varyant Ã¶zelliÄŸi (ProductAttributes.Id â€” Ã–rn: Renk)
        public Guid AttributeValueId { get; set; } // GÃ¶rselin baÄŸlandÄ±ÄŸÄ± deÄŸer (ProductAttributeValues.Id â€” Ã–rn: KÄ±rmÄ±zÄ±)

        public Guid? CoverMediaId { get; set; } // Bu deÄŸerin kapak gÃ¶rseli (Media.Id) â€” listelemede gÃ¶sterilir

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductImportProfiles.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// CSV / XML / JSON / API iÃ§e aktarÄ±m profilleri.
    /// SatÄ±cÄ±, kaynak dosya kolonlarÄ±nÄ±n sistem alanlarÄ±na nasÄ±l eÅŸleneceÄŸini bir kez tanÄ±mlar
    /// (FieldMappingJson), sonraki aktarÄ±mlarda profili yeniden kullanÄ±r.
    /// Trendyol, Amazon, WooCommerce, N11 gibi hazÄ±r kaynak tipleri desteklenir.
    /// </summary>
    public class ProductImportProfiles
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Profili oluÅŸturan kullanÄ±cÄ± (Users.Id)
        public Guid StoreId { get; set; } // BaÄŸlÄ± maÄŸaza (Store.Id)

        public string? Name { get; set; } // Profil adÄ± (Ã–rn: "Trendyol AylÄ±k AktarÄ±m")

        // "CSV", "XML", "JSON", "RestApi", "SoapApi", "Amazon", "Alibaba", "Ebay", "MercadoLibre",
        // "Jd", "Trendyol", "Hepsiburada", "N11", "CicekSepeti", "Etsy", "Shopify", "WooCommerce"
        public string? SourceType { get; set; } = "CSV";

        public string? FieldMappingJson { get; set; } // Kolon â†’ alan eÅŸleme tanÄ±mÄ± (JSON)
        public string? Delimiter { get; set; } = ";"; // CSV ayÄ±racÄ±
        public string? Encoding { get; set; } = "UTF-8"; // Dosya karakter seti
        public string? SourceUrl { get; set; } // API/XML feed adresi (dosya dÄ±ÅŸÄ± kaynaklarda)

        public Guid? DefaultWarehouseId { get; set; } // AktarÄ±lan Ã¼rÃ¼nlerin varsayÄ±lan deposu (Warehouse.Id)
        public int? DefaultCategoryId { get; set; } // EÅŸlenemeyen Ã¼rÃ¼nler iÃ§in varsayÄ±lan kategori (CategoriesTr.Id)

        public bool IsActive { get; set; } = true; // Profil kullanÄ±mda mÄ±
        public DateTime? LastRunDate { get; set; } // Son Ã§alÄ±ÅŸtÄ±rma tarihi
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: ProductMarketplaceListings.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n/varyantÄ±n dÄ±ÅŸ platformlardaki karÅŸÄ±lÄ±ÄŸÄ± ve senkronizasyon durumu.
    /// Ä°Ã§e aktarÄ±mda platformdan gelen HAM VERÄ° RawSourceData alanÄ±nda JSON olarak saklanÄ±r â€”
    /// bÃ¶ylece modelimizde karÅŸÄ±lÄ±ÄŸÄ± olmayan alanlar dahi VERÄ° KAYBI olmadan korunur
    /// ve ileride yeniden iÅŸlenebilir.
    /// </summary>
    public class ProductMarketplaceListings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid? VariantId { get; set; } // Varyant bazlÄ± eÅŸleme gerekiyorsa (ProductVariants.Id)
        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid StoreId { get; set; } // MaÄŸaza (Store.Id) â€” hangi maÄŸazanÄ±n entegrasyonu

        public string? ExternalProductId { get; set; } // Platformdaki Ã¼rÃ¼n ID'si (Ã–rn: Amazon ASIN, Trendyol contentId)
        public string? ExternalSku { get; set; } // Platformdaki SKU (merchantSku vb.)
        public string? ExternalUrl { get; set; } // Platformdaki Ã¼rÃ¼n sayfasÄ± adresi

        // "Pending", "Synced", "Error", "Paused"
        public string? SyncStatus { get; set; } = "Pending";
        public DateTime? LastSyncDate { get; set; } // Son baÅŸarÄ±lÄ± senkronizasyon tarihi
        public string? LastErrorMessage { get; set; } // Son hata mesajÄ±

        public string? RawSourceData { get; set; } // Ä°Ã§e aktarÄ±mda gelen ham kaynak verisi (JSON) â€” veri kaybÄ± sigortasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductPrices.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant bazlÄ±, Ã§oklu para birimli fiyat tablosu (APPEND-ONLY tarih geÃ§miÅŸi).
    /// Her para birimi (TRY/USD/EUR/AZN) iÃ§in ayrÄ± satÄ±r tutulur â€” kur dÃ¶nÃ¼ÅŸÃ¼mÃ¼ YAPILMAZ,
    /// deÄŸerler baÄŸÄ±msÄ±z olarak veri tabanÄ±nda saklanÄ±r.
    /// Fiyat deÄŸiÅŸiminde eski satÄ±r gÃ¼ncellenmez: EffectiveTo kapatÄ±lÄ±r, yeni satÄ±r aÃ§Ä±lÄ±r.
    /// GÃ¼ncel fiyat sorgusu: EffectiveTo == null olan satÄ±r.
    /// </summary>
    public class ProductPrices
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id) â€” raporlama kolaylÄ±ÄŸÄ± iÃ§in denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id) â€” basit Ã¼rÃ¼nde varsayÄ±lan varyant

        public string? Currency { get; set; } = "TRY"; // Para birimi ("TRY", "USD", "EUR", "AZN")

        public decimal Price { get; set; } // Normal satÄ±ÅŸ fiyatÄ±
        public decimal? DiscountedPrice { get; set; } // Ä°ndirimli satÄ±ÅŸ fiyatÄ± (kampanya) â€” null ise indirim yok
        public decimal? CostPrice { get; set; } // Maliyet fiyatÄ± (yalnÄ±zca satÄ±cÄ± gÃ¶rÃ¼r)

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow; // GeÃ§erlilik baÅŸlangÄ±cÄ±
        public DateTime? EffectiveTo { get; set; } // GeÃ§erlilik bitiÅŸi (null = gÃ¼ncel/aktif fiyat)

        public Guid? CreatedByUserId { get; set; } // FiyatÄ± giren kullanÄ±cÄ± (denetim iÃ§in)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // KayÄ±t tarihi
    }
}

``n

## Dosya: Products.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ana Ã¼rÃ¼n tablosu. TÃ¼m Ã¼rÃ¼n tipleri (basit, varyantlÄ±, dijital, hizmet, paket, harici)
    /// bu tablo Ã¼zerinden yÃ¶netilir. Basit Ã¼rÃ¼nler dahi en az bir ProductVariants kaydÄ±na sahiptir
    /// (birleÅŸik tek-varyant modeli) â€” bÃ¶ylece fiyat/stok her zaman varyant Ã¼zerinden okunur.
    /// </summary>
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === Ä°liÅŸkiler (ID bazlÄ±, navigation property kullanÄ±lmaz) ===
        public Guid UserId { get; set; } // ÃœrÃ¼n sahibi kullanÄ±cÄ± (Users.Id)
        public Guid StoreId { get; set; } // ÃœrÃ¼n sahibi maÄŸaza (Store.Id)
        public Guid? BrandId { get; set; } // Marka (Brands.Id)
        public Guid? AttributeTemplateId { get; set; } // ÃœrÃ¼nÃ¼n oluÅŸturulduÄŸu Ã¶zellik ÅŸablonu SÃœRÃœMÃœ (AttributeTemplates.Id) â€” versiyon sabitlenir, ÅŸablon gÃ¼ncellense bile Ã¼rÃ¼n verisi bozulmaz
        public Guid? ShippingProfileId { get; set; } // Kargo/desi profili (ShippingProfile.Id)

        // === ÃœrÃ¼n Tipi ===
        // "Simple", "Variant", "Digital", "Service", "Bundle", "External"
        // Yeni tipler iÃ§in enum yerine string tercih edildi (geniÅŸletilebilirlik)
        public string? ProductType { get; set; } = "Simple";

        // === Ä°Ã§erik AlanlarÄ± ===
        public string? Name { get; set; } // ÃœrÃ¼n adÄ±
        public string? ShortDescription { get; set; } // KÄ±sa aÃ§Ä±klama
        public string? FullDescription { get; set; } // DetaylÄ± aÃ§Ä±klama (HTML destekli)
        public string? Tags { get; set; } // Etiketler (virgÃ¼lle ayrÄ±lmÄ±ÅŸ)

        // === Medya ===
        public Guid? CoverMediaId { get; set; } // Kapak gÃ¶rseli (Media.Id)
        // Galeri gÃ¶rselleri: ItemGallery (ItemType = "ProductGallery", ItemId = Products.Id)
        // Varyant gÃ¶rselleri: ItemGallery (ItemType = "VariantGallery", ItemId = ProductImageVariantGroups.Id)

        // === Harici ÃœrÃ¼n (External) AlanlarÄ± ===
        public string? ExternalUrl { get; set; } // Harici Ã¼rÃ¼n baÄŸlantÄ±sÄ±
        public string? ExternalButtonText { get; set; } // Harici Ã¼rÃ¼n buton metni (Ã–rn: "SatÄ±cÄ± sitesinde gÃ¶r")

        // === YayÄ±n Durumu ===
        // "Draft", "PendingApproval", "Published", "Rejected", "Archived"
        public string? PublishStatus { get; set; } = "Draft";
        public bool? IsApprovedByAdmin { get; set; } // Admin onay durumu
        public DateTime? ApprovedDate { get; set; } // Admin onay tarihi
        public Guid? ApprovedByUserId { get; set; } // Onaylayan admin (Users.Id)
        public bool IsActive { get; set; } = true; // SatÄ±cÄ± tarafÄ±ndan aktif/pasif

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son gÃ¼ncelleme tarihi

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}

``n

## Dosya: ProductSeo.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n SEO bilgileri. Products tablosundan ayrÄ± tutulur â€” listeleme sorgularÄ±nÄ±
    /// ÅŸiÅŸirmez ve ileride dil bazlÄ± SEO satÄ±rlarÄ±na geniÅŸletilebilir.
    /// </summary>
    public class ProductSeo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)

        public string? Slug { get; set; } // SEO dostu URL (Ã–rn: samsung-55-oled-tv) â€” benzersiz olmalÄ±
        public string? SeoTitle { get; set; } // SEO baÅŸlÄ±k (meta title)
        public string? SeoDescription { get; set; } // SEO aÃ§Ä±klama (meta description)
        public string? SeoKeywords { get; set; } // SEO anahtar kelimeler (virgÃ¼lle ayrÄ±lmÄ±ÅŸ)
        public string? CanonicalUrl { get; set; } // Canonical adres (kopya iÃ§erik Ã¶nleme)
        public string? MetaJson { get; set; } // Ek meta veriler (JSON: OpenGraph, schema.org vb.)

        public string? LanguageCode { get; set; } = "tr"; // Dil kodu â€” ileride Ã§oklu dil SEO satÄ±rlarÄ± iÃ§in

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son gÃ¼ncelleme tarihi
    }
}

``n

## Dosya: ProductSpecifications.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼nÃ¼n teknik Ã¶zellik/spesifikasyon DEÄERLERÄ° (varyant Ã¼retmeyen bilgi alanlarÄ±).
    /// Ä°ki kullanÄ±m ÅŸekli desteklenir:
    ///   1) TanÄ±mlÄ± Ã¶zellik: AttributeId (+ AttributeValueId veya serbest CustomValue)
    ///      Ã–rn: Ekran Boyutu â†’ 55"
    ///   2) Tamamen serbest satÄ±r: CustomName + CustomValue (AttributeId null)
    ///      Ã–rn: "Kutu Ä°Ã§eriÄŸi" â†’ "TV, Kumanda, Duvar AparatÄ±" (yalnÄ±zca bu Ã¼rÃ¼nde geÃ§erli)
    /// </summary>
    public class ProductSpecifications
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)

        public Guid? AttributeId { get; set; } // TanÄ±mlÄ± Ã¶zellik (ProductAttributes.Id) â€” serbest satÄ±rda null
        public Guid? AttributeValueId { get; set; } // Ã–nceden tanÄ±mlÄ± deÄŸer seÃ§ildiyse (ProductAttributeValues.Id)

        public string? CustomName { get; set; } // Serbest Ã¶zellik adÄ± (AttributeId null ise kullanÄ±lÄ±r)
        public string? CustomValue { get; set; } // Serbest/metinsel deÄŸer (Text-Number tipli Ã¶zelliklerde de kullanÄ±lÄ±r)

        public int DisplayOrder { get; set; } = 0; // GÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}

``n

## Dosya: ProductStocks.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant + Depo bazlÄ± stok tablosu.
    /// Bir varyant birden fazla depoda tutulabilir (Ã§oklu depo desteÄŸi);
    /// toplam stok = varyantÄ±n tÃ¼m depo satÄ±rlarÄ±nÄ±n toplamÄ±dÄ±r.
    /// </summary>
    public class ProductStocks
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id) â€” denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid WarehouseId { get; set; } // Depo (Warehouse.Id)

        public bool TrackStock { get; set; } = true; // Stok takibi yapÄ±lsÄ±n mÄ± (dijital/hizmet Ã¼rÃ¼nlerde false)

        public int Quantity { get; set; } = 0; // Mevcut stok adedi
        public int? MinStockLevel { get; set; } // Minimum stok miktarÄ±
        public int? CriticalStockLevel { get; set; } // Kritik stok seviyesi (uyarÄ± eÅŸiÄŸi)
        public int? MaxOrderQuantity { get; set; } // Tek sipariÅŸte alÄ±nabilecek maksimum adet

        // "InStock", "OutOfStock", "PreOrder", "Backorder"
        public string? StockStatus { get; set; } = "InStock";

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow; // Son stok gÃ¼ncelleme tarihi
    }
}

``n

## Dosya: ProductVariants.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n varyantlarÄ±. BirleÅŸik tek-varyant modeli gereÄŸi basit Ã¼rÃ¼nlerde de
    /// IsDefault = true olan tek bir varyant kaydÄ± bulunur.
    /// Fiyat (ProductPrices) ve stok (ProductStocks) daima varyant Ã¼zerinden tutulur.
    /// </summary>
    public class ProductVariants
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // BaÄŸlÄ± olduÄŸu Ã¼rÃ¼n (Products.Id)

        // === Kimlik / TanÄ±mlayÄ±cÄ± Alanlar (Marketplace entegrasyon standartlarÄ±) ===
        public string? Sku { get; set; } // Stok kodu (Ã–rn: KZK-KRM-S)
        public string? Barcode { get; set; } // Barkod
        public string? Gtin { get; set; } // Global Trade Item Number
        public string? Upc { get; set; } // Universal Product Code
        public string? Ean { get; set; } // European Article Number
        public string? Isbn { get; set; } // Kitaplar iÃ§in ISBN
        public string? Mpn { get; set; } // Manufacturer Part Number (Ãœretici parÃ§a numarasÄ±)

        // === Durum ===
        public bool IsDefault { get; set; } = false; // VarsayÄ±lan varyant mÄ± (basit Ã¼rÃ¼nde tek kayÄ±t true)
        public bool IsActive { get; set; } = true; // Varyant satÄ±ÅŸta mÄ±
        public int DisplayOrder { get; set; } = 0; // Listeleme sÄ±rasÄ±

        // === Fiziksel Ã–zellikler (kargo/desi hesabÄ± ShippingProfile bÃ¶len deÄŸerleri ile yapÄ±lÄ±r) ===
        public decimal? WeightKg { get; set; } // AÄŸÄ±rlÄ±k (kg)
        public decimal? WidthCm { get; set; } // GeniÅŸlik (cm)
        public decimal? HeightCm { get; set; } // YÃ¼kseklik (cm)
        public decimal? LengthCm { get; set; } // Uzunluk (cm)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son gÃ¼ncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n

## Dosya: ProductVariantValues.cs

`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant kombinasyon tablosu. Her varyantÄ±n hangi Ã¶zellik-deÄŸer ikililerinden
    /// oluÅŸtuÄŸunu tutar. Ã–rn: KZK-KRM-S varyantÄ± iÃ§in iki satÄ±r:
    ///   (VariantId, Renk, KÄ±rmÄ±zÄ±) ve (VariantId, Beden, S)
    /// </summary>
    public class ProductVariantValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid AttributeId { get; set; } // Ã–zellik (ProductAttributes.Id) â€” sorgu kolaylÄ±ÄŸÄ± iÃ§in denormalize
        public Guid AttributeValueId { get; set; } // Ã–zellik deÄŸeri (ProductAttributeValues.Id)
    }
}

``n

## Dosya: Warehouse.cs

`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Depo/lojistik noktasÄ± tanÄ±mÄ±. MaÄŸaza bazlÄ±dÄ±r.
    /// Adres bilgisi iÃ§in mevcut AddressInfo owned type'Ä± yeniden kullanÄ±lÄ±r.
    /// </summary>
    public class Warehouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Depoyu ekleyen kullanÄ±cÄ±nÄ±n/satÄ±cÄ±nÄ±n ID bilgisi
        public Guid StoreId { get; set; } // Deponun baÄŸlÄ± olduÄŸu maÄŸazanÄ±n ID bilgisi

        public bool IsDefault { get; set; } = false; // VarsayÄ±lan depo mu
        public bool IsActive { get; set; } = true; // Depo aktif mi

        public AddressInfo? AddressInfo { get; set; } = new(); // Depo adres bilgisi (mevcut owned type)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}

``n
## Dosya: AttributeTemplateCategories.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Åablon - Kategori atama tablosu.
    /// TemplateGroupId Ã¼zerinden baÄŸlanÄ±r (sÃ¼rÃ¼m deÄŸil) â€” bÃ¶ylece yeni sÃ¼rÃ¼m yayÄ±nlandÄ±ÄŸÄ±nda
    /// kategori atamalarÄ±nÄ± tekrar yapmaya gerek kalmaz; kategori her zaman grubun
    /// yayÄ±ndaki en gÃ¼ncel sÃ¼rÃ¼mÃ¼nÃ¼ sunar. Bir ÅŸablon birden fazla kategoriye atanabilir.
    /// </summary>
    public class AttributeTemplateCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } // Åablon grubu (AttributeTemplates.TemplateGroupId)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id â€” int tip)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: AttributeTemplateItems.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Åablon sÃ¼rÃ¼mÃ¼ ile sistem Ã¶zellikleri arasÄ±ndaki baÄŸlantÄ±.
    /// YalnÄ±zca SÄ°STEM Ã¶zellikleri (ProductAttributes.UserId == null) eklenebilir â€”
    /// satÄ±cÄ± Ã¶zel Ã¶zellikleri ÅŸablonlara ASLA dahil edilmez (uygulama katmanÄ±nda doÄŸrulanÄ±r).
    /// </summary>
    public class AttributeTemplateItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateId { get; set; } // Åablon SÃœRÃœMÃœ (AttributeTemplates.Id)
        public Guid AttributeId { get; set; } // Sistem Ã¶zelliÄŸi (ProductAttributes.Id)

        public bool IsRequired { get; set; } = false; // Bu ÅŸablonda zorunlu mu
        public bool IsVariantSuggested { get; set; } = false; // Varyant Ã¶zelliÄŸi olarak Ã¶nerilsin mi (Ã–rn: Renk)

        public int DisplayOrder { get; set; } = 0; // Åablondaki gÃ¶sterim sÄ±rasÄ±
    }
}
` 
--- 

## Dosya: AttributeTemplates.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// SÄ°STEM Ã–ZELLÄ°K ÅABLONLARI (yalnÄ±zca admin yÃ¶netimli).
    /// VERSÄ°YONLAMA MODELÄ°:
    ///   - Her sÃ¼rÃ¼m ayrÄ± bir satÄ±rdÄ±r (immutable). YayÄ±nlanan sÃ¼rÃ¼m asla deÄŸiÅŸtirilmez.
    ///   - AynÄ± ÅŸablonun tÃ¼m sÃ¼rÃ¼mleri ortak TemplateGroupId deÄŸerini paylaÅŸÄ±r.
    ///   - Yeni sÃ¼rÃ¼m = aynÄ± TemplateGroupId, Version + 1, yeni AttributeTemplateItems seti.
    ///   - ÃœrÃ¼nler Products.AttributeTemplateId ile SÃœRÃœME sabitlenir; bÃ¶ylece ÅŸablon
    ///     gÃ¼ncellendiÄŸinde mevcut Ã¼rÃ¼nlerin veri bÃ¼tÃ¼nlÃ¼ÄŸÃ¼ bozulmaz.
    ///   - Yeni Ã¼rÃ¼nler her zaman grubun IsPublished = true olan en yÃ¼ksek sÃ¼rÃ¼mÃ¼nÃ¼ kullanÄ±r.
    /// </summary>
    public class AttributeTemplates
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } = Guid.NewGuid(); // SÃ¼rÃ¼m grubu kimliÄŸi (tÃ¼m sÃ¼rÃ¼mlerde aynÄ±)
        public int Version { get; set; } = 1; // SÃ¼rÃ¼m numarasÄ± (1, 2, 3...)

        public string? Name { get; set; } // Åablon adÄ± (Ã–rn: Televizyon)
        public string? Description { get; set; } // Åablon aÃ§Ä±klamasÄ±
        public string? VersionNotes { get; set; } // Bu sÃ¼rÃ¼mde yapÄ±lan deÄŸiÅŸikliklerin notu

        public bool IsPublished { get; set; } = false; // YayÄ±nda mÄ± (yeni Ã¼rÃ¼nlerde kullanÄ±labilir mi)
        public bool IsActive { get; set; } = true; // Grup genelinde aktiflik (false â†’ yeni Ã¼rÃ¼nlere kapalÄ±)

        public Guid? CreatedByUserId { get; set; } // Åablonu oluÅŸturan admin (Users.Id)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? PublishedAt { get; set; } // YayÄ±nlanma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: Brands.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Marka tanÄ±mlarÄ±. UserId/StoreId null ise sistem geneli (admin) markasÄ±dÄ±r,
    /// dolu ise yalnÄ±zca ilgili satÄ±cÄ±ya/maÄŸazaya Ã¶zeldir.
    /// </summary>
    public class Brands
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem markasÄ±, dolu = satÄ±cÄ±ya Ã¶zel marka

        public string? Name { get; set; } // Marka adÄ± (Ã–rn: Samsung)
        public string? Slug { get; set; } // SEO dostu marka kodu (Ã–rn: samsung)

        public bool IsApprovedByAdmin { get; set; } = false; // SatÄ±cÄ± markasÄ± admin onayÄ±ndan geÃ§ti mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: MarketplaceAttributeMappings.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem Ã¶zelliklerinin platform Ã¶zelliklerine eÅŸlenmesi
    /// (Amazon Attribute Mapping, eBay Item Specifics, Trendyol Attributes vb.).
    /// Ä°Ã§eri/dÄ±ÅŸarÄ± aktarÄ±mda veri kaybÄ±nÄ± Ã¶nler: "renk" â†” Trendyol "color" (id: 47) gibi.
    /// EÅŸleme kategoriye Ã¶zel olabilir (platformlar kategori bazlÄ± Ã¶zellik ister).
    /// </summary>
    public class MarketplaceAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid AttributeId { get; set; } // Sistem Ã¶zelliÄŸi (ProductAttributes.Id)
        public int? CategoryId { get; set; } // EÅŸleme kategoriye Ã¶zelse (CategoriesTr.Id), null = genel

        public string? ExternalAttributeCode { get; set; } // Platformdaki Ã¶zellik kodu/ID'si
        public string? ExternalAttributeName { get; set; } // Platformdaki Ã¶zellik adÄ±
        public string? ValueMappingJson { get; set; } // DeÄŸer eÅŸleme sÃ¶zlÃ¼ÄŸÃ¼ (JSON): {"kirmizi":"Red","bordo":"Maroon"}

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: MarketplaceCategoryMappings.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem kategorilerinin platform kategorilerine eÅŸlenmesi.
    /// Ã–rn: CategoriesTr "Televizyon" â†” Trendyol "TV & GÃ¶rÃ¼ntÃ¼ Sistemleri" (id: 1234).
    /// </summary>
    public class MarketplaceCategoryMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public int CategoryId { get; set; } // Sistem kategorisi (CategoriesTr.Id â€” int tip)

        public string? ExternalCategoryCode { get; set; } // Platformdaki kategori kodu/ID'si
        public string? ExternalCategoryName { get; set; } // Platformdaki kategori adÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: Marketplaces.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Entegre platform tanÄ±mlarÄ± (Amazon, Trendyol, Hepsiburada, eBay, Shopify vb.).
    /// Admin yÃ¶netimli, sistem geneli referans tablosudur.
    /// </summary>
    public class Marketplaces
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } // Platform adÄ± (Ã–rn: Trendyol)
        public string? Code { get; set; } // Teknik kod (Ã–rn: trendyol, amazon, n11) â€” entegrasyon katmanÄ±nda anahtar
        public string? ApiBaseUrl { get; set; } // API kÃ¶k adresi
        public Guid? LogoMediaId { get; set; } // Platform logosu (Media.Id)

        public bool IsActive { get; set; } = true; // Entegrasyon aktif mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: ProductAttributeMappings.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir Ã¼rÃ¼nÃ¼n hangi Ã¶zellikleri, hangi rollerde kullandÄ±ÄŸÄ±nÄ± tanÄ±mlar.
    /// VARYANT GÃ–RSEL YÃ–NETÄ°MÄ°NÄ°N KALBÄ° BURASIDIR:
    ///   IsVariantAttribute      â†’ bu Ã¶zellik varyant kombinasyonu Ã¼retir (Ã–rn: Renk, Beden)
    ///   IsImageVariantAttribute â†’ bu Ã¶zelliÄŸin DEÄERLERÄ°NE gÃ¶rsel grubu atanÄ±r (Ã–rn: yalnÄ±zca Renk)
    /// Bu seÃ§im ÃœRÃœN BAZINDA yapÄ±lÄ±r: bir Ã¼rÃ¼nde Renk gÃ¶rsel varyantÄ± iken,
    /// baÅŸka bir Ã¼rÃ¼nde Beden gÃ¶rsel varyantÄ± olabilir.
    /// </summary>
    public class ProductAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid AttributeId { get; set; } // Ã–zellik (ProductAttributes.Id)

        public bool IsVariantAttribute { get; set; } = false; // Varyant Ã¼reten Ã¶zellik mi (Renk, Beden vb.)
        public bool IsImageVariantAttribute { get; set; } = false; // GÃ¶rsel grubu bu Ã¶zelliÄŸin deÄŸerlerine mi baÄŸlanacak
        public bool IsRequired { get; set; } = false; // ÃœrÃ¼n kaydÄ±nda zorunlu mu

        public Guid? SourceTemplateId { get; set; } // Ã–zellik bir ÅŸablondan geldiyse ÅŸablon sÃ¼rÃ¼mÃ¼ (AttributeTemplates.Id), satÄ±cÄ± manuel eklediyse null

        public int DisplayOrder { get; set; } = 0; // ÃœrÃ¼n detayÄ±nda gÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductAttributes.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ã–zellik (Attribute) tanÄ±mlarÄ± â€” tamamen dinamik Ã¶zellik sistemi.
    /// KRÄ°TÄ°K SAHÄ°PLÄ°K KURALI:
    ///   UserId = null ve StoreId = null â†’ SÄ°STEM Ã¶zelliÄŸi (yalnÄ±zca admin oluÅŸturur/dÃ¼zenler, ÅŸablonlarda kullanÄ±labilir)
    ///   UserId/StoreId dolu             â†’ SATICIYA Ã–ZEL Ã¶zellik (yalnÄ±zca o satÄ±cÄ±nÄ±n Ã¼rÃ¼nlerinde gÃ¶rÃ¼nÃ¼r,
    ///                                      asla ÅŸablonlara eklenmez, diÄŸer satÄ±cÄ±lara gÃ¶sterilmez)
    /// </summary>
    public class ProductAttributes
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem Ã¶zelliÄŸi, dolu = satÄ±cÄ±ya Ã¶zel
        public Guid? StoreId { get; set; } // Null = sistem Ã¶zelliÄŸi, dolu = maÄŸazaya Ã¶zel

        public string? Name { get; set; } // Ã–zellik adÄ± (Ã–rn: Renk, RAM, Ekran Boyutu)
        public string? Code { get; set; } // Teknik kod / slug (Ã–rn: renk, ram) â€” entegrasyon eÅŸlemede kullanÄ±lÄ±r
        public string? Description { get; set; } // Ã–zellik aÃ§Ä±klamasÄ±

        // GiriÅŸ tipi: "Select", "MultiSelect", "Text", "Number", "Bool", "Date"
        public string? InputType { get; set; } = "Select";

        public string? Unit { get; set; } // Birim (Ã–rn: GB, inÃ§, Hz) â€” Number tipinde kullanÄ±ÅŸlÄ±
        public bool IsFilterable { get; set; } = false; // ÃœrÃ¼n listelerinde filtre olarak gÃ¶sterilsin mi
        public bool IsActive { get; set; } = true; // Ã–zellik kullanÄ±mda mÄ±
        public int DisplayOrder { get; set; } = 0; // Listeleme sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: ProductAttributeValues.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ã–zelliklerin Ã¶nceden tanÄ±mlÄ± deÄŸerleri (Ã–rn: Renk â†’ KÄ±rmÄ±zÄ±, Bordo, Siyah).
    /// Sahiplik kuralÄ± ProductAttributes ile aynÄ±dÄ±r:
    ///   UserId/StoreId null â†’ sistem deÄŸeri, dolu â†’ yalnÄ±zca o satÄ±cÄ±da gÃ¶rÃ¼nen Ã¶zel deÄŸer.
    /// BÃ¶ylece sistem Ã¶zelliÄŸi olan "Renk"e bir satÄ±cÄ± kendi Ã¶zel rengini ekleyebilir
    /// ama bu deÄŸer diÄŸer satÄ±cÄ±lara yansÄ±maz.
    /// </summary>
    public class ProductAttributeValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeId { get; set; } // BaÄŸlÄ± olduÄŸu Ã¶zellik (ProductAttributes.Id)

        public Guid? UserId { get; set; } // Null = sistem deÄŸeri, dolu = satÄ±cÄ±ya Ã¶zel deÄŸer
        public Guid? StoreId { get; set; } // Null = sistem deÄŸeri, dolu = maÄŸazaya Ã¶zel deÄŸer

        public string? Value { get; set; } // DeÄŸer metni (Ã–rn: KÄ±rmÄ±zÄ±, 16 GB, 55")
        public string? Code { get; set; } // Teknik kod (Ã–rn: kirmizi) â€” entegrasyon eÅŸlemede kullanÄ±lÄ±r
        public string? ColorHex { get; set; } // Renk Ã¶zellikleri iÃ§in HEX kodu (Ã–rn: #FF0000) â€” UI renk kutucuÄŸu
        public int DisplayOrder { get; set; } = 0; // Listeleme sÄ±rasÄ±
        public bool IsActive { get; set; } = true; // DeÄŸer kullanÄ±mda mÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: ProductBundleItems.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Paket/Bundle Ã¼rÃ¼n iÃ§erik tablosu.
    /// ProductType = "Bundle" olan Ã¼rÃ¼nÃ¼n hangi alt Ã¼rÃ¼n/varyantlardan oluÅŸtuÄŸunu tutar.
    /// </summary>
    public class ProductBundleItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BundleProductId { get; set; } // Paket Ã¼rÃ¼n (Products.Id, ProductType = "Bundle")
        public Guid ChildProductId { get; set; } // Pakete dahil Ã¼rÃ¼n (Products.Id)
        public Guid? ChildVariantId { get; set; } // Belirli bir varyant zorunluysa (ProductVariants.Id), null = mÃ¼ÅŸteri seÃ§er

        public int Quantity { get; set; } = 1; // Paketteki adet
        public decimal? DiscountRate { get; set; } // Paket iÃ§i indirim oranÄ± (%) â€” opsiyonel

        public int DisplayOrder { get; set; } = 0; // Paket iÃ§eriÄŸi gÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductCategories.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n - Kategori Ã§oklu iliÅŸki tablosu.
    /// Bir Ã¼rÃ¼n birden fazla kategoriye baÄŸlanabilir; IsPrimary = true olan kayÄ±t ana kategoridir.
    /// </summary>
    public class ProductCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id â€” int tip)

        public bool IsPrimary { get; set; } = false; // Ana kategori mi (Ã¼rÃ¼n baÅŸÄ±na yalnÄ±zca 1 adet true olmalÄ±)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductDigitalAssets.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Dijital Ã¼rÃ¼n dosyalarÄ± (ProductType = "Digital").
    /// SatÄ±n alma sonrasÄ± indirilebilir dosyalar ve indirme kurallarÄ± burada tutulur.
    /// </summary>
    public class ProductDigitalAssets
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid? VariantId { get; set; } // Dosya belirli bir varyanta Ã¶zelse (ProductVariants.Id)

        public Guid MediaId { get; set; } // Ä°ndirilebilir dosya (Media.Id)
        public string? DisplayName { get; set; } // MÃ¼ÅŸteriye gÃ¶sterilecek dosya adÄ±

        public int? DownloadLimit { get; set; } // Maksimum indirme sayÄ±sÄ± (null = sÄ±nÄ±rsÄ±z)
        public int? ExpirationDays { get; set; } // SatÄ±n alma sonrasÄ± geÃ§erlilik sÃ¼resi - gÃ¼n (null = sÃ¼resiz)
        public bool RequiresLicenseKey { get; set; } = false; // Lisans anahtarÄ± Ã¼retilsin mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductExternalMedias.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼nÃ¼n galeri dÄ±ÅŸÄ± medya kaynaklarÄ±: video URL'leri, 360 derece gÃ¶rÃ¼nÃ¼m,
    /// embed iÃ§erikler ve PDF/dokÃ¼man dosyalarÄ±.
    /// Sistemde yÃ¼klÃ¼ dosyalar MediaId ile (Media.Id), harici kaynaklar Url ile baÄŸlanÄ±r.
    /// </summary>
    public class ProductExternalMedias
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)

        // "VideoUrl", "View360", "Embed", "Document"
        public string? MediaType { get; set; } = "VideoUrl";

        public string? Url { get; set; } // Harici kaynak adresi (YouTube, Vimeo, 360 viewer vb.)
        public Guid? MediaId { get; set; } // Sistemde yÃ¼klÃ¼ dosya (Media.Id) â€” PDF/dokÃ¼man iÃ§in

        public string? Title { get; set; } // GÃ¶sterim baÅŸlÄ±ÄŸÄ± (Ã–rn: "Kurulum KÄ±lavuzu")
        public int DisplayOrder { get; set; } = 0; // GÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductImageVariantGroups.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant gÃ¶rsel gruplarÄ±.
    /// GÃ¶rseller varyant kombinasyonuna DEÄÄ°L, gÃ¶rsel-varyant Ã¶zelliÄŸinin DEÄERÄ°NE baÄŸlanÄ±r:
    ///   Ã–rn: Renk gÃ¶rsel varyantÄ± ise â†’ KÄ±rmÄ±zÄ± iÃ§in 1 grup, Bordo iÃ§in 1 grup, Siyah iÃ§in 1 grup.
    ///   KÄ±rmÄ±zÄ±-S ve KÄ±rmÄ±zÄ±-M aynÄ± (KÄ±rmÄ±zÄ±) gÃ¶rsel grubunu paylaÅŸÄ±r.
    /// GÃ¶rseller ItemGallery Ã¼zerinden baÄŸlanÄ±r:
    ///   ItemGallery.ItemType = "VariantGallery", ItemGallery.ItemId = ProductImageVariantGroups.Id
    /// Bir gruba birden fazla gÃ¶rsel eklenebilir (ItemGallery zaten Ã§oklu kayÄ±t destekler).
    /// </summary>
    public class ProductImageVariantGroups
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid AttributeId { get; set; } // GÃ¶rsel varyant Ã¶zelliÄŸi (ProductAttributes.Id â€” Ã–rn: Renk)
        public Guid AttributeValueId { get; set; } // GÃ¶rselin baÄŸlandÄ±ÄŸÄ± deÄŸer (ProductAttributeValues.Id â€” Ã–rn: KÄ±rmÄ±zÄ±)

        public Guid? CoverMediaId { get; set; } // Bu deÄŸerin kapak gÃ¶rseli (Media.Id) â€” listelemede gÃ¶sterilir

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductImportProfiles.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// CSV / XML / JSON / API iÃ§e aktarÄ±m profilleri.
    /// SatÄ±cÄ±, kaynak dosya kolonlarÄ±nÄ±n sistem alanlarÄ±na nasÄ±l eÅŸleneceÄŸini bir kez tanÄ±mlar
    /// (FieldMappingJson), sonraki aktarÄ±mlarda profili yeniden kullanÄ±r.
    /// Trendyol, Amazon, WooCommerce, N11 gibi hazÄ±r kaynak tipleri desteklenir.
    /// </summary>
    public class ProductImportProfiles
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Profili oluÅŸturan kullanÄ±cÄ± (Users.Id)
        public Guid StoreId { get; set; } // BaÄŸlÄ± maÄŸaza (Store.Id)

        public string? Name { get; set; } // Profil adÄ± (Ã–rn: "Trendyol AylÄ±k AktarÄ±m")

        // "CSV", "XML", "JSON", "RestApi", "SoapApi", "Amazon", "Alibaba", "Ebay", "MercadoLibre",
        // "Jd", "Trendyol", "Hepsiburada", "N11", "CicekSepeti", "Etsy", "Shopify", "WooCommerce"
        public string? SourceType { get; set; } = "CSV";

        public string? FieldMappingJson { get; set; } // Kolon â†’ alan eÅŸleme tanÄ±mÄ± (JSON)
        public string? Delimiter { get; set; } = ";"; // CSV ayÄ±racÄ±
        public string? Encoding { get; set; } = "UTF-8"; // Dosya karakter seti
        public string? SourceUrl { get; set; } // API/XML feed adresi (dosya dÄ±ÅŸÄ± kaynaklarda)

        public Guid? DefaultWarehouseId { get; set; } // AktarÄ±lan Ã¼rÃ¼nlerin varsayÄ±lan deposu (Warehouse.Id)
        public int? DefaultCategoryId { get; set; } // EÅŸlenemeyen Ã¼rÃ¼nler iÃ§in varsayÄ±lan kategori (CategoriesTr.Id)

        public bool IsActive { get; set; } = true; // Profil kullanÄ±mda mÄ±
        public DateTime? LastRunDate { get; set; } // Son Ã§alÄ±ÅŸtÄ±rma tarihi
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: ProductMarketplaceListings.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n/varyantÄ±n dÄ±ÅŸ platformlardaki karÅŸÄ±lÄ±ÄŸÄ± ve senkronizasyon durumu.
    /// Ä°Ã§e aktarÄ±mda platformdan gelen HAM VERÄ° RawSourceData alanÄ±nda JSON olarak saklanÄ±r â€”
    /// bÃ¶ylece modelimizde karÅŸÄ±lÄ±ÄŸÄ± olmayan alanlar dahi VERÄ° KAYBI olmadan korunur
    /// ve ileride yeniden iÅŸlenebilir.
    /// </summary>
    public class ProductMarketplaceListings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)
        public Guid? VariantId { get; set; } // Varyant bazlÄ± eÅŸleme gerekiyorsa (ProductVariants.Id)
        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid StoreId { get; set; } // MaÄŸaza (Store.Id) â€” hangi maÄŸazanÄ±n entegrasyonu

        public string? ExternalProductId { get; set; } // Platformdaki Ã¼rÃ¼n ID'si (Ã–rn: Amazon ASIN, Trendyol contentId)
        public string? ExternalSku { get; set; } // Platformdaki SKU (merchantSku vb.)
        public string? ExternalUrl { get; set; } // Platformdaki Ã¼rÃ¼n sayfasÄ± adresi

        // "Pending", "Synced", "Error", "Paused"
        public string? SyncStatus { get; set; } = "Pending";
        public DateTime? LastSyncDate { get; set; } // Son baÅŸarÄ±lÄ± senkronizasyon tarihi
        public string? LastErrorMessage { get; set; } // Son hata mesajÄ±

        public string? RawSourceData { get; set; } // Ä°Ã§e aktarÄ±mda gelen ham kaynak verisi (JSON) â€” veri kaybÄ± sigortasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductPrices.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant bazlÄ±, Ã§oklu para birimli fiyat tablosu (APPEND-ONLY tarih geÃ§miÅŸi).
    /// Her para birimi (TRY/USD/EUR/AZN) iÃ§in ayrÄ± satÄ±r tutulur â€” kur dÃ¶nÃ¼ÅŸÃ¼mÃ¼ YAPILMAZ,
    /// deÄŸerler baÄŸÄ±msÄ±z olarak veri tabanÄ±nda saklanÄ±r.
    /// Fiyat deÄŸiÅŸiminde eski satÄ±r gÃ¼ncellenmez: EffectiveTo kapatÄ±lÄ±r, yeni satÄ±r aÃ§Ä±lÄ±r.
    /// GÃ¼ncel fiyat sorgusu: EffectiveTo == null olan satÄ±r.
    /// </summary>
    public class ProductPrices
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id) â€” raporlama kolaylÄ±ÄŸÄ± iÃ§in denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id) â€” basit Ã¼rÃ¼nde varsayÄ±lan varyant

        public string? Currency { get; set; } = "TRY"; // Para birimi ("TRY", "USD", "EUR", "AZN")

        public decimal Price { get; set; } // Normal satÄ±ÅŸ fiyatÄ±
        public decimal? DiscountedPrice { get; set; } // Ä°ndirimli satÄ±ÅŸ fiyatÄ± (kampanya) â€” null ise indirim yok
        public decimal? CostPrice { get; set; } // Maliyet fiyatÄ± (yalnÄ±zca satÄ±cÄ± gÃ¶rÃ¼r)

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow; // GeÃ§erlilik baÅŸlangÄ±cÄ±
        public DateTime? EffectiveTo { get; set; } // GeÃ§erlilik bitiÅŸi (null = gÃ¼ncel/aktif fiyat)

        public Guid? CreatedByUserId { get; set; } // FiyatÄ± giren kullanÄ±cÄ± (denetim iÃ§in)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // KayÄ±t tarihi
    }
}
` 
--- 

## Dosya: Products.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ana Ã¼rÃ¼n tablosu. TÃ¼m Ã¼rÃ¼n tipleri (basit, varyantlÄ±, dijital, hizmet, paket, harici)
    /// bu tablo Ã¼zerinden yÃ¶netilir. Basit Ã¼rÃ¼nler dahi en az bir ProductVariants kaydÄ±na sahiptir
    /// (birleÅŸik tek-varyant modeli) â€” bÃ¶ylece fiyat/stok her zaman varyant Ã¼zerinden okunur.
    /// </summary>
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === Ä°liÅŸkiler (ID bazlÄ±, navigation property kullanÄ±lmaz) ===
        public Guid UserId { get; set; } // ÃœrÃ¼n sahibi kullanÄ±cÄ± (Users.Id)
        public Guid StoreId { get; set; } // ÃœrÃ¼n sahibi maÄŸaza (Store.Id)
        public Guid? BrandId { get; set; } // Marka (Brands.Id)
        public Guid? AttributeTemplateId { get; set; } // ÃœrÃ¼nÃ¼n oluÅŸturulduÄŸu Ã¶zellik ÅŸablonu SÃœRÃœMÃœ (AttributeTemplates.Id) â€” versiyon sabitlenir, ÅŸablon gÃ¼ncellense bile Ã¼rÃ¼n verisi bozulmaz
        public Guid? ShippingProfileId { get; set; } // Kargo/desi profili (ShippingProfile.Id)

        // === ÃœrÃ¼n Tipi ===
        // "Simple", "Variant", "Digital", "Service", "Bundle", "External"
        // Yeni tipler iÃ§in enum yerine string tercih edildi (geniÅŸletilebilirlik)
        public string? ProductType { get; set; } = "Simple";

        // === Ä°Ã§erik AlanlarÄ± ===
        public string? Name { get; set; } // ÃœrÃ¼n adÄ±
        public string? ShortDescription { get; set; } // KÄ±sa aÃ§Ä±klama
        public string? FullDescription { get; set; } // DetaylÄ± aÃ§Ä±klama (HTML destekli)
        public string? Tags { get; set; } // Etiketler (virgÃ¼lle ayrÄ±lmÄ±ÅŸ)

        // === Medya ===
        public Guid? CoverMediaId { get; set; } // Kapak gÃ¶rseli (Media.Id)
        // Galeri gÃ¶rselleri: ItemGallery (ItemType = "ProductGallery", ItemId = Products.Id)
        // Varyant gÃ¶rselleri: ItemGallery (ItemType = "VariantGallery", ItemId = ProductImageVariantGroups.Id)

        // === Harici ÃœrÃ¼n (External) AlanlarÄ± ===
        public string? ExternalUrl { get; set; } // Harici Ã¼rÃ¼n baÄŸlantÄ±sÄ±
        public string? ExternalButtonText { get; set; } // Harici Ã¼rÃ¼n buton metni (Ã–rn: "SatÄ±cÄ± sitesinde gÃ¶r")

        // === YayÄ±n Durumu ===
        // "Draft", "PendingApproval", "Published", "Rejected", "Archived"
        public string? PublishStatus { get; set; } = "Draft";
        public bool? IsApprovedByAdmin { get; set; } // Admin onay durumu
        public DateTime? ApprovedDate { get; set; } // Admin onay tarihi
        public Guid? ApprovedByUserId { get; set; } // Onaylayan admin (Users.Id)
        public bool IsActive { get; set; } = true; // SatÄ±cÄ± tarafÄ±ndan aktif/pasif

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son gÃ¼ncelleme tarihi

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
` 
--- 

## Dosya: ProductSeo.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n SEO bilgileri. Products tablosundan ayrÄ± tutulur â€” listeleme sorgularÄ±nÄ±
    /// ÅŸiÅŸirmez ve ileride dil bazlÄ± SEO satÄ±rlarÄ±na geniÅŸletilebilir.
    /// </summary>
    public class ProductSeo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)

        public string? Slug { get; set; } // SEO dostu URL (Ã–rn: samsung-55-oled-tv) â€” benzersiz olmalÄ±
        public string? SeoTitle { get; set; } // SEO baÅŸlÄ±k (meta title)
        public string? SeoDescription { get; set; } // SEO aÃ§Ä±klama (meta description)
        public string? SeoKeywords { get; set; } // SEO anahtar kelimeler (virgÃ¼lle ayrÄ±lmÄ±ÅŸ)
        public string? CanonicalUrl { get; set; } // Canonical adres (kopya iÃ§erik Ã¶nleme)
        public string? MetaJson { get; set; } // Ek meta veriler (JSON: OpenGraph, schema.org vb.)

        public string? LanguageCode { get; set; } = "tr"; // Dil kodu â€” ileride Ã§oklu dil SEO satÄ±rlarÄ± iÃ§in

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son gÃ¼ncelleme tarihi
    }
}
` 
--- 

## Dosya: ProductSpecifications.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼nÃ¼n teknik Ã¶zellik/spesifikasyon DEÄERLERÄ° (varyant Ã¼retmeyen bilgi alanlarÄ±).
    /// Ä°ki kullanÄ±m ÅŸekli desteklenir:
    ///   1) TanÄ±mlÄ± Ã¶zellik: AttributeId (+ AttributeValueId veya serbest CustomValue)
    ///      Ã–rn: Ekran Boyutu â†’ 55"
    ///   2) Tamamen serbest satÄ±r: CustomName + CustomValue (AttributeId null)
    ///      Ã–rn: "Kutu Ä°Ã§eriÄŸi" â†’ "TV, Kumanda, Duvar AparatÄ±" (yalnÄ±zca bu Ã¼rÃ¼nde geÃ§erli)
    /// </summary>
    public class ProductSpecifications
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id)

        public Guid? AttributeId { get; set; } // TanÄ±mlÄ± Ã¶zellik (ProductAttributes.Id) â€” serbest satÄ±rda null
        public Guid? AttributeValueId { get; set; } // Ã–nceden tanÄ±mlÄ± deÄŸer seÃ§ildiyse (ProductAttributeValues.Id)

        public string? CustomName { get; set; } // Serbest Ã¶zellik adÄ± (AttributeId null ise kullanÄ±lÄ±r)
        public string? CustomValue { get; set; } // Serbest/metinsel deÄŸer (Text-Number tipli Ã¶zelliklerde de kullanÄ±lÄ±r)

        public int DisplayOrder { get; set; } = 0; // GÃ¶sterim sÄ±rasÄ±

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
    }
}
` 
--- 

## Dosya: ProductStocks.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant + Depo bazlÄ± stok tablosu.
    /// Bir varyant birden fazla depoda tutulabilir (Ã§oklu depo desteÄŸi);
    /// toplam stok = varyantÄ±n tÃ¼m depo satÄ±rlarÄ±nÄ±n toplamÄ±dÄ±r.
    /// </summary>
    public class ProductStocks
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // ÃœrÃ¼n (Products.Id) â€” denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid WarehouseId { get; set; } // Depo (Warehouse.Id)

        public bool TrackStock { get; set; } = true; // Stok takibi yapÄ±lsÄ±n mÄ± (dijital/hizmet Ã¼rÃ¼nlerde false)

        public int Quantity { get; set; } = 0; // Mevcut stok adedi
        public int? MinStockLevel { get; set; } // Minimum stok miktarÄ±
        public int? CriticalStockLevel { get; set; } // Kritik stok seviyesi (uyarÄ± eÅŸiÄŸi)
        public int? MaxOrderQuantity { get; set; } // Tek sipariÅŸte alÄ±nabilecek maksimum adet

        // "InStock", "OutOfStock", "PreOrder", "Backorder"
        public string? StockStatus { get; set; } = "InStock";

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow; // Son stok gÃ¼ncelleme tarihi
    }
}
` 
--- 

## Dosya: ProductVariants.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// ÃœrÃ¼n varyantlarÄ±. BirleÅŸik tek-varyant modeli gereÄŸi basit Ã¼rÃ¼nlerde de
    /// IsDefault = true olan tek bir varyant kaydÄ± bulunur.
    /// Fiyat (ProductPrices) ve stok (ProductStocks) daima varyant Ã¼zerinden tutulur.
    /// </summary>
    public class ProductVariants
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // BaÄŸlÄ± olduÄŸu Ã¼rÃ¼n (Products.Id)

        // === Kimlik / TanÄ±mlayÄ±cÄ± Alanlar (Marketplace entegrasyon standartlarÄ±) ===
        public string? Sku { get; set; } // Stok kodu (Ã–rn: KZK-KRM-S)
        public string? Barcode { get; set; } // Barkod
        public string? Gtin { get; set; } // Global Trade Item Number
        public string? Upc { get; set; } // Universal Product Code
        public string? Ean { get; set; } // European Article Number
        public string? Isbn { get; set; } // Kitaplar iÃ§in ISBN
        public string? Mpn { get; set; } // Manufacturer Part Number (Ãœretici parÃ§a numarasÄ±)

        // === Durum ===
        public bool IsDefault { get; set; } = false; // VarsayÄ±lan varyant mÄ± (basit Ã¼rÃ¼nde tek kayÄ±t true)
        public bool IsActive { get; set; } = true; // Varyant satÄ±ÅŸta mÄ±
        public int DisplayOrder { get; set; } = 0; // Listeleme sÄ±rasÄ±

        // === Fiziksel Ã–zellikler (kargo/desi hesabÄ± ShippingProfile bÃ¶len deÄŸerleri ile yapÄ±lÄ±r) ===
        public decimal? WeightKg { get; set; } // AÄŸÄ±rlÄ±k (kg)
        public decimal? WidthCm { get; set; } // GeniÅŸlik (cm)
        public decimal? HeightCm { get; set; } // YÃ¼kseklik (cm)
        public decimal? LengthCm { get; set; } // Uzunluk (cm)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son gÃ¼ncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

## Dosya: ProductVariantValues.cs


`csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant kombinasyon tablosu. Her varyantÄ±n hangi Ã¶zellik-deÄŸer ikililerinden
    /// oluÅŸtuÄŸunu tutar. Ã–rn: KZK-KRM-S varyantÄ± iÃ§in iki satÄ±r:
    ///   (VariantId, Renk, KÄ±rmÄ±zÄ±) ve (VariantId, Beden, S)
    /// </summary>
    public class ProductVariantValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid AttributeId { get; set; } // Ã–zellik (ProductAttributes.Id) â€” sorgu kolaylÄ±ÄŸÄ± iÃ§in denormalize
        public Guid AttributeValueId { get; set; } // Ã–zellik deÄŸeri (ProductAttributeValues.Id)
    }
}
` 
--- 

## Dosya: Warehouse.cs


`csharp
using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Depo/lojistik noktasÄ± tanÄ±mÄ±. MaÄŸaza bazlÄ±dÄ±r.
    /// Adres bilgisi iÃ§in mevcut AddressInfo owned type'Ä± yeniden kullanÄ±lÄ±r.
    /// </summary>
    public class Warehouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Depoyu ekleyen kullanÄ±cÄ±nÄ±n/satÄ±cÄ±nÄ±n ID bilgisi
        public Guid StoreId { get; set; } // Deponun baÄŸlÄ± olduÄŸu maÄŸazanÄ±n ID bilgisi

        public bool IsDefault { get; set; } = false; // VarsayÄ±lan depo mu
        public bool IsActive { get; set; } = true; // Depo aktif mi

        public AddressInfo? AddressInfo { get; set; } = new(); // Depo adres bilgisi (mevcut owned type)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // OluÅŸturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
` 
--- 

