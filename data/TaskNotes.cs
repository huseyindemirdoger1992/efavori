using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
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
