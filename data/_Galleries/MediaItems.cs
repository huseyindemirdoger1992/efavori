using data.Owned;
using System.ComponentModel.DataAnnotations;

namespace data._Galleries
{
    /// <summary>
    /// Bir medya kaydının ürün, varyant, profil, gönderi, belge veya başka bir varlığa hangi bağlam ve sunum özellikleriyle bağlandığını tutar.
    /// </summary>
    public class MediaItems
    {
        // Medya ilişkisinin benzersiz kimliğidir.
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Bu medya ilişkisinin sahibi veya ilişkiyi oluşturan kullanıcıyı belirtir.
        public Guid? UserId { get; set; }

        // Medyanın bağlandığı gerçek varlığın benzersiz kimliğini belirtir.
        public Guid? ItemId { get; set; }

        // ItemId değerinin hangi entity türüne ait olduğunu belirtir ve Product, ProductVariant, User, Post veya Document gibi değerler içerebilir.
        [MaxLength(100)]
        public string? ItemType { get; set; }

        // İlişkilendirilen medya kaydının benzersiz kimliğini belirtir.
        public Guid MediaId { get; set; }

        // Medyanın ilgili varlık içindeki kullanım amacını belirtir ve Gallery, Cover, Avatar, Thumbnail, Attachment veya Content gibi değerler içerebilir.
        [MaxLength(50)]
        public string? MediaRole { get; set; }

        // Medyanın ilgili varlığın ana veya kapak medyası olup olmadığını belirtir.
        public bool IsPrimary { get; set; } = false;

        // Medyanın galeri veya medya listesi içerisindeki gösterim sırasını belirtir.
        public int SortOrder { get; set; } = 0;

        // Medyanın ilgili varlık içinde gösterilip gösterilmeyeceğini belirtir.
        public bool IsVisible { get; set; } = true;

        // Medyanın HTML img alt niteliğinde veya erişilebilirlik araçlarında kullanılacak alternatif metnini tutar.
        [MaxLength(1000)]
        public string? AltText { get; set; }

        // Medyanın galeri, sosyal gönderi veya içerik altında kullanıcıya gösterilecek açıklamasını tutar.
        [MaxLength(4000)]
        public string? Caption { get; set; }

        // Medya için ilgili varlık bağlamında gösterilecek başlığı tutar.
        [MaxLength(500)]
        public string? Title { get; set; }

        // Medyanın ilgili varlık içerisinde özel bir bağlantıya yönlendirilmesi gerektiğinde kullanılacak URL adresini tutar.
        [MaxLength(2048)]
        public string? LinkUrl { get; set; }

        // Medyanın ilgili varlık içerisinde yatay odak noktasını 0 ile 1 arasındaki oranla belirtir.
        public decimal? FocalPointX { get; set; }

        // Medyanın ilgili varlık içerisinde dikey odak noktasını 0 ile 1 arasındaki oranla belirtir.
        public decimal? FocalPointY { get; set; }

        // Medyanın ilgili varlık içinde belirli bir kırpma veya sunum ayarına sahip olması gerektiğinde kullanılacak JSON bilgisini tutar.
        public string? CropDataJson { get; set; }

        // Medyanın ilişkiye eklendiği tarihi UTC olarak belirtir.
        public DateTime ItemAddDate { get; set; } = DateTime.UtcNow;

        // Medya ilişkisinin son güncellendiği tarihi UTC olarak belirtir.
        public DateTime? UpdatedAt { get; set; }

        // Medyanın bu ilişkiden soft-delete edilme durumunu tutar.
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}