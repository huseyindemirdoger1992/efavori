using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    public class Regions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; } // Primary Key, IDENTITY(1,1)

        [StringLength(100)]
        public string? name { get; set; } = null!; // NOT NULL, NVARCHAR(100)

        public string? translations { get; set; } // NVARCHAR(MAX), JSON formatında veri içerir

        public string? wikiDataId { get; set; } // NVARCHAR(255)

        public DateTime? created_at { get; set; } // DATETIME2 NULL

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Veritabanının bu değeri üretmesini sağlar
        public DateTime? updated_at { get; set; } = DateTime.Now; // DATETIME2 NOT NULL, DEFAULT GETDATE()

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Veritabanındaki DEFAULT değerini kullanması için
        public bool? flag { get; set; } = true; // BIT NOT NULL, DEFAULT 1
    }
}
