using data._Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    /// <summary>
    /// Medya dosyalarının meta verilerini ve depolama bilgilerini tutan sınıf.
    /// </summary>
    public class Media
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        // Dosyayı yükleyen kullanıcının ID'si
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        // Dosyanın orijinal adı (Örn: tatil_fotografi.jpg)
        public string FileName { get; set; }

        [Required]
        // Dosyanın diskteki veya buluttaki benzersiz adı (Çakışmaları önlemek için)
        public string FileStoredName { get; set; }

        [Required]
        // Dosyanın erişim adresi (CDN veya Local URL)
        public string FileUrl { get; set; }

        // Dosyanın fiziksel veya göreceli yolu
        public string FilePhysicalPathRoad { get; set; }

        [MaxLength(10)]
        // Dosya uzantısı (Örn: .png, .mp4)
        public string FileExtensionType { get; set; }

        [MaxLength(50)]
        // MIME Tipi (Örn: image/jpeg, video/mp4) - Tarayıcılar için önemlidir.
        public string ContentType { get; set; }

        // Dosya boyutu (Byte cinsinden tutmak hesaplamalar için daha iyidir)
        public long OriginalSize { get; set; }

        // Eğer sıkıştırma yapıldıysa işlem sonrası boyut
        public long? CompressedSize { get; set; }

        // Yükleme tarihi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Silinme durumu
        public IsDeleted IsDeleted { get; set; }
    }
}