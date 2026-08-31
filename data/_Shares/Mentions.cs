using System;

namespace data._Shares
{
    /// <summary>
    /// GÖNDERİDE KULLANICI ANMA / MENTION (§20).
    ///
    /// Metin içindeki "@kullanici" ifadesinin konumu <see cref="StartIndex"/> ve
    /// <see cref="Length"/> ile saklanır. Böylece istemci, metni yeniden ayrıştırmadan
    /// bağlantıları doğru konumlara yerleştirebilir; kullanıcı adı sonradan
    /// değiştirilse bile bağlantı doğru kişiye gider (FK kullanıcı kimliğinedir,
    /// metne değil).
    ///
    /// GİZLİLİK: Anma kaydı oluşturulmadan önce hedefin
    /// <c>UserPrivacySettings.WhoCanMention</c> kuralı ve <c>UserBlocks</c> denetlenir.
    ///
    /// TEKİLLİK: (PostId, MentionedUserId, StartIndex) — aynı kişi bir metinde birden
    /// çok kez anılabilir, ama aynı konumda iki kez anılamaz.
    /// </summary>
    public class PostMentions : SocialEntityBase
    {
        /// <summary>Anmanın geçtiği gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Anılan kullanıcı (Users.Id).</summary>
        public Guid MentionedUserId { get; set; }

        /// <summary>Anmayı yapan kullanıcı (Users.Id) — bildirimde "aktör" olarak gösterilir.</summary>
        public Guid MentionedByUserId { get; set; }

        /// <summary>Metin içindeki başlangıç konumu (0 tabanlı karakter indeksi).</summary>
        public int StartIndex { get; set; }

        /// <summary>Anma ifadesinin karakter uzunluğu ("@" dâhil).</summary>
        public int Length { get; set; }

        /// <summary>
        /// Anma anındaki kullanıcı adının kopyası. Kullanıcı adını sonradan değiştirirse
        /// eski metin ile yeni bağlantı arasındaki farkı denetlemeye yarar.
        /// </summary>
        public string? MentionedUsernameSnapshot { get; set; }

        /// <summary>
        /// Anılan kişi bu anmayı onayladı mı? <c>UserPrivacySettings.RequireTagApproval</c>
        /// açıksa, onaylanana kadar anma profilinde görünmez.
        /// </summary>
        public bool IsApprovedByMentionedUser { get; set; } = true;

        /// <summary>Anılan kişi bu anmayı kaldırdı mı? (bağlantı görünmez olur)</summary>
        public bool IsRemovedByMentionedUser { get; set; }
    }

    /// <summary>
    /// YORUMDA KULLANICI ANMA / MENTION (§20).
    /// Kurallar <see cref="PostMentions"/> ile aynıdır; yalnızca hedef yorumdur.
    /// TEKİLLİK: (CommentId, MentionedUserId, StartIndex).
    /// </summary>
    public class CommentMentions : SocialEntityBase
    {
        /// <summary>Anmanın geçtiği yorum (PostComments.Id).</summary>
        public Guid CommentId { get; set; }

        /// <summary>Anılan kullanıcı (Users.Id).</summary>
        public Guid MentionedUserId { get; set; }

        /// <summary>Anmayı yapan kullanıcı (Users.Id).</summary>
        public Guid MentionedByUserId { get; set; }

        /// <summary>Metin içindeki başlangıç konumu.</summary>
        public int StartIndex { get; set; }

        /// <summary>Anma ifadesinin karakter uzunluğu.</summary>
        public int Length { get; set; }

        /// <summary>Anma anındaki kullanıcı adının kopyası.</summary>
        public string? MentionedUsernameSnapshot { get; set; }
    }

    /// <summary>
    /// GÖNDERİDE MAĞAZA/ÜRÜN ETİKETLEME.
    ///
    /// Instagram Shopping benzeri "ürünü etiketle" özelliğinin veri modelidir:
    /// bir sosyal gönderi doğrudan satın alınabilir ürünlere bağlanır ve bu bağ,
    /// pazaryeri ile sosyal ağın birleştiği noktadır.
    ///
    /// TEKİLLİK: (PostId, ProductId).
    /// </summary>
    public class PostProductTags : SocialEntityBase
    {
        /// <summary>Etiketin bulunduğu gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Etiketlenen ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Etiketlenen varyant (ProductVariants.Id) — opsiyonel.</summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>Etiketin bağlı olduğu medya (PostMedia.Id). Null = gönderi geneli.</summary>
        public Guid? PostMediaId { get; set; }

        /// <summary>Görsel üzerindeki yatay konum (0.0000–1.0000).</summary>
        public decimal? PositionX { get; set; }

        /// <summary>Görsel üzerindeki dikey konum (0.0000–1.0000).</summary>
        public decimal? PositionY { get; set; }

        /// <summary>Etiketten ürüne yapılan tıklama sayısı (önbellek — dönüşüm ölçümü).</summary>
        public int ClickCount { get; set; }
    }
}
