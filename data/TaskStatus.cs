using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class TaskStatus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Görevi atayan kişi Guid tipinde
        public Guid? AssignedByUserId { get; set; } // Görevin atandığı kişi UsersType Sadece "Admin" olabilir

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
        public DateTime? DateToBeDone { get; set; } = DateTime.Now;
        public DateTime? DateInProcess { get; set; } = DateTime.Now;
        public DateTime? DateInEditing { get; set; } = DateTime.Now;
        public DateTime? DateCompleted { get; set; } = DateTime.Now;
        public DateTime DateCreatedAt { get; set; } = DateTime.Now;
        public IsDeleted? IsDeleted { get; set; }
    }
}
