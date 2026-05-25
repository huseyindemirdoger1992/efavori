using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class TryTableSingle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }

        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Cities { get; set; }

        public bool? CheckSliding { get; set; }
        public bool? CheckMarked { get; set; }
        public string? RadioButton { get; set; }

        public DateTime SelectDateTime { get; set; }
        public DateTime GetDateTime { get; set; }

        public ProfileCoverGallery? ProfileCoverGallery { get; set; }
        public IsDeleted? IsDeleted { get; set; }

    }
}
