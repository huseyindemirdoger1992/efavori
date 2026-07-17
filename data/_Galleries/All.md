## Dosya: Media.cs
```csharp
﻿using data.Owned;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Galleries
{
    /// <summary>
    /// Medya dosyalarının meta verilerini ve depolama bilgilerini tutan sınıf.
    /// </summary>
    public class Media
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public string? FileName { get; set; }

        public string? FileStoredName { get; set; }

        // Dosyanın boyutu düşürülmemiş halidir.
        public string? FileUrl { get; set; }

        // 1/2 oranında küçültülmüş 
        public string? FileUrl_Ratio_1_2 { get; set; }

        // 1/4 oranında küçültülmüş 
        public string? FileUrl_Ratio_1_4 { get; set; }

        // 1/8 oranında küçültülmüş 
        public string? FileUrl_Ratio_1_8 { get; set; }

        // 1/16 oranında küçültülmüş 
        public string? FileUrl_Ratio_1_16 { get; set; }

        public string? FilePhysicalPathRoad { get; set; }

        public string? OrjFileUrl { get; set; }

        public string? OrjFilePhysicalPathRoad { get; set; }

        public string? FileExtensionType { get; set; }

        public string? ContentType { get; set; }

        public long? OriginalSize { get; set; }

        public long? CompressedSize { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public bool? AiAvif { get; set; }    

        public IsDeleted? IsDeleted { get; set; } = new();

    }
}```

## Dosya: MediaItems.cs
```csharp
﻿using data.Owned;
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
```

