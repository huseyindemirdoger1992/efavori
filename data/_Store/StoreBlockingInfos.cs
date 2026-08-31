using System;

namespace data._Store
{
    /// <summary>
    /// MAĞAZA ENGELLEME / ONAYLANMAMA KAYDI — yönetim kararlarının denetim izi.
    ///
    /// Mağazanın anlık durumu <c>Store.Status</c> alanındadır; bu tablo o duruma
    /// GİDEN KARARLARIN geçmişini tutar: ne zaman, hangi yönetici, hangi gerekçeyle
    /// askıya aldı ve sorun ne zaman çözüldü.
    ///
    /// Ortak <see cref="StoreEntityBase"/> desenine taşınmıştır (audit + rowversion).
    /// </summary>
    public class StoreBlockingInfos : StoreEntityBase
    {
        /// <summary>Karara konu olan mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Mağazanın sahibi olan satıcı (Users.Id) — denormalize iz.</summary>
        public Guid? OwnerUserId { get; set; }

        /// <summary>Kararı veren yönetici (Users.Id).</summary>
        public Guid? AdminUserId { get; set; }

        /// <summary>Engelleme/onaylamama gerekçesinin açıklaması.</summary>
        public string BlockInfoDescription { get; set; } = string.Empty;

        /// <summary>Satıcının uygulaması gereken düzeltme adımı.</summary>
        public string? RequiredAction { get; set; }

        /// <summary>Sorun çözüldü mü?</summary>
        public bool IsResolved { get; set; }

        /// <summary>Sorunun çözüldüğü an (UTC).</summary>
        public DateTime? ResolvedAtUtc { get; set; }

        /// <summary>Çözümü onaylayan yönetici (Users.Id).</summary>
        public Guid? ResolvedByUserId { get; set; }

        /// <summary>Kararın uygulandığı an (UTC).</summary>
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Askıya almanın kendiliğinden biteceği an (UTC). Null = süresiz.</summary>
        public DateTime? EffectiveUntilUtc { get; set; }
    }
}
