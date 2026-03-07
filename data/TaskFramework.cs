using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
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
