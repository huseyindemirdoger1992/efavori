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

        public Guid UserId { get; set; }

        public string? FileName { get; set; }

        public string? FileStoredName { get; set; }

        public string? FileUrl { get; set; }

        public string? FilePhysicalPathRoad { get; set; }

        public string? OrjFileUrl { get; set; }

        public string? OrjFilePhysicalPathRoad { get; set; }

        public string? FileExtensionType { get; set; }

        public string? ContentType { get; set; }

        public long? OriginalSize { get; set; }

        public long? CompressedSize { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDeletedStatu { get; set; } = false;

        public bool? AiAvif { get; set; }    

        public DateTime? DeletedAtDate { get; set; }
    }
}