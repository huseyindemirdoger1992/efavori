## Dosya: CartsFavorite.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Carts
{
    public class CartsFavorite
    {
        [Key]
        // Kaydının benzersiz kimliği.
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string? ProductSlug { get; set; }

        public CartProductSnapshot? ProductSnapshot { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
```

## Dosya: CartsProduct.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Carts
{
    public class CartsProduct
    {
        [Key]
        // Kaydının benzersiz kimliği.
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string? ProductSlug { get; set; }

        public CartProductSnapshot? ProductSnapshot { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
```

