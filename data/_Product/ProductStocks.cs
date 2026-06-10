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
