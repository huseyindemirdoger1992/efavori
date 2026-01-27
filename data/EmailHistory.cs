using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class EmailHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FromWhom { get; set; }
        public string ToWho { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
    }
}
