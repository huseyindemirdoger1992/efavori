using System;

namespace data._Products
{
    /// <summary>
    /// Ürün ↔ Kategori çoklu atama tablosu (many-to-many).
    /// Bir ürün birden fazla kategoride listelenebilir (Amazon/eBay davranışı);
    /// tam olarak BİR satır IsPrimary = true olur ve Products.PrimaryCategoryId
    /// ile tutarlı tutulur (breadcrumb + attribute çözümlemesi birincilden yapılır).
    /// (ProductId, CategoryId) benzersizdir.
    /// </summary>
    public class ProductCategoryLinks : ProductEntityBase
    {
        /// <summary>Bağlı ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Bağlı kategori (CategoriesProduct.Id).</summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Birincil kategori mi? Breadcrumb, SEO ve CategoryAttribute çözümlemesi
        /// bu satırdan yapılır. Ürün başına yalnızca bir satırda true olur.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>Kategori listelemesindeki manuel sıralama önceliği (opsiyonel).</summary>
        public int DisplayOrder { get; set; }
    }
}
