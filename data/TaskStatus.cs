using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace data
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
