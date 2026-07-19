using System;

namespace data._Attribute
{
    /// <summary>
    /// Dış entegrasyon platformu (referans tablo). Enum yerine tablo tercih edildi:
    /// yeni platform (yeni pazaryeri/format) yayın gerektirmeden eklenebilir.
    /// Ör: amazon, trendyol, hepsiburada, n11, ebay, aliexpress, temu,
    /// google_merchant, facebook_catalog, woocommerce, csv, json, xml, api.
    /// </summary>
    public class IntegrationPlatform : AttributeEntityBase
    {
        /// <summary>Tekil kod (ör. "trendyol").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>İnsan-okur ad (ör. "Trendyol").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Platform aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// İç <see cref="AttributeDefinition"/> ile dış platform attribute'ının
    /// eşleştirmesi. Import/Export ve platform-zorunluluğu bilgisini taşır.
    /// (IntegrationPlatformId, AttributeDefinitionId, Direction) benzersizdir.
    /// </summary>
    public class AttributeMapping : AttributeEntityBase
    {
        /// <summary>Bağlı platform.</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Bağlı iç attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Dış platformdaki attribute kodu.</summary>
        public string ExternalCode { get; set; } = string.Empty;

        /// <summary>Dış platformdaki attribute kimliği (varsa).</summary>
        public string? ExternalId { get; set; }

        /// <summary>Dış platformdaki attribute adı (referans).</summary>
        public string? ExternalName { get; set; }

        /// <summary>Eşleştirme yönü (import/export/iki yönlü).</summary>
        public MappingDirection Direction { get; set; } = MappingDirection.Bidirectional;

        /// <summary>Bu attribute'ın platform açısından zorunluluk seviyesi.</summary>
        public PlatformRequirementLevel RequirementLevel { get; set; } = PlatformRequirementLevel.Optional;

        /// <summary>Dönüşüm kuralı yapılandırması (JSON).</summary>
        public string? TransformRuleJson { get; set; }

        /// <summary>Uygulanacak normalizasyon kuralı (opsiyonel).</summary>
        public Guid? NormalizationRuleId { get; set; }

        /// <summary>Eşleştirme aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// İç <see cref="AttributeOption"/> ile dış platform option/değerinin eşleştirmesi.
    /// Ör: iç "black" ↔ Amazon "BLACK". <see cref="AttributeDefinitionId"/> denormalize
    /// tutulur; import'ta (platform + attribute + dış kod) ile hızlı option bulunur.
    /// (IntegrationPlatformId, AttributeOptionId, Direction) benzersizdir.
    /// </summary>
    public class AttributeOptionMapping : AttributeEntityBase
    {
        /// <summary>Bağlı platform.</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Bağlı iç option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Denormalize attribute referansı (import lookup indeksleri için).</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Dış platformdaki option kodu.</summary>
        public string ExternalCode { get; set; } = string.Empty;

        /// <summary>Dış platformdaki option değeri/etiketi (referans).</summary>
        public string? ExternalValue { get; set; }

        /// <summary>Dış platformdaki option kimliği (varsa).</summary>
        public string? ExternalId { get; set; }

        /// <summary>Eşleştirme yönü.</summary>
        public MappingDirection Direction { get; set; } = MappingDirection.Bidirectional;

        /// <summary>Eşleştirme aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }
}
