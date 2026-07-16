using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Galleries
{
    public class MediaItems
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemType { get; set; } // InShooting, OutShooting, Document
        public Guid? MediaId { get; set; }
        public DateTime? ItemAddDate { get; set; }
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
