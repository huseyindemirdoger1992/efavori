using System;
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
