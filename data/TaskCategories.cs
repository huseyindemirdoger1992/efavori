using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class TaskCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid? WorkstationEmployeeGroupId { get; set; } // Bağlı olduğu İş istasyonu + 
        public string? CategoryStructure { get; set; } // PlanningTaskBoard - InProcessTaskBoard - CompletedTaskBoard
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public IsDeleted? IsDeleted { get; set; }
    }
}
