using data._Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace data
{
    public class Article
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsUser { get; set; }
        public Guid? UserStoreId { get; set; }

        public Guid FeaturedImage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
