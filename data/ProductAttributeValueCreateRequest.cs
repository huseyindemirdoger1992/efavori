using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    /// <summary>
    /// Bir özelliğe değer (Örn: Renk -> "Kırmızı") ekleme isteği. Tek başına gönderilir.
    /// AttributeId, önce oluşturulan/var olan ProductAttribute kaydının ID'sidir.
    /// Sunucu yeni kayıt oluşturursa dönen Id, ProductVariantValueCreateRequest.AttributeValueId'de kullanılır.
    /// </summary>
    public class ProductAttributeValueCreateRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid AttributeId { get; set; } // Değerin bağlı olduğu özelliğin ID'si
        public string Value { get; set; } = string.Empty; // Değer metni (Örn: "Kırmızı")
        public string? ColorHex { get; set; } // Renk tipi değerler için hex kodu (Örn: "#FF0000")
        public string? ImagePath { get; set; } // Görsel tabanlı değerler için swatch/görsel yolu
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası
        public IsDeleted? IsDeleted { get; set; } = new(); // silinme durumu (soft delete için)

    }
}
