using data._Shared;
using System;
using System.Collections.Generic;

namespace data
{
    public class Posts
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsUser { get; set; }
        public Guid UserStoreId { get; set; }

        // İçerik
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}