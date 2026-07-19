using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// Bir attribute'ın hangi kategoriye, hangi grup içinde, hangi sırada ve
    /// hangi kurallarla bağlandığını tutar. (TaskKeeperJoint deseni)
    /// Bir attribute birden fazla kategoriye bağlanabilir ve her kategoride
    /// farklı sıra / zorunluluk / grup değeri alabilir.
    /// Satıcı ürün eklerken kategori seçtiğinde, form bu tablodan dinamik üretilir.
    /// </summary>
    public class AttributeCategoryJoint
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Bağlanan attribute ID'si (AttributeDefinition tablosu).
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        /// Bağlanan kategori ID'si (CategoriesProduct tablosu).
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Bu kategoride attribute'ın görüneceği grup ID'si (AttributeGroup tablosu).
        /// Null ise "Genel" kabul edilir.
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Bu kategoriye özel gösterim sırası. Aynı attribute farklı kategorilerde
        /// farklı sıralarda gösterilebilir.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        // -------------------- KATEGORİ BAZLI OVERRIDE'LAR --------------------
        // Null ise AttributeDefinition'daki varsayılan değer geçerlidir.

        /// <summary>
        /// Bu kategoride zorunlu mu? Null → AttributeDefinition.IsRequiredDefault kullanılır.
        /// </summary>
        public bool? IsRequiredOverride { get; set; }

        /// <summary>
        /// Bu kategoride filtre olarak kullanılsın mı? Null → tanımdaki değer geçerli.
        /// </summary>
        public bool? IsFilterableOverride { get; set; }

        /// <summary>
        /// Bu kategoride varyant ekseni olarak kullanılsın mı? Null → tanımdaki değer geçerli.
        /// Örn: "Renk" giyimde varyant, mobilyada sadece bilgi alanı olabilir.
        /// </summary>
        public bool? IsVariantOverride { get; set; }

        /// <summary>
        /// Bu kategoride görünür mü? Null → tanımdaki değer geçerli.
        /// </summary>
        public bool? IsVisibleOverride { get; set; }

        // -------------------- MİRAS --------------------

        /// <summary>
        /// true ise bu bağ, kategorinin tüm alt kategorilerine de uygulanır.
        /// Form üretilirken kategori ağacı yukarı doğru gezilir (BFS/parent zinciri)
        /// ve InheritToChildren = true olan üst bağlar da forma dahil edilir.
        /// Böylece "Elektronik" seviyesinde tanımlanan "Marka" tüm alt dallara iner.
        /// </summary>
        public bool InheritToChildren { get; set; } = true;

        /// <summary>Aktif / Pasif. Pasif yapılırsa satıcı formunda görünmez, mevcut değerler silinmez.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bu bağ AI kategori analizi tarafından mı oluşturuldu?
        /// </summary>
        public bool? CreatedByAi { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
