using data._Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Articles
{
    public class AiTitlesForArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }

        public bool? AiIsOk { get; set; }

        // AI retry üst limiti — bu sayıya ulaşan başlık tekrar denenmez
        public int AiRetryCount { get; set; } = 0;
        public string? AiErrorMessage { get; set; }
        public DateTime? AiProcessedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}