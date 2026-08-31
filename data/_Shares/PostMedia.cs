using System;
using data._Galleries;

namespace data._Shares
{
    /// <summary>
    /// GÖNDERİ–MEDYA İLİŞKİSİ (§13).
    ///
    /// Gönderinin fiziksel medya alanı (CoverImage, ImageUrl, MediaPath) YOKTUR;
    /// tüm medya merkezî <c>data._Galleries.Media</c> deposundan gelir ve buraya
    /// GERÇEK FK ile bağlanır. Bu tablo yalnızca ilişki ve sunum katmanıdır — asset
    /// barındırmaz.
    ///
    /// NEDEN MediaItems YETMİYOR: MediaItems polimorfiktir (ItemId için FK yoktur).
    /// Gönderi medyası akışın en sıcak sorgusudur ve referans bütünlüğü kritiktir;
    /// bu yüzden gönderiler için gerçek FK'li özel tablo kullanılır. Aynı yaklaşım
    /// ProductReviewMedia, ChatMessageMedia ve StoreDocuments için de geçerlidir.
    ///
    /// Tek gönderide 1 fotoğraf, 10 fotoğraf, video, ses ve belge birlikte
    /// kullanılabilir; sıralama <see cref="DisplayOrder"/> ile belirlenir.
    /// </summary>
    public class PostMedia : SocialEntityBase
    {
        /// <summary>Bağlı gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Merkezî medya asset'i (Media.Id).</summary>
        public Guid MediaId { get; set; }

        /// <summary>Medyanın gönderi içindeki rolü (Cover / Gallery / Video / Attachment...).</summary>
        public MediaRole MediaRole { get; set; } = MediaRole.Gallery;

        /// <summary>Karusel/galeri içindeki gösterim sırası (0 tabanlı).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Gönderinin kapak/önizleme medyası mı? Gönderi başına en fazla bir tane true.</summary>
        public bool IsCover { get; set; }

        /// <summary>
        /// Erişilebilirlik alt metni. Merkezî medyanın genel alt metninden farklı
        /// olabilir (aynı görsel farklı gönderilerde farklı anlam taşıyabilir).
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>Bu medyaya özel açıklama (karusel altı metni).</summary>
        public string? Caption { get; set; }

        /// <summary>Hassas içerik olarak işaretlendi mi? (bulanıklaştırılarak gösterilir)</summary>
        public bool IsSensitive { get; set; }

        /// <summary>Videonun otomatik oynatılmasına izin verilsin mi?</summary>
        public bool AllowAutoPlay { get; set; } = true;
    }
}
