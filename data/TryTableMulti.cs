using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class TryTableMulti
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemType { get; set; } // InShooting, OutShooting, Document
        public Guid? MediaId { get; set; }
        public DateTime? ItemAddDate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
