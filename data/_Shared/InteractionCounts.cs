using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data._Shared
{
    [Owned]

    public class InteractionCounts
    {
        public int? ViewCount { get; set; }
        public int? LikeCount { get; set; }
        public int? ShareCount { get; set; }
        public int? BookmarkCount { get; set; } 
    }
}
