using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    public class Logs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // --- KİMLİK BİLGİLERİ ---
        public Guid? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } // Örn: "MediaService"

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } // Örn: "ConvertAvif"

        // --- HTTP VE BAĞLANTI DETAYLARI (UserInfos'tan Gelenler) ---
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? RequestPath { get; set; }  // Yeni: İşlemin yapıldığı URL
        public string? RequestMethod { get; set; } // Yeni: GET, POST, PUT vb.
        public string? Referrer { get; set; }      // Yeni: Kullanıcı nereden yönlendi?
        public string? Languages { get; set; }     // Yeni: Tarayıcı dil tercihi

        [MaxLength(100)]
        public string? TraceId { get; set; }       // Yeni: İşlemin benzersiz takip numarası

        // --- DURUM VE HATA BİLGİLERİ ---
        public string? Exception { get; set; }
        public string? StackTrace { get; set; }
        public long DurationMs { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}