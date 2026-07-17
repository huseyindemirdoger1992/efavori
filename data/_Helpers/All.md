## Dosya: SupportTickets.cs
```csharp
﻿using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Helper
{
    public class SupportTickets
    {
        [Key]
        // Açıklama: Destek kaydının veritabanı işlemlerinde benzersiz şekilde tanınmasını sağlayan anahtar değerdir.
        public Guid Id { get; set; } = Guid.NewGuid();

        // Açıklama: Destek talebini oluşturan sistem kullanıcısının benzersiz kimlik bilgisidir.
        public Guid UserId { get; set; }

        // Açıklama: Kullanıcının yaşadığı sorunu kısaca özetleyen başlık metnidir.
        public string Subject { get; set; }

        // Açıklama: Kullanıcının destek talebiyle ilgili detaylı açıklama veya mesaj içeriğidir.
        public string UserResponse { get; set; }

        // Açıklama: Talebin sisteme ilk giriş yaptığı tarih ve zaman bilgisidir.
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Açıklama: Talebin son güncellenme tarihini tutan isteğe bağlı bir alandır.
        public DateTime? UpdatedAt { get; set; }

        // Açıklama: Destek talebine yetkili bir yönetici tarafından verilen yanıt metnidir.
        public string? AdminResponse { get; set; }

        // Açıklama: Talebi kapatan yöneticinin benzersiz kimlik bilgisidir.
        public Guid? AssignedAdminId { get; set; }

        // Açıklama: Talebin mevcut işleme durumunu (Open, Waiting, Closed) belirten alandır.
        public string Status { get; set; } = "Open";

        // Açıklama: Talebin öncelik derecesini (Low, Medium, High) tanımlayan alandır. Değer ataması Admin tarafından yapılır.
        public string Priority { get; set; } = "Medium";

        // Açıklama: Talebin silinip silinmediğini belirten alandır.
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}```

