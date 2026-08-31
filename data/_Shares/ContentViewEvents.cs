using System;

namespace data._Shares
{
    /// <summary>
    /// GÖRÜNTÜLENME OLAYI (§63) — çok yüksek hacimli, SALT-EKLEME (append-only) tablo.
    ///
    /// NEDEN AYRI TABLO:
    /// Milyonlarca görüntülenme kaydını <c>Posts</c> tablosunda tutmak (veya her
    /// görüntülenmede Posts satırını UPDATE etmek) sıcak tabloda kilit çekişmesine,
    /// sayfa bölünmesine ve tüm akış sorgularının yavaşlamasına yol açar. Bu yüzden
    /// ham olaylar buraya yazılır, gün sonunda <see cref="ContentViewDailyAggregates"/>
    /// tablosuna toplanır ve yalnızca özet, ilgili varlığın sayaç önbelleğine yansıtılır.
    ///
    /// TASARIM KARARLARI:
    ///  • Soft delete YOK, RowVersion YOK, denetim alanları YOK — satır başına maliyeti
    ///    en aza indirmek için. Olay kaydı hiç güncellenmez.
    ///  • Yazma yolunda İNDEKS EN AZDIR; raporlama toplu tablodan yapılır.
    ///  • Saklama süresi (öneri: 90 gün) dolan satırlar arka plan servisi tarafından
    ///    parti parti silinir. Tablo, <c>CreatedAtUtc</c> üzerinden tarih bazlı
    ///    BÖLÜMLEME (partitioning) veya aylık tablo bölme için uygundur.
    ///  • Tekilleştirme (aynı kullanıcı aynı gönderiyi 10 kez gördü) burada YAPILMAZ;
    ///    toplama işinde <see cref="ViewerUserId"/> ve <see cref="SessionId"/> üzerinden
    ///    hesaplanır.
    /// </summary>
    public class ContentViewEvents
    {
        /// <summary>
        /// Birincil anahtar. Sıralı GUID (sequential/COMB) üretilmesi ÖNEMLİDİR:
        /// rastgele GUID, kümelenmiş indekste ağır sayfa bölünmesine yol açar.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Görüntülenen içeriğin türü.</summary>
        public ContentTargetType TargetType { get; set; }

        /// <summary>
        /// Görüntülenen içeriğin kimliği.
        /// BU KOLON İÇİN FK TANIMLANMAZ — bilinçli tercihtir: saniyede binlerce satır
        /// yazan bir olay tablosunda FK doğrulaması yazma maliyetini gereksiz artırır
        /// ve saklama süresi dolduğunda içerik silinse bile olay kaydı geçerliliğini
        /// korumalıdır (analitik veri, ilişkisel veri değildir).
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>Görüntüleyen kullanıcı (Users.Id). Anonim ziyarette null.</summary>
        public Guid? ViewerUserId { get; set; }

        /// <summary>Anonim ziyaretçi oturum kimliği — tekil ziyaretçi sayımı için.</summary>
        public string? SessionId { get; set; }

        /// <summary>Görüntülemenin geldiği yüzey (akış, profil, arama...).</summary>
        public ViewSourceSurface SourceSurface { get; set; } = ViewSourceSurface.HomeFeed;

        /// <summary>
        /// İçeriğin ekranda kalma süresi (milisaniye). "Gördü" ile "okudu" ayrımını
        /// yapmayı ve akış sıralama kalitesini ölçmeyi sağlar.
        /// </summary>
        public int? DwellTimeMs { get; set; }

        /// <summary>Video/ses içeriğin izlenme oranı (0–100).</summary>
        public byte? WatchPercentage { get; set; }

        /// <summary>Cihaz türü ("Web", "Android", "iOS").</summary>
        public string? Platform { get; set; }

        /// <summary>Ziyaretçinin ülke kodu (ISO 3166-1 alpha-2).</summary>
        public string? CountryCode { get; set; }

        /// <summary>Olayın gerçekleştiği an (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Olayın tarih kovası (yalnızca tarih). Toplama işi ve bölümleme anahtarı
        /// olarak kullanılır; <see cref="CreatedAtUtc"/> üzerinden hesaplanır.
        /// </summary>
        public DateOnly EventDate { get; set; }
    }

    /// <summary>
    /// GÜNLÜK GÖRÜNTÜLENME ÖZETİ (§63).
    ///
    /// <see cref="ContentViewEvents"/> tablosundan bir arka plan servisi tarafından
    /// günlük olarak üretilir. Raporlama, grafik ve satıcı paneli sorguları HAM olay
    /// tablosuna değil bu tabloya gider. Ham olaylar silinse bile geçmiş istatistik
    /// burada kalıcıdır.
    ///
    /// TEKİLLİK: (TargetType, TargetId, EventDate).
    /// </summary>
    public class ContentViewDailyAggregates
    {
        /// <summary>Birincil anahtar.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Özetlenen içeriğin türü.</summary>
        public ContentTargetType TargetType { get; set; }

        /// <summary>Özetlenen içeriğin kimliği.</summary>
        public Guid TargetId { get; set; }

        /// <summary>Özetin ait olduğu gün (UTC).</summary>
        public DateOnly EventDate { get; set; }

        /// <summary>Toplam görüntülenme (tekilleştirilmemiş).</summary>
        public int ViewCount { get; set; }

        /// <summary>Tekil kullanıcı sayısı (giriş yapmış).</summary>
        public int UniqueUserCount { get; set; }

        /// <summary>Tekil anonim oturum sayısı.</summary>
        public int UniqueSessionCount { get; set; }

        /// <summary>Ortalama ekranda kalma süresi (milisaniye).</summary>
        public int? AverageDwellTimeMs { get; set; }

        /// <summary>Ortalama izlenme oranı (0–100) — video içerikler için.</summary>
        public byte? AverageWatchPercentage { get; set; }

        /// <summary>Özetin üretildiği an (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Özetin en son yeniden hesaplandığı an (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
