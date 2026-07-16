using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Locations
{
    public class States
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [MaxLength(255)]
        [Column("name")]
        public string? name { get; set; }

        [Column("country_id")]
        public int? country_id { get; set; }

        [MaxLength(10)]
        [Column("country_code")]
        public string? country_code { get; set; }

        [MaxLength(255)]
        [Column("country_name")]
        public string? country_name { get; set; }

        [MaxLength(10)]
        [Column("iso2")]
        public string? iso2 { get; set; }

        [MaxLength(20)]
        [Column("iso3166_2")]
        public string? iso3166_2 { get; set; }

        [MaxLength(10)]
        [Column("fips_code")]
        public string? fips_code { get; set; }

        [MaxLength(50)]
        [Column("type")]
        public string? type { get; set; }

        [Column("level")]
        public int? level { get; set; }

        [Column("parent_id")]
        public int? parent_id { get; set; }

        [MaxLength(255)]
        [Column("native")]
        public string? native { get; set; }

        // States sınıfı içinde:
        [Column(TypeName = "decimal(18, 8)")]
        public decimal latitude { get; set; }

        [Column(TypeName = "decimal(18, 8)")]
        public decimal longitude { get; set; }

        [MaxLength(50)]
        [Column("timezone")]
        public string? timezone { get; set; }

        [Column("translations")]
        public string? translations { get; set; }

        [MaxLength(50)]
        [Column("wikiDataId")]
        public string? wikiDataId { get; set; }

        [Column("population")]
        public long? population { get; set; }
    }
}
