using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace data
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid SenderId { get; set; } // Gönderen kişinin kullanıcı ID'si
        [Required]
        public Guid ReceiverId { get; set; } // Alıcı kişinin kullanıcı ID'si
        public string Content { get; set; } // Mesaj içeriği (Dosya gönderimi olmadığı için sadece string)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Mesajın gönderilme zamanı
        public bool IsRead { get; set; } = false; // Mesajın okunup okunmadığı bilgisi (WhatsApp'taki mavi tık mantığı)
        public bool IsDelivered { get; set; } = false; // Mesajın alıcıya ulaşıp ulaşmadığı bilgisi (Çift tık mantığı)
        public bool IsDeletedBySender { get; set; } = false; // Opsiyonel: Mesajın silinip silinmediği(tek taraftan)
        public bool IsDeletedBySenderAndReceiver { get; set; } = false; // Opsiyonel: Mesajın silinip silinmediği(Her iki taraftan)
    }
}
