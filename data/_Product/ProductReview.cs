using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Text;

namespace data._Product
{
    public class ProductReview
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public string CommentText { get; set; }

        // Eğer bu alan null ise ana yorumdur. 
        // Bir değer içeriyorsa, yanıt verilen üst yorumun ID'sini tutar.
        public Guid? ParentReviewId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IsDeleted IsDeleted { get; set; }

    }
}
