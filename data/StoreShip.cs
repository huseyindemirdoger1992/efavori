using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class StoreShip
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // İsteği Başlatan Kişi (Sender / Requestor)
        [Required]
        public Guid RequestorId { get; set; }

        // İsteğe Maruz Kalan / Alıcı Kişi (Receiver)
        [Required]
        public Guid ReceiverId { get; set; }

        // İsteğin Durumu
        public bool? Status { get; set; } = null;
        public bool? Block { get; set; } = null;

        // Zaman Damgaları
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ActionDate { get; set; } // Kabul veya ret tarihi

        public IsDeleted? IsDeleted { get; set; }

    }
}
