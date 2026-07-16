using data.Owned;
using System.ComponentModel.DataAnnotations;

namespace data._Users
{
    public class UserShortcuts
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Kullanıcı ID'si
        // Şifrele ***-***-***
        public string ShortcutName { get; set; } // Kısayolun adı
        // Şifrele ***-***-***
        public string ShortcutUrl { get; set; } // Kısayolun URL'si
        // Şifrele ***-***-***
        public string ShortcutIcon { get; set; } // Kısayolun URL'si
        public IsDeleted? IsDeleted { get; set; }
    }
}
