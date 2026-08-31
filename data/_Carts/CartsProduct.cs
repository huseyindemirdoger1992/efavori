using System;
using data.Owned;

namespace data._Carts
{
    /// <summary>
    /// SEPET SATIRI — kullanıcının sepetine eklediği ürün/varyant.
    ///
    /// KALDIRILAN KARDEŞ ENTITY (§71): Aynı klasördeki <c>CartsFavorite</c> entity'si
    /// bu sınıfın birebir kopyasıydı; yalnızca adı farklıydı. "Favori" ile "sepet"
    /// tamamen farklı kavramlardır — favorinin fiyat/kargo/kupon snapshot'ına
    /// ihtiyacı yoktur, buna karşılık fiyat düşüşü bildirimi ve çoklu listeye
    /// ihtiyacı vardır. Bu yüzden <c>CartsFavorite</c> KALDIRILMIŞ, yerini
    /// <c>data._Products.ProductFavorites</c> ve <c>data._Products.Wishlists</c>
    /// almıştır.
    ///
    /// Sepet satırı, sipariş satırından FARKLIDIR: sipariş anında tüm değerler
    /// <c>data._Orders.OrderItems</c> içine DEĞİŞMEZ snapshot olarak kopyalanır (§60);
    /// bu tablo yalnızca çalışma alanıdır ve sipariş sonrası temizlenir.
    /// </summary>
    public class CartsProduct
    {
        /// <summary>Sepet satırının benzersiz kimliği.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Sepetin sahibi (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Ürünün satıcısı (Store.Id) — sepetin mağaza bazlı gruplanması için.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Sepete eklenen ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Seçilen varyant (ProductVariants.Id).</summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>Ürünün vitrin adresi (kullanıcının gördüğü dildeki slug) — hızlı bağlantı için.</summary>
        public string? ProductSlug { get; set; }

        /// <summary>Sepete eklendiği andaki ürün bilgileri (fiyat değişimi uyarısı için).</summary>
        public CartProductSnapshot? ProductSnapshot { get; set; }

        /// <summary>Sepete eklenme anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Satırın son güncellenme anı (UTC) — adet değişikliği vb.</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Soft delete durumu.</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>
        /// İyimser eşzamanlılık belirteci (§44). Aynı sepeti iki sekmede açan
        /// kullanıcının adet güncellemelerinin birbirini ezmesini önler.
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
