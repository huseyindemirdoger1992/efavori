using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    public class ItemGallery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemType { get; set; }
        public string? ItemRoad { get; set; }
        public DateTime? ItemAddDate { get; set; }
        public bool? IsDelete { get; set; }


    }
}
