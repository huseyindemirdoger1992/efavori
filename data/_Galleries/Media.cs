using System;
using data.Owned;

namespace data._Galleries
{
    /// <summary>
    /// MERKEZÎ MEDYA ASSET'İ (§14, §72) — platformdaki TEK fiziksel medya deposu.
    ///
    /// KESİN MİMARİ KURAL:
    /// Görsel, video, ses ve dosya asset'i YALNIZCA bu tabloda tutulur. Product, Post,
    /// Review, User, Store, Chat, Notification gibi domainler KENDİ fiziksel medya
    /// kolonlarını (ImageUrl, CoverImage, ProfileImagePath...) AÇMAZ. Bu domainler
    /// medyaya yalnızca <c>MediaId</c> ile referans verir; kullanım/sunum bilgisi
    /// <see cref="MediaItems"/> veya domaine özel ilişki tablolarında
    /// (PostMedia, ProductReviewMedia, ChatMessageMedia, StoreDocuments) durur.
    ///
    /// Bu sayede aynı asset — tek bir fiziksel dosya — profil fotoğrafı, ürün görseli,
    /// gönderi görseli, yorum eki ve mağaza banner'ı olarak aynı anda kullanılabilir;
    /// depolama tekrarı ve tutarsız URL'ler ortadan kalkar.
    ///
    /// Bu sürümde (V2) değişenler:
    ///  • <c>MediaType</c> ve <c>ProcessingStatus</c> string'den ENUM'a çevrildi.
    ///  • <c>IsPublic</c> boolean'ı <see cref="MediaVisibility"/> enum'ına dönüştü.
    ///  • Denetim (audit), eşzamanlılık (RowVersion) ve güvenlik taraması alanları eklendi.
    /// </summary>
    public class Media
    {
        /// <summary>Medyanın benzersiz kimliği.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Medyayı sisteme yükleyen kullanıcı (Users.Id).</summary>
        public Guid? UserId { get; set; }

        /// <summary>Asset'in ait olduğu mağaza (Store.Id) — satıcı medya kütüphanesi ve kota denetimi için.</summary>
        public Guid? OwnerStoreId { get; set; }

        // ── Dosya kimliği ─────────────────────────────────────────────────────
        /// <summary>Kullanıcının yüklediği orijinal dosya adı.</summary>
        public string? FileName { get; set; }

        /// <summary>Depolama sisteminde kullanılan benzersiz fiziksel dosya adı.</summary>
        public string? FileStoredName { get; set; }

        /// <summary>Medyanın genel türü.</summary>
        public MediaAssetType MediaType { get; set; } = MediaAssetType.Image;

        /// <summary>MIME içerik türü ("image/jpeg", "video/mp4", "application/pdf").</summary>
        public string? ContentType { get; set; }

        /// <summary>Dosya uzantısı ("jpg", "png", "mp4", "pdf") — noktasız ve küçük harf.</summary>
        public string? FileExtensionType { get; set; }

        // ── Depolama ──────────────────────────────────────────────────────────
        /// <summary>Depolama sağlayıcısı ("Local", "AzureBlob", "S3", "Cloudflare R2").</summary>
        public string? StorageProvider { get; set; }

        /// <summary>Bucket / container / storage alanı adı.</summary>
        public string? StorageBucket { get; set; }

        /// <summary>Depolama sistemindeki benzersiz nesne anahtarı.</summary>
        public string? StorageKey { get; set; }

        /// <summary>Aktif olarak kullanılan (optimize edilmiş) public URL.</summary>
        public string? FileUrl { get; set; }

        /// <summary>Orijinal, değiştirilmemiş dosyanın public URL'i.</summary>
        public string? OrjFileUrl { get; set; }

        /// <summary>Sunucu üzerindeki aktif fiziksel yol.</summary>
        public string? FilePhysicalPathRoad { get; set; }

        /// <summary>Orijinal dosyanın fiziksel yolu.</summary>
        public string? OrjFilePhysicalPathRoad { get; set; }

        // ── Türevler (rendition) ──────────────────────────────────────────────
        /// <summary>1/2 ölçekli türev URL'i.</summary>
        public string? FileUrl_Ratio_1_2 { get; set; }

        /// <summary>1/4 ölçekli türev URL'i.</summary>
        public string? FileUrl_Ratio_1_4 { get; set; }

        /// <summary>1/8 ölçekli türev URL'i.</summary>
        public string? FileUrl_Ratio_1_8 { get; set; }

        /// <summary>1/16 ölçekli türev URL'i.</summary>
        public string? FileUrl_Ratio_1_16 { get; set; }

        /// <summary>
        /// Ek türevler (JSON). JSON KULLANIMI BURADA MEŞRUDUR (§46): şema değiştirmeden
        /// yeni boyut/format (avif, webp, hls) eklenebilmesi gereken esnek meta veridir,
        /// ilişkisel veri değildir.
        /// </summary>
        public string? RenditionsJson { get; set; }

        // ── Boyut ve bütünlük ─────────────────────────────────────────────────
        /// <summary>Orijinal dosya boyutu (byte).</summary>
        public long? OriginalSize { get; set; }

        /// <summary>Optimize edilmiş dosya boyutu (byte).</summary>
        public long? CompressedSize { get; set; }

        /// <summary>SHA-256 özeti — aynı dosyanın tekrar yüklenmesini tespit eder (dedup).</summary>
        public string? Sha256 { get; set; }

        /// <summary>Depolama katmanının ETag/bütünlük bilgisi.</summary>
        public string? ETag { get; set; }

        // ── Görsel / video teknik meta verisi ─────────────────────────────────
        /// <summary>Piksel genişliği.</summary>
        public int? Width { get; set; }

        /// <summary>Piksel yüksekliği.</summary>
        public int? Height { get; set; }

        /// <summary>Süre (milisaniye) — video/ses için.</summary>
        public long? DurationMs { get; set; }

        /// <summary>Kare hızı (fps).</summary>
        public decimal? FrameRate { get; set; }

        /// <summary>Bit hızı (bit/saniye).</summary>
        public long? Bitrate { get; set; }

        /// <summary>Codec bilgisi ("h264", "vp9", "aac").</summary>
        public string? Codec { get; set; }

        /// <summary>Sayfa sayısı — çok sayfalı belgeler için.</summary>
        public int? PageCount { get; set; }

        /// <summary>Görselde şeffaflık kanalı var mı?</summary>
        public bool? HasAlpha { get; set; }

        /// <summary>EXIF yön bilgisi.</summary>
        public short? Orientation { get; set; }

        /// <summary>BlurHash / düşük çözünürlüklü placeholder verisi.</summary>
        public string? BlurHash { get; set; }

        /// <summary>Baskın renk (hex) — placeholder ve tema uyumu için.</summary>
        public string? DominantColor { get; set; }

        // ── Harici sağlayıcı ──────────────────────────────────────────────────
        /// <summary>Harici medya URL'i (YouTube, Vimeo). MediaType = VideoEmbed iken dolu.</summary>
        public string? ExternalUrl { get; set; }

        /// <summary>Harici sağlayıcı adı ("YouTube", "Vimeo", "TikTok").</summary>
        public string? ExternalProvider { get; set; }

        /// <summary>Harici sağlayıcıdaki video kimliği.</summary>
        public string? ExternalVideoId { get; set; }

        // ── İşleme durumu ─────────────────────────────────────────────────────
        /// <summary>Arka plan işleme durumu.</summary>
        public MediaProcessingStatus ProcessingStatus { get; set; } = MediaProcessingStatus.Pending;

        /// <summary>İşleme başarısız olduysa hata mesajı.</summary>
        public string? ProcessingError { get; set; }

        /// <summary>İşlemenin tamamlandığı an (UTC).</summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>Otomatik/AI içerik işlemesinden geçirildi mi? (etiketleme, alt metin üretimi)</summary>
        public bool AiProcessed { get; set; }

        // ── Güvenlik (§61) ────────────────────────────────────────────────────
        /// <summary>Erişim düzeyi. Belgeler ve faturalar ASLA Public olmamalıdır.</summary>
        public MediaVisibility Visibility { get; set; } = MediaVisibility.Public;

        /// <summary>Zararlı yazılım taramasından geçti mi?</summary>
        public bool IsVirusScanned { get; set; }

        /// <summary>Taramanın yapıldığı an (UTC).</summary>
        public DateTime? VirusScannedAtUtc { get; set; }

        /// <summary>Otomatik içerik denetiminde uygunsuz bulundu mu? (NSFW/şiddet)</summary>
        public bool IsFlaggedByModeration { get; set; }

        /// <summary>Moderasyon puanı (0.00–1.00) — otomatik içerik denetimi skoru.</summary>
        public decimal? ModerationScore { get; set; }

        // ── Denetim ───────────────────────────────────────────────────────────
        /// <summary>Oluşturulma anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (Users.Id).</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (Users.Id).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>
        /// Bu asset'in kaç yerde kullanıldığı (önbellek). Kaynak: <see cref="MediaItems"/>
        /// ve domaine özel medya tabloları. 0 ise fiziksel dosya güvenle temizlenebilir.
        /// </summary>
        public int UsageCount { get; set; }

        /// <summary>Soft delete durumu. Fiziksel dosya, kullanım sayısı 0 olunca temizlenir.</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>İyimser eşzamanlılık belirteci (SQL Server rowversion).</summary>
        public byte[]? RowVersion { get; set; }
    }
}
