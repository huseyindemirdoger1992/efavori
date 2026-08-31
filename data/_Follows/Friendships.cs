using System;

namespace data._Follows
{
    /// <summary>
    /// ARKADAŞLIK / ARKADAŞLIK İSTEĞİ (§7) — eski <c>FriendShip</c> entity'sinin yerini alır.
    ///
    /// NEDEN DEĞİŞTİ:
    ///  • <c>bool? Status</c> dört farklı durumu (beklemede / kabul / ret / iptal) temsil
    ///    edemiyordu → <see cref="FriendshipStatus"/> enum'ına dönüştürüldü.
    ///  • <c>bool? Block</c> alanı arkadaşlıkla aynı satırda tutuluyordu. Engelleme
    ///    arkadaşlıktan BAĞIMSIZ bir ilişkidir (arkadaş olmayan biri de engellenebilir);
    ///    ayrı <see cref="UserBlocks"/> tablosuna taşındı.
    ///
    /// ÇİFT YÖNLÜ VERİ PROBLEMİ NASIL ÇÖZÜLDÜ:
    /// Arkadaşlık simetriktir (A-B ile B-A aynı ilişkidir) ama istek yönlüdür
    /// (kim gönderdi, kim yanıtladı bilinmelidir). Bu yüzden yön korunur, tekillik ise
    /// SIRALANMIŞ ÇİFT üzerinden kurulur: veritabanı, iki kullanıcı kimliğini
    /// büyüklüğe göre sıralayıp <see cref="UserPairLowId"/> / <see cref="UserPairHighId"/>
    /// hesaplanmış (persisted computed) kolonlarına yazar. Bu iki kolon üzerindeki
    /// filtreli TEKİL indeks sayesinde A→B ve B→A istekleri aynı anda AKTİF olamaz;
    /// kural uygulama katmanına değil, veritabanına gömülüdür.
    ///
    /// ARKADAŞ LİSTESİ SORGUSU:
    ///     WHERE (UserPairLowId = @me OR UserPairHighId = @me) AND Status = 2
    /// (her iki kolon için de ayrı indeks tanımlıdır).
    /// </summary>
    public class Friendships : FollowEntityBase
    {
        /// <summary>İsteği gönderen kullanıcı (Users.Id).</summary>
        public Guid RequestorUserId { get; set; }

        /// <summary>İsteği alan kullanıcı (Users.Id).</summary>
        public Guid ReceiverUserId { get; set; }

        /// <summary>
        /// HESAPLANMIŞ KOLON (persisted). İki kullanıcı kimliğinden KÜÇÜK olanı.
        /// Uygulama tarafından YAZILMAZ; SQL Server üretir. Tekillik ve simetrik
        /// sorgular bu kolon üzerinden çalışır.
        /// </summary>
        public Guid UserPairLowId { get; private set; }

        /// <summary>
        /// HESAPLANMIŞ KOLON (persisted). İki kullanıcı kimliğinden BÜYÜK olanı.
        /// Uygulama tarafından YAZILMAZ; SQL Server üretir.
        /// </summary>
        public Guid UserPairHighId { get; private set; }

        /// <summary>İlişkinin durumu.</summary>
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

        /// <summary>İstekle birlikte gönderilen kısa not (opsiyonel).</summary>
        public string? RequestMessage { get; set; }

        /// <summary>Alıcının isteği yanıtladığı an (kabul veya ret — UTC).</summary>
        public DateTime? RespondedAtUtc { get; set; }

        /// <summary>Arkadaşlığın kurulduğu an (UTC). Yalnızca Status = Accepted iken dolu.</summary>
        public DateTime? AcceptedAtUtc { get; set; }

        /// <summary>Arkadaşlığın sonlandırıldığı an (UTC). Status = Unfriended iken dolu.</summary>
        public DateTime? UnfriendedAtUtc { get; set; }

        /// <summary>Arkadaşlığı sonlandıran taraf (Users.Id). Status = Unfriended iken dolu.</summary>
        public Guid? UnfriendedByUserId { get; set; }

        /// <summary>
        /// "Yakın arkadaş" işareti — istek gönderenin listesi için.
        /// <c>PostVisibility.Custom</c> hedef kitlesi oluştururken kullanılabilir.
        /// </summary>
        public bool IsCloseFriendForRequestor { get; set; }

        /// <summary>"Yakın arkadaş" işareti — isteği alanın listesi için.</summary>
        public bool IsCloseFriendForReceiver { get; set; }

        /// <summary>
        /// Beklemedeki isteğin geçerlilik bitişi (UTC). Arka plan servisi bu tarihi
        /// geçen Pending kayıtları Expired'a çeker.
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }
    }
}
