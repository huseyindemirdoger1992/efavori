using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    /// <summary>
    /// Varyant özelliği (Örn: "Renk", "Beden") oluşturma/seçme isteği. Tek başına gönderilir.
    /// Var olan bir özellik kullanılacaksa istek atılmaz; mevcut AttributeId doğrudan kullanılır.
    /// Sunucu yeni kayıt oluşturursa dönen Id, ProductAttributeValueCreateRequest.AttributeId'de kullanılır.
    /// </summary>
    public class ProductAttributeCreateRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public string Name { get; set; } = string.Empty; // Özellik adı (Örn: "Renk")
        public string? Code { get; set; } // Özellik teknik kodu (Örn: "color")
        public string DisplayType { get; set; } = "text"; // Gösterim tipi: "color" | "text" | "image"
        public bool IsVariantDefining { get; set; } = true; // Bu özellik varyant üretiminde kullanılıyor mu
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası
        public IsDeleted? IsDeleted { get; set; } = new(); // silinme durumu (soft delete için)

    }
}
