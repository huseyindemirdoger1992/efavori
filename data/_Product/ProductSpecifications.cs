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
