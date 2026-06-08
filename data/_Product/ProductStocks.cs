using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün ve varyant stoklarını depolara göre tutar.
    /// Warehouse tablosu ile ilişkilendirilir.
    /// Standart ürünlerde VariantId=null olur.
    /// </summary>
    public class ProductStocks
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }     // Ürün ID bilgisi
        public Guid? VariantId { get; set; }    // Varyant ID bilgisi (null ise standart ürün stoğu)
        public Guid WarehouseId { get; set; }   // Depo ID bilgisi (Warehouse.Id)
        public Guid UserId { get; set; }        // Satıcı ID bilgisi

        // === Stok Bilgileri ===
        public int Quantity { get; set; } = 0;            // Mevcut stok adedi
        public int? ReservedQuantity { get; set; } = 0;   // Rezerve edilen (siparişte bekleyen) adet
        public int? LowStockThreshold { get; set; }       // Düşük stok uyarı eşiği

        // === Stok Takip ===
        public bool TrackStock { get; set; } = true;      // Stok takibi yapılsın mı
        public bool AllowBackorder { get; set; } = false;  // Stok bittiğinde sipariş alınsın mı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
