using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; } // Primary key

        [Required]
        public string name { get; set; } = null!; // NOT NULL

        public string? iso3 { get; set; } // AFG, ALA vb.
        public string? iso2 { get; set; } // AF, AX vb.
        public string? numeric_code { get; set; }
        public string? phonecode { get; set; }
        public string? capital { get; set; }
        public string? currency { get; set; }
        public string? currency_name { get; set; }
        public string? currency_symbol { get; set; }
        public string? tld { get; set; }
        public string? native { get; set; }

        public long? population { get; set; } // Örn: 43844000
        public long? gdp { get; set; } // Örn: 1771681

        public string? region { get; set; }
        public int? region_id { get; set; }
        public string? subregion { get; set; }
        public int? subregion_id { get; set; }
        public string? nationality { get; set; }
        public long? area_sq_km { get; set; }

        public string? postal_code_format { get; set; }
        public string? postal_code_regex { get; set; }

        public string? timezones { get; set; } // JSON string
        public string? translations { get; set; } // JSON string

        [Column(TypeName = "decimal(18, 8)")]
        public decimal? latitude { get; set; } // SQL'de 8 basamak hassasiyet

        [Column(TypeName = "decimal(18, 8)")]
        public decimal? longitude { get; set; } // SQL'de 8 basamak hassasiyet

        public string? emoji { get; set; }
        public string? emojiU { get; set; }
        public string? wikiDataId { get; set; }

        public DateTime? updated_at { get; set; } = DateTime.Now;

        public bool? flag { get; set; } = true; // SQL tarafında 0/1 ile eşleşir
    }
}
