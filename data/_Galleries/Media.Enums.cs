using System;

namespace data._Galleries
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Merkezî Medya Sistemi (Media V2)
    //  Enum kataloğu — tinyint (byte) olarak saklanır.
    //  KURAL: Değerlerin ARASINA ekleme yapılmaz; yalnızca SONA eklenir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Medya asset'inin genel türü. Eski <c>string MediaType</c> alanının yerini alır
    /// ("Image", "image", "IMAGE" gibi yazım farklarının veriyi bölmesini engeller).
    /// </summary>
    public enum MediaAssetType : byte
    {
        /// <summary>Görsel (jpg, png, webp, avif, gif).</summary>
        Image = 1,

        /// <summary>Video dosyası (mp4, webm, mov).</summary>
        Video = 2,

        /// <summary>Ses dosyası (mp3, m4a, ogg).</summary>
        Audio = 3,

        /// <summary>Belge (pdf, docx, xlsx).</summary>
        Document = 4,

        /// <summary>Arşiv (zip, rar, 7z) — toplu içe aktarım dosyaları.</summary>
        Archive = 5,

        /// <summary>Harici video gömme (YouTube, Vimeo) — fiziksel dosya yoktur.</summary>
        VideoEmbed = 6,

        /// <summary>3B model (glb, usdz) — ürün önizlemesi.</summary>
        Model3D = 7,

        /// <summary>Yukarıdakilerin hiçbirine girmeyen dosya türü.</summary>
        Other = 100
    }

    /// <summary>
    /// Medyanın arka planda işlenme durumu (yeniden boyutlandırma, transcode, tarama).
    /// Eski <c>string ProcessingStatus</c> alanının yerini alır ve işleme kuyruğunun
    /// durum makinesidir.
    /// </summary>
    public enum MediaProcessingStatus : byte
    {
        /// <summary>Yüklendi, işleme kuyruğunda bekliyor.</summary>
        Pending = 1,

        /// <summary>İşleniyor (bir arka plan servisi lease almış durumda).</summary>
        Processing = 2,

        /// <summary>İşleme tamamlandı; tüm türevler (rendition) hazır.</summary>
        Ready = 3,

        /// <summary>İşleme başarısız oldu; <c>ProcessingError</c> alanına bakılır.</summary>
        Failed = 4,

        /// <summary>Virüs/uygunsuz içerik taramasından geçemedi; erişime KAPALIDIR.</summary>
        Quarantined = 5,

        /// <summary>Kaynak dosya depolamadan kaldırıldı (arşivlendi); yalnızca meta veri durur.</summary>
        Archived = 6
    }

    /// <summary>
    /// Bir medya kullanımının (MediaItems) bağlı olduğu varlık türü.
    /// Eski <c>string ItemType</c> alanının yerini alır.
    ///
    /// NOT (§42): Bu polimorfik ilişki BİLİNÇLİ bir tercihtir — merkezî medya
    /// kütüphanesi doğası gereği her domaine bağlanabilmelidir. Buna karşılık
    /// sıcak yolda olan ve referans bütünlüğü kritik olan bağlantılar için AYRICA
    /// gerçek FK'li özel tablolar vardır: <c>PostMedia</c>, <c>ProductReviewMedia</c>,
    /// <c>ChatMessageMedia</c>, <c>StoreDocuments</c>. MediaItems bu tabloların yerine
    /// GEÇMEZ; genel kütüphane/kullanım katmanıdır.
    /// </summary>
    public enum MediaOwnerType : byte
    {
        /// <summary>Ürün (Products.Id).</summary>
        Product = 1,

        /// <summary>Ürün varyantı (ProductVariants.Id).</summary>
        ProductVariant = 2,

        /// <summary>Marka (Brands.Id).</summary>
        Brand = 3,

        /// <summary>Ürün kategorisi (CategoriesProduct.Id).</summary>
        ProductCategory = 4,

        /// <summary>Kullanıcı profili (Users.Id).</summary>
        User = 5,

        /// <summary>Mağaza (Store.Id).</summary>
        Store = 6,

        /// <summary>Sosyal gönderi (Posts.Id).</summary>
        Post = 7,

        /// <summary>Gönderi yorumu (PostComments.Id).</summary>
        PostComment = 8,

        /// <summary>Makale (Articles.Id).</summary>
        Article = 9,

        /// <summary>Ürün yorumu (ProductReviews.Id).</summary>
        ProductReview = 10,

        /// <summary>Ürün sorusu/cevabı (ProductQuestions.Id).</summary>
        ProductQuestion = 11,

        /// <summary>Sohbet mesajı eki (ChatMessages.Id).</summary>
        ChatMessage = 12,

        /// <summary>Sipariş faturası (OrderInvoices.Id).</summary>
        OrderInvoice = 13,

        /// <summary>Sevkiyat belgesi/etiketi (Shipments.Id).</summary>
        Shipment = 14,

        /// <summary>İade talebi kanıtı (ReturnRequests.Id).</summary>
        ReturnRequest = 15,

        /// <summary>İhtilaf eki (Disputes.Id).</summary>
        Dispute = 16,

        /// <summary>Kampanya görseli (Campaigns.Id).</summary>
        Campaign = 17,

        /// <summary>Kupon görseli (Coupons.Id).</summary>
        Coupon = 18,

        /// <summary>Bildirim görseli (Notifications.Id).</summary>
        Notification = 19,

        /// <summary>Mağaza doğrulama belgesi (StoreDocuments.Id).</summary>
        StoreDocument = 20,

        /// <summary>Toplu içe aktarım kaynak dosyası (ImportJob.Id).</summary>
        ImportJob = 21,

        /// <summary>Destek talebi eki (SupportTickets.Id).</summary>
        SupportTicket = 22,

        /// <summary>Hakediş dekontu (SellerPayouts.Id).</summary>
        SellerPayout = 23,

        /// <summary>Şikâyet kanıtı (ContentReports.Id).</summary>
        ContentReport = 24,

        /// <summary>Sisteme ait genel içerik (CMS sayfası, e-posta şablonu).</summary>
        SystemContent = 100
    }

    /// <summary>
    /// Medyanın bağlı olduğu varlık içindeki KULLANIM AMACI.
    /// Eski <c>string MediaRole</c> alanının yerini alır.
    /// </summary>
    public enum MediaRole : byte
    {
        /// <summary>Kapak görseli — listelerde ve önizlemelerde kullanılan tek görsel.</summary>
        Cover = 1,

        /// <summary>Galeri öğesi — sıralı çoklu görsel.</summary>
        Gallery = 2,

        /// <summary>Profil fotoğrafı / logo.</summary>
        Avatar = 3,

        /// <summary>Kapak (banner) görseli — geniş formatlı üst görsel.</summary>
        Banner = 4,

        /// <summary>Küçük önizleme görseli.</summary>
        Thumbnail = 5,

        /// <summary>Ek dosya (mesaj eki, talep eki).</summary>
        Attachment = 6,

        /// <summary>Video içeriği.</summary>
        Video = 7,

        /// <summary>Ses içeriği.</summary>
        Audio = 8,

        /// <summary>Belge içeriği.</summary>
        Document = 9,

        /// <summary>Zengin metin içine gömülü medya (içerik gövdesinde).</summary>
        InlineContent = 10,

        /// <summary>Resmî belge / sertifika (mağaza doğrulama).</summary>
        Certificate = 11,

        /// <summary>Teslimat kanıtı (imza/fotoğraf).</summary>
        ProofOfDelivery = 12,

        /// <summary>Fatura çıktısı.</summary>
        Invoice = 13,

        /// <summary>Kargo etiketi (PDF/ZPL).</summary>
        ShippingLabel = 14,

        /// <summary>Şikâyet/ihtilaf kanıtı.</summary>
        Evidence = 15,

        /// <summary>3B model dosyası.</summary>
        Model3D = 16,

        /// <summary>Beden/ölçü tablosu görseli.</summary>
        SizeChart = 17
    }

    /// <summary>
    /// Medyanın erişim düzeyi (§61). Eski <c>bool IsPublic</c> alanının yerini alır;
    /// "herkese açık değil" ile "yalnızca sahibi ve yönetim görebilir" arasındaki fark
    /// güvenlik açısından kritiktir.
    ///
    /// SERVİS KATMANI KURALI: Dosya sunucusu (media endpoint) her istekte bu alanı
    /// kontrol etmeli, Private/Restricted medyayı imzalı ve süreli URL ile sunmalıdır.
    /// Mağaza doğrulama belgeleri ve fatura PDF'leri ASLA Public olmamalıdır.
    /// </summary>
    public enum MediaVisibility : byte
    {
        /// <summary>Herkese açık — CDN üzerinden doğrudan sunulabilir.</summary>
        Public = 1,

        /// <summary>Yalnızca yükleyen kullanıcı (ve yönetim) erişebilir.</summary>
        Private = 2,

        /// <summary>
        /// Kısıtlı — yalnızca ilişkili varlığa erişim yetkisi olanlar görebilir
        /// (ör. siparişin fatura PDF'ini yalnızca alıcı ve satıcı görür).
        /// </summary>
        Restricted = 3,

        /// <summary>Yalnızca platform personeli/moderasyon erişebilir (KYC belgeleri).</summary>
        Internal = 4
    }
}
