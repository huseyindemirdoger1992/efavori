using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    /// <summary>
    /// SOSYAL ETKİLEŞİM SAYAÇ ÖNBELLEĞİ (counter cache).
    ///
    /// DİKKAT — BU DEĞERLER GERÇEĞİN KAYNAĞI DEĞİLDİR.
    /// Asıl kaynak her zaman etkileşim tablolarıdır:
    ///   LikeCount / ReactionCount → data._Shares.PostReactions
    ///   CommentCount              → data._Shares.PostComments
    ///   ShareCount                → data._Shares.PostShares
    ///   RepostCount               → data._Shares.PostReposts
    ///   SaveCount                 → data._Shares.SavedPosts
    ///   ViewCount                 → data._Shares.ContentViewDailyAggregates
    ///
    /// Bu alanlar yalnızca listeleme ekranlarında N+1 COUNT sorgusunu önlemek için
    /// tutulur. Servis/application katmanı kuralı:
    ///  1) Etkileşim yazma işlemi ve sayaç artırımı AYNI transaction içinde yapılır.
    ///  2) Artırım her zaman "UPDATE ... SET X = X + 1" biçiminde (okuma yapmadan)
    ///     uygulanır; böylece eşzamanlı isteklerde kayıp güncelleme oluşmaz.
    ///  3) Bir arka plan servisi (drift reconciliation) periyodik olarak kaynak
    ///     tablolardan yeniden sayıp sapmaları düzeltir.
    ///
    /// Tüm alanlar int ve NOT NULL'dır (varsayılan 0) — null sayaç aritmetiği
    /// (NULL + 1 = NULL) sessiz veri kaybına yol açtığı için bilinçli tercih edilmiştir.
    /// </summary>
    [Owned]
    public class InteractionCounts
    {
        /// <summary>Toplam görüntülenme (tekilleştirilmemiş).</summary>
        public int ViewCount { get; set; }

        /// <summary>Yalnızca <c>ReactionType.Like</c> türündeki tepki sayısı.</summary>
        public int LikeCount { get; set; }

        /// <summary>Tüm tepki türlerinin toplamı (Like dâhil).</summary>
        public int ReactionCount { get; set; }

        /// <summary>Silinmemiş ve görünür durumdaki yorum sayısı (yanıtlar dâhil).</summary>
        public int CommentCount { get; set; }

        /// <summary>Paylaşım (share) sayısı.</summary>
        public int ShareCount { get; set; }

        /// <summary>Yeniden paylaşım (repost) sayısı.</summary>
        public int RepostCount { get; set; }

        /// <summary>Kaydetme/yer imi (bookmark) sayısı.</summary>
        public int SaveCount { get; set; }

        /// <summary>Tavsiye etme sayısı (mevcut alan — geriye dönük uyumluluk için korunmuştur).</summary>
        public int RecommendCount { get; set; }
    }
}
