using data.Owned;
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
