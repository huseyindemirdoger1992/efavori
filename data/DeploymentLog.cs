using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    [Table("DeploymentLog", Schema = "efavori")]
    public class DeploymentLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string ProjectName { get; set; } = null!;

        [Required]
        public string CommitMessage { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string DeployedBy { get; set; } = null!;

        [Required]
        public DateTime DeployDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "null";
    }
}
