using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Security.Cryptography;
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

        // Yapay zeka yapılan yorumu kontrol eder ve eğer koşullar sağlanıyorsa true döner. Aksi takdirde false döner.
        public bool? ConfirmedByAi { get; set; }

        // Yapay zeka eğer yorumu onaylamaz ise nedenini burada belirtir. Eğer yorum onaylanmışsa bu alan null olur.
        public string? WhyDidAiNotApproveIt { get; set; }

        // Yorumun oluşturulma tarihi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Yorumun AI tarafından kontrol edilme tarihi. Bu tarih, yorum oluşturulduktan sonra AI tarafından kontrol edildiğinde güncellenir.
        public DateTime? AIControlDate { get; set; }
        public IsDeleted IsDeleted { get; set; }

    }
}
