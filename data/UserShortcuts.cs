using System.ComponentModel.DataAnnotations;

namespace data
{
    public class UserShortcuts
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Kullanıcı ID'si
        public string ShortcutName { get; set; } // Kısayolun adı
        public string ShortcutUrl { get; set; } // Kısayolun URL'si
        public string ShortcutIcon { get; set; } // Kısayolun URL'si
        public bool IsDeleted { get; set; } = false; // Varsayılan olarak false
    }
}
