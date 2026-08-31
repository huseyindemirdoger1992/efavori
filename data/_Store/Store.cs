using System;
using data._Users;
using data.Owned;

namespace data._Store
{
    /// <summary>
    /// MAĞAZA (§30, §58) — satıcının (vendor/seller) ticari kimliği VE sosyal profili.
    ///
    /// Bir mağaza yalnızca ticari bir varlık değildir; kendi adına gönderi paylaşan,
    /// takip edilen, mesajlaşan, yorumlanan bir SOSYAL AKTÖRDÜR. Bu yüzden mağaza
    /// modeli hem pazaryeri hem sosyal ağ alanlarını taşır.
    ///
    /// SAHİPLİK KURALI (§58): <see cref="OwnerUserId"/> üzerinde TEKİL indeks YOKTUR.
    /// Bir kullanıcı hiç mağazaya sahip olmayabilir, bir mağazaya sahip olabilir veya
    /// birden çok mağaza açabilir. Kullanıcı ile mağaza AYNI ENTITY DEĞİLDİR ve asla
    /// birleştirilmemelidir; satıcı olma yetkisi kullanıcı tarafında
    /// <c>Users.VendorCapability</c> alanında tutulur.
    ///
    /// BU SÜRÜMDE DÜZELTİLENLER:
    ///  • İki boolean'lı durum modeli (<c>IsActiveStateAdmin</c> / <c>IsActiveStateVendor</c>)
    ///    tek bir <see cref="StoreStatus"/> enum'ına + satıcının geçici kapatma alanına ayrıldı.
    ///  • 15 adet belge Guid kolonu <c>StoreDocuments</c> tablosuna normalize edildi.
    ///  • <c>ProfileCoverGallery</c> (fiziksel yol) kaldırıldı; avatar/kapak artık
    ///    merkezî medyaya FK'dir (§72).
    ///  • Sosyal profil alanları (handle, bio, doğrulama, sayaçlar) eklendi.
    ///  • Eşzamanlılık için RowVersion eklendi (§44).
    /// </summary>
    public class Store : StoreEntityBase
    {
        // ── Sahiplik ──────────────────────────────────────────────────────────
        /// <summary>Mağazayı açan/sahibi olan kullanıcı (Users.Id).</summary>
        public Guid OwnerUserId { get; set; }

        // ── Kimlik ────────────────────────────────────────────────────────────
        /// <summary>Vitrinde görünen mağaza adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mağaza kullanıcı adı / URL handle'ı ("efavori-elektronik").
        /// GLOBAL TEKİLDİR ve mağaza sayfası adresini oluşturur.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş handle (küçük harf) — tekillik ve arama bu kolondadır.</summary>
        public string NormalizedSlug { get; set; } = string.Empty;

        /// <summary>Resmî ticari unvan (faturada görünen). Vitrindeki addan farklı olabilir.</summary>
        public string? LegalName { get; set; }

        /// <summary>Vergi numarası. KİŞİSEL/TİCARİ HASSAS VERİ — yalnızca faturalama servisinde okunur.</summary>
        public string? TaxNumber { get; set; }

        /// <summary>Vergi dairesi.</summary>
        public string? TaxOffice { get; set; }

        /// <summary>Mersis numarası.</summary>
        public string? MersisNumber { get; set; }

        // ── Sosyal profil (§30) ───────────────────────────────────────────────
        /// <summary>Kısa tanıtım metni (profil üstünde görünen).</summary>
        public string? Bio { get; set; }

        /// <summary>Uzun "Hakkımızda" metni.</summary>
        public string? Description { get; set; }

        /// <summary>Mağaza logosu / avatarı (data._Galleries.Media.Id).</summary>
        public Guid? AvatarMediaId { get; set; }

        /// <summary>Mağaza kapak görseli / banner (data._Galleries.Media.Id).</summary>
        public Guid? CoverMediaId { get; set; }

        /// <summary>Web sitesi adresi.</summary>
        public string? Website { get; set; }

        /// <summary>İletişim bilgileri ve sosyal medya bağlantıları (mevcut ortak owned tip).</summary>
        public ContactInformation ContactInformation { get; set; } = new();

        /// <summary>Fiziksel adres ve harita konumu (mevcut ortak owned tip).</summary>
        public AddressInfo AddressInfo { get; set; } = new();

        /// <summary>Haftalık çalışma saatleri (mevcut ortak owned tip).</summary>
        public WorkingHours WorkingHours { get; set; } = new();

        /// <summary>SEO meta verileri (mevcut ortak owned tip).</summary>
        public Meta? Meta { get; set; }

        // ── Durum ─────────────────────────────────────────────────────────────
        /// <summary>Yönetim tarafından belirlenen yaşam döngüsü durumu.</summary>
        public StoreStatus Status { get; set; } = StoreStatus.Draft;

        /// <summary>Durumun en son değiştiği an (UTC).</summary>
        public DateTime? StatusChangedAtUtc { get; set; }

        /// <summary>Onaylayan/redden sorumlu yönetici (Users.Id).</summary>
        public Guid? StatusChangedByUserId { get; set; }

        /// <summary>Ret veya askıya alma gerekçesi.</summary>
        public string? StatusReason { get; set; }

        /// <summary>Mağazanın ilk onaylandığı an (UTC).</summary>
        public DateTime? ApprovedAtUtc { get; set; }

        /// <summary>
        /// Satıcının kendi kararıyla verdiği geçici kapatma ("tatil modu").
        /// Yönetim kararından (Status) BAĞIMSIZDIR; ürünler görünür kalır ama
        /// sipariş alınmaz.
        /// </summary>
        public bool IsTemporarilyClosed { get; set; }

        /// <summary>Geçici kapatmanın biteceği an (UTC).</summary>
        public DateTime? TemporarilyClosedUntilUtc { get; set; }

        /// <summary>Müşteriye gösterilecek geçici kapatma mesajı.</summary>
        public string? TemporaryClosureMessage { get; set; }

        // ── Doğrulama (§57) ───────────────────────────────────────────────────
        /// <summary>
        /// Mağaza doğrulama (kurumsal rozet) durumu.
        /// DİKKAT: Bu, "satıcı belgeleri onaylandı" (StoreDocuments) ve "doğrulanmış
        /// satın alma" (ProductReviews.IsVerifiedPurchase) kavramlarından FARKLIDIR.
        /// </summary>
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.None;

        /// <summary>Rozetin verildiği an (UTC).</summary>
        public DateTime? VerifiedAtUtc { get; set; }

        /// <summary>Zorunlu belgelerin tamamı onaylandı mı? (StoreDocuments'tan türetilir)</summary>
        public bool AreDocumentsApproved { get; set; }

        // ── SAYAÇ ÖNBELLEĞİ (§24 — gerçeğin kaynağı değildir) ─────────────────
        //  FollowersCount → data._Follows.StoreFollowers
        //  ProductCount   → data._Products.Products
        //  PostCount      → data._Shares.Posts (AuthorStoreId)
        //  Rating*        → data._Products.ProductRatingSummary toplamı
        /// <summary>Takipçi sayısı (önbellek).</summary>
        public int FollowersCount { get; set; }

        /// <summary>Yayında olan ürün sayısı (önbellek).</summary>
        public int ProductCount { get; set; }

        /// <summary>Yayımlanmış gönderi sayısı (önbellek).</summary>
        public int PostCount { get; set; }

        /// <summary>Toplam yorum sayısı (önbellek).</summary>
        public int ReviewCount { get; set; }

        /// <summary>Puan veren müşteri sayısı (önbellek).</summary>
        public int RatingCount { get; set; }

        /// <summary>Ortalama mağaza puanı 1.00–5.00 (önbellek).</summary>
        public decimal? AverageRating { get; set; }

        /// <summary>Tamamlanmış sipariş sayısı (önbellek) — satıcı güven göstergesi.</summary>
        public int CompletedOrderCount { get; set; }

        /// <summary>Mağaza sayfası görüntülenme sayısı (önbellek — kaynak: ContentViewDailyAggregates).</summary>
        public int ProfileViewCount { get; set; }

        /// <summary>Ortalama ilk yanıt süresi (dakika) — mesajlaşma performansı göstergesi.</summary>
        public int? AverageResponseTimeMinutes { get; set; }
    }

    /// <summary>
    /// MAĞAZA DOĞRULAMA BELGESİ (§30, §61).
    ///
    /// Eski modeldeki 15 ayrı <c>Guid?</c> belge kolonunun normalize edilmiş hâlidir.
    /// Artık belge başına durum, son kullanma tarihi, red gerekçesi ve inceleme izi
    /// tutulabilir; yeni belge türü eklemek şema değişikliği gerektirmez.
    ///
    /// GÜVENLİK (§61): Belgeler kişisel ve ticari sır içerir. Bağlı medya kaydının
    /// <c>Media.Visibility</c> değeri ZORUNLU olarak <c>Internal</c> olmalıdır;
    /// yalnızca mağaza sahibi ve yetkili personel erişebilir. Belge medyası ASLA
    /// CDN üzerinden public sunulmaz.
    /// </summary>
    public class StoreDocuments : StoreEntityBase
    {
        /// <summary>Belgenin ait olduğu mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Belgenin türü.</summary>
        public StoreDocumentType DocumentType { get; set; }

        /// <summary>Belge dosyası (data._Galleries.Media.Id).</summary>
        public Guid MediaId { get; set; }

        /// <summary>Belge numarası / referansı.</summary>
        public string? DocumentNumber { get; set; }

        /// <summary>Serbest açıklama (DocumentType = Other iken zorunlu).</summary>
        public string? Description { get; set; }

        /// <summary>Belgenin düzenlenme tarihi.</summary>
        public DateOnly? IssuedOn { get; set; }

        /// <summary>Belgenin geçerlilik bitiş tarihi. Yaklaşan sürelerde satıcı uyarılır.</summary>
        public DateOnly? ExpiresOn { get; set; }

        /// <summary>İnceleme durumu.</summary>
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

        /// <summary>Belgeyi inceleyen yönetici (Users.Id).</summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>İncelemenin yapıldığı an (UTC).</summary>
        public DateTime? ReviewedAtUtc { get; set; }

        /// <summary>Red gerekçesi (Status = Rejected iken dolu).</summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Bu belge türünün güncel (yürürlükteki) sürümü mü? Yenileme yüklendiğinde
        /// eski satır false yapılır; böylece belge geçmişi korunur.
        /// </summary>
        public bool IsCurrent { get; set; } = true;
    }

    /// <summary>
    /// MAĞAZA SAYFASINDA ÖNE ÇIKARILAN ÖĞE (§30).
    ///
    /// Mağazanın vitrininde sabitlediği ürün, gönderi, kampanya, kategori veya
    /// makaleleri sıralı olarak tutar. Hedef türüne göre AYRI ve NULLABLE gerçek
    /// FK kolonları kullanılır; polimorfik tek kolon tercih edilmemiştir (§42).
    /// </summary>
    public class StoreFeaturedItems : StoreEntityBase
    {
        /// <summary>Öğenin ait olduğu mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Öne çıkarılan öğenin türü.</summary>
        public StoreFeaturedItemType ItemType { get; set; }

        /// <summary>Öne çıkarılan ürün (Products.Id).</summary>
        public Guid? ProductId { get; set; }

        /// <summary>Öne çıkarılan gönderi (Posts.Id).</summary>
        public Guid? PostId { get; set; }

        /// <summary>Öne çıkarılan kampanya (Campaigns.Id).</summary>
        public Guid? CampaignId { get; set; }

        /// <summary>Öne çıkarılan kategori (CategoriesProduct.Id).</summary>
        public Guid? ProductCategoryId { get; set; }

        /// <summary>Öne çıkarılan makale (Articles.Id).</summary>
        public Guid? ArticleId { get; set; }

        /// <summary>Vitrindeki gösterim sırası (0 tabanlı).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Öğe için özel başlık (boşsa hedefin kendi başlığı kullanılır).</summary>
        public string? CustomTitle { get; set; }

        /// <summary>Öne çıkarmanın kendiliğinden biteceği an (UTC).</summary>
        public DateTime? ExpiresAtUtc { get; set; }
    }
}
