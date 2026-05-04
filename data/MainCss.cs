using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class MainCss
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UserCodes { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? GetDateTime { get; set; }
        public bool? IsDelete { get; set; }
    }
}
