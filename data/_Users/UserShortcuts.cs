using System;

namespace data._Users
{
    /// <summary>
    /// Kullanıcının panelinde tanımladığı hızlı erişim kısayolları.
    /// Ortak <see cref="UserEntityBase"/> desenine taşınmıştır (audit + rowversion + soft delete).
    /// </summary>
    public class UserShortcuts : UserEntityBase
    {
        /// <summary>Kısayolun sahibi (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Kısayolun görünen adı.</summary>
        public string ShortcutName { get; set; } = string.Empty;

        /// <summary>Kısayolun hedef adresi (uygulama içi yol veya tam URL).</summary>
        public string ShortcutUrl { get; set; } = string.Empty;

        /// <summary>Kısayol ikonu (ikon kütüphanesi anahtarı, ör. "bi-cart").</summary>
        public string? ShortcutIcon { get; set; }

        /// <summary>Panel içindeki gösterim sırası.</summary>
        public int DisplayOrder { get; set; }
    }
}
