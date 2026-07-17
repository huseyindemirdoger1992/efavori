## Dosya: Brands.cs
```csharp
using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Products
{
    /// <summary>
    /// Marka tanımları. UserId/StoreId null ise sistem geneli (admin) markasıdır,
    /// dolu ise yalnızca ilgili satıcıya/mağazaya özeldir.
    /// </summary>
    public class Brands
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem markası, dolu = satıcıya özel marka

        public string? Name { get; set; } // Marka adı (Örn: Samsung)
        public string? Slug { get; set; } // SEO dostu marka kodu (Örn: samsung)

        public bool IsApprovedByAdmin { get; set; } = false; // Satıcı markası admin onayından geçti mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
```

## Dosya: MoneyExchangeRate.cs
```csharp
﻿using System;
using System.ComponentModel.DataAnnotations;

namespace data._Products
{
    public class MoneyExchangeRate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Dolar_Usd { get; set; }
        public decimal Euro_Eur { get; set; }
        public decimal Manat_Azn { get; set; }
        public decimal Lira_Tl { get; set; }

        // Verinin çekildiği an
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}```

