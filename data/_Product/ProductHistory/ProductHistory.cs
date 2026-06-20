using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Product.ProductHistory
{
    public class ProductHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductSlug { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public IsDeleted? IsDeleted { get; set; } = new(); 
    }
}
