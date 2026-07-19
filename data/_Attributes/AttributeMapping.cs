using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// İç attribute'ların dış platform alanlarıyla eşleştirilmesini sağlar.
    /// Amazon, Trendyol, Hepsiburada, AliExpress, WooCommerce, Google Merchant,
    /// Facebook Catalog, CSV/Excel/XML/JSON kaynakları için import ve export
    /// yönünde çalışır. Bir attribute'ın her platform için ayrı mapping satırı olur.
    /// Import sırasında eşleşmeyen alanlar kaybolmaz; AttributeValue.RawImportValue
    /// içinde ham olarak saklanır ve AI otomatik mapping önerisi üretebilir.
    /// </summary>
    public class AttributeMapping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Eşleştirilen iç attribute ID'si (AttributeDefinition tablosu).
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        /// Platform/kaynak kodu. Örn: "Amazon", "Trendyol", "Hepsiburada",
        /// "AliExpress", "WooCommerce", "GoogleMerchant", "FacebookCatalog",
        /// "Csv", "Excel", "Xml", "Json", "Api".
        /// </summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Dış platformdaki alanın görünen adı. Örn: Amazon → "Screen Size".
        /// </summary>
        public string? ExternalName { get; set; }

        /// <summary>
        /// Dış platformdaki alanın teknik kodu. Örn: Amazon → "display.size".
        /// </summary>
        public string? ExternalCode { get; set; }

        /// <summary>
        /// Dış platformdaki alanın sayısal/benzersiz ID'si. Örn: Trendyol attributeId → "338".
        /// </summary>
        public string? ExternalId { get; set; }

        /// <summary>
        /// CSV/Excel importlarında kolon başlığı eşleştirmesi için serbest ad.
        /// Örn: "Ekran Boyutu (inç)". Birden fazla olası ad "|" ile ayrılabilir.
        /// </summary>
        public string? MappingName { get; set; }

        /// <summary>
        /// Değer dönüşüm kuralları (JSON). Birim çevrimi, format düzeltme vb.
        /// Örn: {"unitFrom":"inch","unitTo":"cm","multiply":2.54}
        /// </summary>
        public string? TransformJson { get; set; }

        /// <summary>
        /// Dış seçenek değerlerinin iç AttributeOption.Value kodlarına eşlenmesi (JSON).
        /// Örn: {"Rot":"red","Rouge":"red","красный":"red"}
        /// </summary>
        public string? OptionMapJson { get; set; }

        /// <summary>
        /// Eşleştirme yönü: "Import", "Export", "Both".
        /// </summary>
        public string? Direction { get; set; } = "Both";

        /// <summary>Aktif / Pasif</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bu mapping AI tarafından mı önerildi/oluşturuldu?
        /// </summary>
        public bool? CreatedByAi { get; set; } = false;

        /// <summary>
        /// Admin bu mapping'i elle düzenledi mi? true ise AI üzerine yazamaz.
        /// </summary>
        public bool IsManuallyEdited { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
