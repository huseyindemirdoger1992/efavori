using System;

namespace data._Shares
{
    /// <summary>
    /// ÖZEL HEDEF KİTLE — KULLANICI LİSTESİ (§12).
    ///
    /// <c>Posts.Visibility = Custom</c> iken gönderiyi görebilecek kullanıcıları,
    /// <c>Visibility = FriendsExcept</c> iken ise görmemesi gereken kullanıcıları tutar.
    /// Hangi anlamda kullanıldığını <see cref="IsExcluded"/> belirler.
    ///
    /// Bir gönderi için hem dâhil hem hariç satırları bulunabilir; servis katmanı
    /// önce dâhil kümesini hesaplar, sonra hariç kümesini çıkarır.
    /// </summary>
    public class PostAudienceUsers : SocialEntityBase
    {
        /// <summary>Bağlı gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Hedef kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// false = bu kullanıcı gönderiyi GÖREBİLİR (Custom listesi).
        /// true  = bu kullanıcı gönderiyi GÖREMEZ (FriendsExcept listesi).
        /// </summary>
        public bool IsExcluded { get; set; }
    }

    /// <summary>
    /// ÖZEL HEDEF KİTLE — KURAL TABANLI (§12, gelecek genişlemesi).
    ///
    /// Ülke, yaş aralığı, dil, mağaza takipçiliği veya ürün satın alma geçmişi gibi
    /// ileri hedeflemeleri, <c>PostVisibility</c> enum'ına yeni değer eklemeden
    /// modellemeyi sağlar. Bir gönderinin birden çok kuralı olabilir; kurallar
    /// VE (AND) mantığıyla değerlendirilir.
    ///
    /// Bu tablo BOŞ da kalabilir — mevcut sürümde zorunlu değildir; şema hazır
    /// olduğu için ileride özellik eklerken migration gerektirmez.
    /// </summary>
    public class PostAudienceRules : SocialEntityBase
    {
        /// <summary>Bağlı gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Kuralın türü.</summary>
        public PostAudienceRuleType RuleType { get; set; }

        /// <summary>
        /// Kuralın sayısal değeri (yaş, Country.id, Language byte değeri).
        /// Guid hedefli kurallarda <see cref="TargetId"/> kullanılır.
        /// </summary>
        public int? NumericValue { get; set; }

        /// <summary>Kuralın Guid hedefi (Store.Id, Products.Id).</summary>
        public Guid? TargetId { get; set; }

        /// <summary>true = kural dışlayıcıdır (eşleşenler göremez).</summary>
        public bool IsExcluded { get; set; }
    }
}
