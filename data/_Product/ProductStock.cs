using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Çoklu depo stok tablosu. Tek depo senaryosunda Product/ProductVariant.StockQuantity yeterlidir;
    /// çoklu depoya geçildiğinde stoğun asıl kaynağı bu tablodur.
    /// Satılabilir stok = Quantity - ReservedQuantity.
    /// </summary>
    public class ProductStock
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Stoğu yöneten kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // Stoğun ait olduğu ana ürünün ID'si (her zaman dolu)
        public Guid? VariantId { get; set; } // Stoğun ait olduğu varyantın ID'si (basit üründe null)
        public Guid? WarehouseId { get; set; } // Stoğun bulunduğu deponun ID'si (tek depoda null/varsayılan)

        public int Quantity { get; set; } // Fiziksel stok adedi
        public int ReservedQuantity { get; set; } // Rezerve (sipariş bekleyen) adet
        public int SafetyStockThreshold { get; set; } // Kritik stok uyarı eşiği

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
