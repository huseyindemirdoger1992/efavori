using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class Categories
    {
        // -------------------- CATEGORY NAMES --------------------

        public string NameTr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameAz { get; set; } = string.Empty;
        public string NameDe { get; set; } = string.Empty;
        public string NameEs { get; set; } = string.Empty;
        public string NameFr { get; set; } = string.Empty;
        public string NameHi { get; set; } = string.Empty;
        public string NamePt { get; set; } = string.Empty;
        public string NameRu { get; set; } = string.Empty;
        public string NameZh { get; set; } = string.Empty;

        // -------------------- SEO URL --------------------

        public string? SlugTr { get; set; }
        public string? SlugEn { get; set; }
        public string? SlugAz { get; set; }
        public string? SlugDe { get; set; }
        public string? SlugEs { get; set; }
        public string? SlugFr { get; set; }
        public string? SlugHi { get; set; }
        public string? SlugPt { get; set; }
        public string? SlugRu { get; set; }
        public string? SlugZh { get; set; }
    }
}
