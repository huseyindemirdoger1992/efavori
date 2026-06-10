using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Paket/Bundle ürün içerik tablosu.
    /// ProductType = "Bundle" olan ürünün hangi alt ürün/varyantlardan oluştuğunu tutar.
    /// </summary>
    public class ProductBundleItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BundleProductId { get; set; } // Paket ürün (Products.Id, ProductType = "Bundle")
        public Guid ChildProductId { get; set; } // Pakete dahil ürün (Products.Id)
        public Guid? ChildVariantId { get; set; } // Belirli bir varyant zorunluysa (ProductVariants.Id), null = müşteri seçer

        public int Quantity { get; set; } = 1; // Paketteki adet
        public decimal? DiscountRate { get; set; } // Paket içi indirim oranı (%) — opsiyonel

        public int DisplayOrder { get; set; } = 0; // Paket içeriği gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
