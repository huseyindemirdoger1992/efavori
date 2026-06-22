using Microsoft.EntityFrameworkCore;

namespace data._Shared
{
    [Owned]
    public class InteractionCounts
    {
        public int? ViewCount { get; set; }
        public int? FavoriteCount { get; set; }        // Favoriye Ekleme
        public int? CompareCount { get; set; }         // Karşılaştırma
        public int? ShareCount { get; set; }           // Paylaşma
        public int? RecommendCount { get; set; }       // Tavsiye Etme
        public int? AskSellerCount { get; set; }       // Satıcıya Sorma
        public int? NotifyPriceDropCount { get; set; } // Fiyat Düşünce Haber Verme
    }
}