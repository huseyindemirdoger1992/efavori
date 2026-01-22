using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class Addresses
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ItemId { get; set; }
        public AddressInfo? AddressInfo { get; set; }
        public IsDeleted? IsDeleted { get; set; }
    }
}
