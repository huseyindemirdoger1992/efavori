using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Systems
{
    public class Logs
    {
        // <summary>Log kaydının benzersiz tanımlayıcısı.</summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // --- KİMLİK BİLGİLERİ ---

        // <summary>İşlemi gerçekleştiren kullanıcının benzersiz kimliği.</summary>
        public Guid? UserId { get; set; }

        // <summary>Logun ait olduğu servis veya modül adı (örn: "MediaService").</summary>
        public string? PageNameSpaceTitle { get; set; }

        // <summary>Gerçekleştirilen işlemin adı (örn: "ConvertAvif").</summary>
        public string? Action { get; set; }

        // --- HTTP VE BAĞLANTI DETAYLARI (UserInfos'tan Gelenler) ---

        // <summary>İsteği yapan kullanıcının IP adresi.</summary>
        public string? IpAddress { get; set; }

        // <summary>Kullanıcının tarayıcı ve işletim sistemi bilgisi.</summary>
        public string? UserAgent { get; set; }

        // <summary>İşlemin yapıldığı URL yolu.</summary>
        public string? RequestPath { get; set; }

        // <summary>Tarayıcının kabul ettiği dil tercihleri.</summary>
        public string? Languages { get; set; }

        // <summary>İşlem sırasında oluşan hata mesajı.</summary>
        public string? Exception { get; set; }

        // <summary>Hatanın oluştuğu kod yığını (stack trace) bilgisi.</summary>
        public string? StackTrace { get; set; }

        // <summary>Log kaydının oluşturulma tarihi ve saati (UTC).</summary>
        [Required]
        public DateTime? Date { get; set; } = DateTime.UtcNow;
    }
}
