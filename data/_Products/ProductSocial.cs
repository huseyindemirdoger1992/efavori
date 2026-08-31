using System;

namespace data._Products
{
    /// <summary>
    /// ÜRÜN FAVORİSİ (§25, §29).
    ///
    /// Kullanıcının "kalp" ikonuyla işaretlediği hızlı favori listesidir. Tek ve
    /// isimsiz bir listedir; birden çok adlandırılmış liste için
    /// <see cref="Wishlists"/> kullanılır. İkisi AYNI ŞEY DEĞİLDİR ve bilinçli
    /// olarak ayrı tutulmuştur:
    ///  • Favori  = tek tıkla işaretleme, tek liste, fiyat düşünce bildirim.
    ///  • Wishlist = adlandırılmış, paylaşılabilir, öncelikli, adetli koleksiyon.
    ///
    /// Bu tablo eski <c>CartsFavorite</c> entity'sinin yerini alır. <c>CartsFavorite</c>
    /// bir sepet satırı gibi tasarlanmıştı (fiyat/kargo snapshot'ı taşıyordu) ve
    /// favori kavramı için hem gereksiz genişti hem de yanıltıcıydı.
    ///
    /// TEKİLLİK: (UserId, ProductId).
    /// </summary>
    public class ProductFavorites : ProductEntityBase
    {
        /// <summary>Favorileyen kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Favorilenen ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>İlgilenilen varyant (ProductVariants.Id) — opsiyonel.</summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>Ürünün satıcısı (Store.Id) — DENORMALIZE, satıcı analitiği için.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Fiyat düştüğünde bildirim gönderilsin mi?</summary>
        public bool NotifyOnPriceDrop { get; set; } = true;

        /// <summary>Ürün yeniden stoğa girdiğinde bildirim gönderilsin mi?</summary>
        public bool NotifyOnBackInStock { get; set; } = true;

        /// <summary>
        /// Favorilendiği andaki fiyat (kullanıcının para biriminde).
        /// "Favorine eklediğinden beri %20 ucuzladı" bildirimi bu değerle hesaplanır.
        /// </summary>
        public decimal? PriceAtFavoriteTime { get; set; }

        /// <summary>Yukarıdaki fiyatın para birimi.</summary>
        public CurrencyCode? PriceCurrency { get; set; }
    }

    /// <summary>
    /// ÜRÜNE TEPKİ (§25).
    ///
    /// Ürün sayfasındaki hafif sosyal etkileşimdir (beğeni/ilgi). Yorumdan farklıdır:
    /// metin ve moderasyon gerektirmez, satın alma şartı yoktur ve ürün puanını
    /// ETKİLEMEZ — puanlama yalnızca <see cref="ProductReviews"/> üzerinden yapılır.
    /// Bu ayrım, sahte etkileşimin ürün puanını bozmasını engeller.
    ///
    /// TEKİLLİK: (UserId, ProductId) — kullanıcı başına tek aktif tepki.
    /// </summary>
    public class ProductReactions : ProductEntityBase
    {
        /// <summary>Tepkiyi veren kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Tepki verilen ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Tepki türü — sosyal modülün ortak sözlüğü kullanılır
        /// (data._Shares.ReactionType). Byte olarak saklanır.
        /// </summary>
        public data._Shares.ReactionType ReactionType { get; set; } = data._Shares.ReactionType.Like;

        /// <summary>Tepkinin en son değiştirildiği an (UTC).</summary>
        public DateTime? ChangedAtUtc { get; set; }
    }

    /// <summary>
    /// ÜRÜN PAYLAŞIMI (§25) — pazarlama dönüşüm ölçümü.
    ///
    /// Tekil değildir: aynı kullanıcı aynı ürünü farklı kanallardan defalarca
    /// paylaşabilir; her paylaşım ayrı bir olaydır.
    /// </summary>
    public class ProductShares : ProductEntityBase
    {
        /// <summary>Paylaşan kullanıcı (Users.Id). Anonim paylaşımda null.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Paylaşılan ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Paylaşım kanalı.</summary>
        public ProductShareChannel Channel { get; set; } = ProductShareChannel.CopyLink;

        /// <summary>Platform içi paylaşımda hedef sohbet (ChatConversations.Id).</summary>
        public Guid? TargetConversationId { get; set; }

        /// <summary>Paylaşımla oluşturulan gönderi (Posts.Id) — sosyal paylaşımda.</summary>
        public Guid? CreatedPostId { get; set; }

        /// <summary>
        /// Bu paylaşım bağlantısından gelen tıklama sayısı (önbellek).
        /// Yönlendirme (referral) ölçümü için.
        /// </summary>
        public int ClickCount { get; set; }
    }

    /// <summary>
    /// İSTEK LİSTESİ (§29) — adlandırılmış, paylaşılabilir ürün koleksiyonu.
    ///
    /// Basit favoriden (<see cref="ProductFavorites"/>) farkıdır: kullanıcı birden
    /// çok liste oluşturabilir, listeye ad ve amaç verebilir, paylaşabilir ve
    /// kalemlere adet/öncelik/not ekleyebilir. Düğün ve doğum günü hediye listesi
    /// senaryoları bu tabloyla desteklenir.
    /// </summary>
    public class Wishlists : ProductEntityBase
    {
        /// <summary>Listenin sahibi (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Liste adı ("Yaz Tatili", "Düğün Listemiz").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Liste açıklaması.</summary>
        public string? Description { get; set; }

        /// <summary>Listenin amacı.</summary>
        public WishlistPurpose Purpose { get; set; } = WishlistPurpose.General;

        /// <summary>Görünürlük.</summary>
        public WishlistVisibility Visibility { get; set; } = WishlistVisibility.Private;

        /// <summary>
        /// Paylaşım için üretilen rastgele ve tahmin edilemez anahtar.
        /// Visibility = LinkOnly iken adres bu değerle oluşturulur.
        /// </summary>
        public string? ShareToken { get; set; }

        /// <summary>Kullanıcının varsayılan listesi mi? Kullanıcı başına en fazla bir tane true.</summary>
        public bool IsDefault { get; set; }

        /// <summary>Etkinlik tarihi (düğün, doğum günü) — hediye listelerinde.</summary>
        public DateOnly? EventDate { get; set; }

        /// <summary>Liste kapak görseli (data._Galleries.Media.Id).</summary>
        public Guid? CoverMediaId { get; set; }

        /// <summary>Listedeki kalem sayısı (önbellek — kaynak: WishlistItems).</summary>
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// İSTEK LİSTESİ KALEMİ (§29).
    /// TEKİLLİK: (WishlistId, ProductId, ProductVariantId).
    /// </summary>
    public class WishlistItems : ProductEntityBase
    {
        /// <summary>Bağlı liste (Wishlists.Id).</summary>
        public Guid WishlistId { get; set; }

        /// <summary>Listedeki ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>İstenen varyant (ProductVariants.Id) — beden/renk seçimi.</summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>İstenen adet.</summary>
        public int DesiredQuantity { get; set; } = 1;

        /// <summary>Öncelik (0 = normal, yükseldikçe daha çok isteniyor).</summary>
        public byte Priority { get; set; }

        /// <summary>Kullanıcının kendine veya hediye alacak kişiye notu.</summary>
        public string? Note { get; set; }

        /// <summary>Listeye eklendiği andaki fiyat — fiyat takibi için.</summary>
        public decimal? PriceAtAddTime { get; set; }

        /// <summary>Yukarıdaki fiyatın para birimi.</summary>
        public CurrencyCode? PriceCurrency { get; set; }

        /// <summary>Gösterim sırası.</summary>
        public int DisplayOrder { get; set; }

        // ── Hediye listesi akışı ──────────────────────────────────────────────
        /// <summary>Bu kalem birileri tarafından satın alındı mı? (hediye listesi)</summary>
        public bool IsPurchased { get; set; }

        /// <summary>Satın alınan adet.</summary>
        public int PurchasedQuantity { get; set; }

        /// <summary>Satın alma anı (UTC).</summary>
        public DateTime? PurchasedAtUtc { get; set; }

        /// <summary>
        /// Satın alan kullanıcı (Users.Id). Sürpriz bozulmasın diye liste sahibine
        /// GÖSTERİLMEZ; yalnızca "alındı" bilgisi gösterilir.
        /// </summary>
        public Guid? PurchasedByUserId { get; set; }
    }

    /// <summary>
    /// ÜRÜN YORUMU MEDYASI (§26) — eski <c>ProductReviews.MediaIdsJson</c> alanının
    /// ilişkisel karşılığı.
    ///
    /// NEDEN JSON'DAN İLİŞKİSEL YAPIYA GEÇİLDİ:
    ///  • JSON dizisindeki Media.Id değerleri için FOREIGN KEY kurulamıyordu; silinmiş
    ///    bir medyaya işaret eden kırık referanslar sessizce birikiyordu.
    ///  • "Bu medya nerelerde kullanılıyor?" sorusu yanıtlanamıyor, dolayısıyla
    ///    kullanılmayan dosyalar güvenle temizlenemiyordu.
    ///  • Sıralama, kapak seçimi ve moderasyon bayrağı gibi kolon başına veri
    ///    tutulamıyordu.
    ///  • "Fotoğraflı yorumlar" filtresi JSON metnini taramayı gerektiriyordu.
    ///
    /// Görseller ve videolar merkezî <c>data._Galleries.Media</c> deposundan gelir (§72).
    /// TEKİLLİK: (ProductReviewId, MediaId).
    /// </summary>
    public class ProductReviewMedia : ProductEntityBase
    {
        /// <summary>Medyanın eklendiği yorum (ProductReviews.Id).</summary>
        public Guid ProductReviewId { get; set; }

        /// <summary>Merkezî medya asset'i (data._Galleries.Media.Id).</summary>
        public Guid MediaId { get; set; }

        /// <summary>Galeri içindeki gösterim sırası (0 tabanlı).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Yorumun kapak görseli mi? Yorum başına en fazla bir tane true.</summary>
        public bool IsCover { get; set; }

        /// <summary>Erişilebilirlik alt metni.</summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Medya moderasyondan geçti mi? Kullanıcı yüklemesi olduğu için görsel,
        /// yorum metninden BAĞIMSIZ olarak denetlenir.
        /// </summary>
        public ModerationStatus MediaModerationStatus { get; set; } = ModerationStatus.Pending;
    }

    /// <summary>
    /// ÜRÜN SORU/CEVAP MEDYASI (§28) — soru veya cevaba eklenen görsel/video.
    /// TEKİLLİK: (ProductQuestionId, MediaId).
    /// </summary>
    public class ProductQuestionMedia : ProductEntityBase
    {
        /// <summary>Medyanın eklendiği soru/cevap (ProductQuestions.Id).</summary>
        public Guid ProductQuestionId { get; set; }

        /// <summary>Merkezî medya asset'i (data._Galleries.Media.Id).</summary>
        public Guid MediaId { get; set; }

        /// <summary>Gösterim sırası.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Erişilebilirlik alt metni.</summary>
        public string? AltText { get; set; }

        /// <summary>Medya moderasyon durumu.</summary>
        public ModerationStatus MediaModerationStatus { get; set; } = ModerationStatus.Pending;
    }
}
