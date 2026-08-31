using System;
using data._Attribute;
using data.Owned;

namespace data._Galleries
{
    /// <summary>
    /// MEDYA KULLANIM KAYDI (media usage) — bir <see cref="Media"/> asset'inin hangi
    /// varlıkta, hangi rolle ve hangi sunum ayarlarıyla kullanıldığını tutar.
    ///
    /// BU TABLO ASSET BARINDIRMAZ (§14). Fiziksel dosyaya ait hiçbir alan burada yoktur;
    /// yalnızca <see cref="MediaId"/> ile merkezî depoya bağlanır. Aynı asset farklı
    /// satırlarla profil fotoğrafı, ürün görseli, gönderi görseli ve mağaza banner'ı
    /// olarak eş zamanlı kullanılabilir.
    ///
    /// ÖZEL TABLOLARLA İLİŞKİSİ: Sıcak yolda olan ve referans bütünlüğü kritik olan
    /// bağlantılar için ayrıca gerçek FK'li tablolar vardır (<c>PostMedia</c>,
    /// <c>ProductReviewMedia</c>, <c>ChatMessageMedia</c>, <c>StoreDocuments</c>).
    /// MediaItems onların yerine geçmez; genel medya kütüphanesi/kullanım katmanıdır
    /// ve "bu asset nerelerde kullanılıyor?" sorusunu tek sorguda yanıtlar.
    ///
    /// V2'de değişenler: <c>ItemType</c> ve <c>MediaRole</c> string'den ENUM'a çevrildi,
    /// erişim düzeyi ve çok dilli alt metin desteği eklendi.
    /// </summary>
    public class MediaItems
    {
        /// <summary>Kullanım kaydının benzersiz kimliği.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>İlişkiyi kuran kullanıcı (Users.Id).</summary>
        public Guid? UserId { get; set; }

        /// <summary>Medyanın bağlandığı varlığın kimliği.</summary>
        public Guid ItemId { get; set; }

        /// <summary>Bağlanılan varlığın türü. Eski <c>string ItemType</c> yerine geçer.</summary>
        public MediaOwnerType ItemType { get; set; }

        /// <summary>İlişkilendirilen medya asset'i (Media.Id).</summary>
        public Guid MediaId { get; set; }

        /// <summary>Medyanın bu varlık içindeki kullanım amacı.</summary>
        public MediaRole MediaRole { get; set; } = MediaRole.Gallery;

        /// <summary>Bu varlığın ana/kapak medyası mı? Varlık başına en fazla bir tane true.</summary>
        public bool IsPrimary { get; set; }

        /// <summary>Galeri içindeki gösterim sırası (0 tabanlı).</summary>
        public int SortOrder { get; set; }

        /// <summary>Bu kullanım şu anda gösteriliyor mu? (asset'i silmeden gizlemek için)</summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Bu KULLANIMIN erişim düzeyi. Asset'in kendi düzeyinden (Media.Visibility)
        /// daha geniş OLAMAZ; servis katmanı ikisinin en kısıtlayıcısını uygular.
        /// </summary>
        public MediaVisibility Visibility { get; set; } = MediaVisibility.Public;

        // ── SEO / erişilebilirlik (§56) ───────────────────────────────────────
        /// <summary>HTML <c>alt</c> niteliği — erişilebilirlik ve görsel SEO için.</summary>
        public string? AltText { get; set; }

        /// <summary>Kullanıcıya gösterilecek açıklama metni.</summary>
        public string? Caption { get; set; }

        /// <summary>Bu bağlamda gösterilecek başlık.</summary>
        public string? Title { get; set; }

        /// <summary>
        /// Metin alanlarının dili (mevcut data._Attribute.Language enum'ı).
        /// Aynı asset farklı dillerde farklı alt metinle kullanılabilir:
        /// (ItemType, ItemId, MediaId, MediaRole, Language) tekildir.
        /// </summary>
        public Language Language { get; set; } = Language.Tr;

        /// <summary>Medyaya tıklandığında gidilecek adres (banner/kampanya kullanımı).</summary>
        public string? LinkUrl { get; set; }

        // ── Sunum ayarları ────────────────────────────────────────────────────
        /// <summary>Yatay odak noktası (0.00–1.00) — otomatik kırpmada merkez.</summary>
        public decimal? FocalPointX { get; set; }

        /// <summary>Dikey odak noktası (0.00–1.00).</summary>
        public decimal? FocalPointY { get; set; }

        /// <summary>
        /// Kırpma/sunum ayarları (JSON). JSON KULLANIMI BURADA MEŞRUDUR (§46):
        /// serbest biçimli, sorgulanmayan sunum yapılandırmasıdır.
        /// </summary>
        public string? CropDataJson { get; set; }

        // ── Denetim ───────────────────────────────────────────────────────────
        /// <summary>İlişkinin kurulduğu an (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (Users.Id).</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (Users.Id).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft delete durumu.</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>İyimser eşzamanlılık belirteci (SQL Server rowversion).</summary>
        public byte[]? RowVersion { get; set; }
    }
}
