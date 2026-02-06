using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class DeploymentLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? ProjectName { get; set; }
        public string? CommitMessage { get; set; }
        public string? DeployedBy { get; set; }
        public DateTime? DeployDate { get; set; } = DateTime.Now;
        public string? Status { get; set; } 
    }
}
