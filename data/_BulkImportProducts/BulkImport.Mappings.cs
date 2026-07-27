using System;

namespace data._BulkImportProducts
{
    /// <summary>
    /// ALAN/SÜTUN EŞLEŞTİRMESİ — kaynaktaki bir alanı (CSV sütunu, JSON/XML yolu)
    /// efavori'deki bir ürün alanına bağlar. Ör: WooCommerce CSV "Regular price"
    /// sütunu → <see cref="ImportTargetField.Price"/>; Amazon API "item.title" →
    /// <see cref="ImportTargetField.Name"/>.
    ///
    /// Bir profile bağlıdır (<see cref="ImportProfileId"/>); böylece kullanıcı bir kez
    /// eşleştirir, o profille yaptığı her içe aktarımda yeniden kullanılır.
    /// (ImportProfileId, SourceField, TargetLanguageCode, TargetAttributeDefinitionId)
    /// pratikte tekildir; benzersizlik uygulama katmanında da doğrulanır.
    /// </summary>
    public class ImportFieldMapping : BulkImportEntityBase
    {
        /// <summary>Bağlı içe aktarım profili (ImportProfile.Id).</summary>
        public Guid ImportProfileId { get; set; }

        /// <summary>
        /// Kaynaktaki alan adı/yolu. CSV'de sütun başlığı (ör. "Regular price"),
        /// JSON'da yol (ör. "price.amount"), XML'de XPath (ör. "g:price").
        /// </summary>
        public string SourceField { get; set; } = string.Empty;

        /// <summary>
        /// Başlıksız CSV için sütun sırası (0 tabanlı). HasHeaderRow=false ise
        /// SourceField yerine bu kullanılır.
        /// </summary>
        public int? SourceColumnIndex { get; set; }

        /// <summary>Hedef efavori ürün alanı.</summary>
        public ImportTargetField TargetField { get; set; }

        /// <summary>
        /// TargetField = Name/ShortDescription/Description gibi çevrilebilir alanlarda
        /// bu değerin hangi dile yazılacağı (kültür kodu; boşsa profilin kaynak dili).
        /// </summary>
        public string? TargetLanguageCode { get; set; }

        /// <summary>
        /// TargetField = Attribute/VariantAxis ise, değerin bağlanacağı iç attribute
        /// (data._Attribute.AttributeDefinition.Id). Böylece kaynak "Screen Size"
        /// sütunu iç "screen_size" attribute'una gider.
        /// </summary>
        public Guid? TargetAttributeDefinitionId { get; set; }

        /// <summary>TargetField = Price/ListPrice ise fiyatın para birimi (ör. "USD").</summary>
        public string? TargetCurrencyCode { get; set; }

        /// <summary>Bu alan zorunlu mu? (boşsa satır NeedsReview/atlanır).</summary>
        public bool IsRequired { get; set; }

        /// <summary>Kaynak boşsa kullanılacak sabit varsayılan değer (opsiyonel).</summary>
        public string? DefaultValue { get; set; }

        /// <summary>Uygulanacak dönüşüm türü.</summary>
        public ImportTransformType TransformType { get; set; } = ImportTransformType.None;

        /// <summary>
        /// Dönüşüm ayrıntısı (JSON). Ör: Multiply için {"factor":1.2};
        /// RegexReplace için {"pattern":"...","replacement":"..."};
        /// Split için {"separator":"|"}; ValueMap için {"red":"kirmizi",...}.
        /// </summary>
        public string? TransformConfigJson { get; set; }

        /// <summary>Eşleştirme aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Sıralama (çok değerli alanlarda uygulama önceliği).</summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// KATEGORİ EŞLEŞTİRMESİ — kaynak platformdaki bir kategoriyi/yolu efavori
    /// kategorisine bağlar. Ör: Amazon "Electronics > Phones > Smartphones" →
    /// efavori CategoriesProduct.Id. Kullanıcı bir kez eşleştirir, sonraki içe
    /// aktarımlarda otomatik uygulanır.
    ///
    /// Profile bağlı çalışır ama <see cref="ImportProfileId"/> null bırakılarak
    /// kullanıcı+platform genelinde paylaşılan bir eşleştirme de yapılabilir
    /// (böylece aynı Amazon kategorisi tüm profillerde tanınır).
    /// </summary>
    public class ImportCategoryMapping : BulkImportEntityBase
    {
        /// <summary>Eşleştirmeyi yapan kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Kaynak platform (data._Attribute.IntegrationPlatform.Id).</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Bağlı profil (opsiyonel; null = kullanıcı+platform geneli).</summary>
        public Guid? ImportProfileId { get; set; }

        /// <summary>Kaynak kategori kodu/kimliği (varsa; ör. Amazon browse node id).</summary>
        public string? SourceCategoryCode { get; set; }

        /// <summary>Kaynak kategori yolu/adı (ör. "Electronics > Phones > Smartphones").</summary>
        public string SourceCategoryPath { get; set; } = string.Empty;

        /// <summary>Kaynak kategori yolunun normalize edilmiş biçimi (dedup/lookup, indeksli).</summary>
        public string NormalizedSourcePath { get; set; } = string.Empty;

        /// <summary>Hedef efavori kategorisi (CategoriesProduct.Id).</summary>
        public Guid TargetCategoryId { get; set; }

        /// <summary>Eşleştirme aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bu eşleştirme AI tarafından mı önerildi? (kategori adı benzerliğine göre
        /// otomatik öneri). Kullanıcı onaylayınca IsConfirmed = true olur.
        /// </summary>
        public bool IsAiSuggested { get; set; }

        /// <summary>Kullanıcı bu eşleştirmeyi onayladı mı?</summary>
        public bool IsConfirmed { get; set; }

        /// <summary>AI önerisinde güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }

        /// <summary>Marka eşleştirme boşsa uygulanacak varsayılan marka (Brands.Id — opsiyonel).</summary>
        public Guid? DefaultBrandId { get; set; }
    }
}
