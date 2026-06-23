using Microsoft.EntityFrameworkCore;

namespace data._Shared
{
    [Owned]
    public class InteractionCounts
    {
        public int? ViewCount { get; set; }            // Görüntülenme
        public int? ShareCount { get; set; }           // Paylaşma
        public int? RecommendCount { get; set; }       // Tavsiye Etme

    }
}