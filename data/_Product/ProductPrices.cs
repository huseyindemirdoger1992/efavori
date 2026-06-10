using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant bazlı, çoklu para birimli fiyat tablosu (APPEND-ONLY tarih geçmişi).
    /// Her para birimi (TRY/USD/EUR/AZN) için ayrı satır tutulur — kur dönüşümü YAPILMAZ,
    /// değerler bağımsız olarak veri tabanında saklanır.
    /// Fiyat değişiminde eski satır güncellenmez: EffectiveTo kapatılır, yeni satır açılır.
    /// Güncel fiyat sorgusu: EffectiveTo == null olan satır.
    /// </summary>
    public class ProductPrices
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id) — raporlama kolaylığı için denormalize
        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id) — basit üründe varsayılan varyant

        public string? Currency { get; set; } = "TRY"; // Para birimi ("TRY", "USD", "EUR", "AZN")

        public decimal Price { get; set; } // Normal satış fiyatı
        public decimal? DiscountedPrice { get; set; } // İndirimli satış fiyatı (kampanya) — null ise indirim yok
        public decimal? CostPrice { get; set; } // Maliyet fiyatı (yalnızca satıcı görür)

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow; // Geçerlilik başlangıcı
        public DateTime? EffectiveTo { get; set; } // Geçerlilik bitişi (null = güncel/aktif fiyat)

        public Guid? CreatedByUserId { get; set; } // Fiyatı giren kullanıcı (denetim için)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Kayıt tarihi
    }
}
