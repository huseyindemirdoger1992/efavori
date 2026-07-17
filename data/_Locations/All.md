## Dosya: Cities.cs
```csharp
﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Locations
{
    public class Cities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }  // Primary key, zorunlu

        public string? name { get; set; }
        public int? state_id { get; set; }
        public string? state_code { get; set; }
        public string? state_name { get; set; }
        public int? country_id { get; set; }
        public string? country_code { get; set; }
        public string? country_name { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? native { get; set; }
        public string? type { get; set; }
        public int? level { get; set; }
        public int? parent_id { get; set; }
        public long? population { get; set; }
        public string? timezone { get; set; }
        public string? translations { get; set; }
        public string? wikiDataId { get; set; }
    }
}
```

## Dosya: Country.cs
```csharp
﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Locations
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
```

## Dosya: Regions.cs
```csharp
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Locations
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
```

## Dosya: States.cs
```csharp
﻿using System;
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
```

