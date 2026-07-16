using System.ComponentModel.DataAnnotations;

namespace data._Users
{
    public class LoginTry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UsersId { get; set; } // Kullanıcı ID'si

        public DateTime? AttemptDate { get; set; } = DateTime.UtcNow; // Deneme tarihi

        public bool? IsSuccessful { get; set; } // true = başarılı, false = başarısız

        public string? IPAddress { get; set; } // IP adresi

        public string? UserAgent { get; set; } // Tarayıcı User-Agent bilgisi

        public string? Platform { get; set; } // Platform bilgisi (örneğin: Web, Mobile)

        public string? Browser { get; set; } // Tarayıcı bilgisi

        // <summary>İşlemin yapıldığı URL yolu.</summary>
        public string? RequestPath { get; set; }
    }
}
