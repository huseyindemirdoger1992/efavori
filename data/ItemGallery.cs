using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks.Dataflow;

namespace data
{
    public class ItemGallery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid? ItemId { get; set; }
        public bool? InteriorPictures { get; set; } = false;
        public bool? ExteriorImages { get; set; } = false;
        public string? ItemType { get; set; }
        public string? ItemRoad { get; set; }
        public DateTime? ItemAddDate { get; set; }
        public IsDeleted? IsDeleted { get; set; }
    }
}
