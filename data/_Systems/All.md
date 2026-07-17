## Dosya: AccountPermissions.cs
```csharp
﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Systems
{
    public class AccountPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = 1; // Tek satır olacağı için ID'yi 1'e sabitleyip kapatıyoruz.

        public bool CanRegister { get; set; } = true;

        public bool CanResetPassword { get; set; } = true;

        public bool CanLogin { get; set; } = true;
        public bool WebActionInfos { get; set; } = true;
    }
}```

## Dosya: AllBackgroundServicesFrequencyRate.cs
```csharp
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data._Systems
{
    public class AllBackgroundServicesFrequencyRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = 1; // Tek satır olacağı için ID'yi 1'e sabitleyip kapatıyoruz.

        // Döviz kuru çekme durumu izni
        public bool IsCurrencyFetchEnabled { get; set; }

        // Döviz kuru çekme sıklığı (saniye cinsinden)
        public int CurrencyFetchIntervalInSeconds { get; set; }

        // AI ürün içerik üretimi durumu izni
        public bool IsAiContentGenerationEnabled { get; set; }

        // AI ürün içerik üretimi sıklığı (saniye cinsinden)
        public int AiContentGenerationIntervalInSeconds { get; set; }

        // AI ürün içerik üretimi için maksimum deneme sayısı
        public int AiContentGenerationIntervalMaxAiRetry { get; set; }
    }
}
```

## Dosya: Logs.cs
```csharp
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Systems
{
    public class Logs
    {
        // <summary>Log kaydının benzersiz tanımlayıcısı.</summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // --- KİMLİK BİLGİLERİ ---

        // <summary>İşlemi gerçekleştiren kullanıcının benzersiz kimliği.</summary>
        public Guid? UserId { get; set; }

        // <summary>Logun ait olduğu servis veya modül adı (örn: "MediaService").</summary>
        public string? PageNameSpaceTitle { get; set; }

        // <summary>Gerçekleştirilen işlemin adı (örn: "ConvertAvif").</summary>
        public string? Action { get; set; }

        // --- HTTP VE BAĞLANTI DETAYLARI (UserInfos'tan Gelenler) ---

        // <summary>İsteği yapan kullanıcının IP adresi.</summary>
        public string? IpAddress { get; set; }

        // <summary>Kullanıcının tarayıcı ve işletim sistemi bilgisi.</summary>
        public string? UserAgent { get; set; }

        // <summary>İşlemin yapıldığı URL yolu.</summary>
        public string? RequestPath { get; set; }

        // <summary>Tarayıcının kabul ettiği dil tercihleri.</summary>
        public string? Languages { get; set; }

        // <summary>İşlem sırasında oluşan hata mesajı.</summary>
        public string? Exception { get; set; }

        // <summary>Hatanın oluştuğu kod yığını (stack trace) bilgisi.</summary>
        public string? StackTrace { get; set; }

        // <summary>Log kaydının oluşturulma tarihi ve saati (UTC).</summary>
        [Required]
        public DateTime? Date { get; set; } = DateTime.UtcNow;
    }
}```

## Dosya: MainCssJs.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Systems
{
    public class MainCssJs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Benzersiz kimlik oluşturmak için Guid kullanılır
        public string? Title { get; set; } // Başlık alanı
        public string? Description { get; set; } // Açıklama alanı
        public string? UserCodes { get; set; } // Kullanıcı kodları alanı
        public bool? IsActive { get; set; } // Aktiflik durumu alanı
        public DateTime? GetDateTime { get; set; } // Oluşturulma tarihi alanı
        public bool? IsCssOrJs { get; set; } // CSS veya JS olduğunu belirten alan (true: CSS, false: JS) select option yapısında kullanılabilir Css ise true, Js ise false
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
```

## Dosya: TryTableSingle.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Systems
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
```

