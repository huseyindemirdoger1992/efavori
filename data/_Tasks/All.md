## Dosya: TaskCategories.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Tasks
{
    public class TaskCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? WorkstationEmployeeGroupId { get; set; } // Bağlı olduğu İş istasyonu + 
        // public string? CategoryStructure { get; set; } // PlanningTaskBoard - InProcessTaskBoard - CompletedTaskBoard
        public string? Title { get; set; }
        public string? Description { get; set; }

        public Guid? TaskFrameworkId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public IsDeleted? IsDeleted { get; set; }
    }
}
```

## Dosya: TaskFramework.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Tasks
{
    public class TaskFramework
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.NewGuid();

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Statu { get; set; } // Plan,Aktif,Tamam
        public DateTime CreatedAt { get; set; }
        public IsDeleted? IsDeleted { get; set; }
    }
}
```

## Dosya: TaskKeeperJoint.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Tasks
{
    public class TaskKeeperJoint
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Bağlı olduğu görev (TaskStatus.Id)
        public Guid TaskId { get; set; }

        // Bu göreve atanan katılımcı (Users.Id)
        public Guid UserId { get; set; }

        // === Katılımcı bazlı aşama takibi ===
        // "ToBeDone", "InProcess", "InEditing", "Completed"
        // Her katılımcı görevde kendi aşamasını ilerletir. Görev kartında
        // hangi kullanıcının hangi aşamada olduğu bu alan üzerinden gösterilir.
        public string? Status { get; set; } = "ToBeDone";

        // Katılımcının aşama tarihleri
        public DateTime? DateToBeDone { get; set; }
        public DateTime? DateInProcess { get; set; }
        public DateTime? DateInEditing { get; set; }
        public DateTime? DateCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IsDeleted? IsDeleted { get; set; }
    }
}
```

## Dosya: TaskNotes.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data._Tasks
{
    public class TaskNotes
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskStatusId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string? Note { get; set; }
        public DateTime? NoteCreatedAt { get; set; }
        public bool? IsTheNoteOk { get; set; }
        public IsDeleted? IsDeleted { get; set; }


    }
}
```

## Dosya: TaskStatus.cs
```csharp
﻿using data.Owned;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace data._Tasks
{
    public class TaskStatus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Görevin ait olduğu kategori Guid
        public Guid? TaskCategoriesId { get; set; }

        // Görevi atayan kişi Guid tipinde
        public Guid? AssignedByUserId { get; set; } 

        // Görevin öncelik seviyesi (örneğin: "Düşük", "Orta", "Yüksek")
        public string? Priority { get; set; }

        // Görevi kimin üstlendiği Guid
        public Guid? PersonInChargeUserId { get; set; } // Görevin üstlenen kişi UsersType Sadece "Admin" olabilir

        // Görev adı
        public string? TaskTitle { get; set; }

        // Görev açıklaması
        public string? TaskDescription { get; set; }

        // Görevin tamamlanması gereken tarih
        public DateTime? TargetDate { get; set; }

        // "ToBeDone", "InProcess", "InEditing", "Completed"
        public string? Status { get; set; }

        // Bildirim Email gönderilme durumu
        public bool? IsNew { get; set; }
        public bool? IsToBeDone { get; set; }
        public bool? IsInProgress { get; set; }
        public bool? IsInEditing { get; set; }
        public bool? IsCompleted { get; set; }

        // Tarihsel veriler
        public DateTime? DateToBeDone { get; set; } = DateTime.Now;
        public DateTime? DateInProcess { get; set; } = DateTime.Now;
        public DateTime? DateInEditing { get; set; } = DateTime.Now;
        public DateTime? DateCompleted { get; set; } = DateTime.Now;
        public DateTime DateCreatedAt { get; set; } = DateTime.Now;
        public IsDeleted? IsDeleted { get; set; }
    }
}
```

