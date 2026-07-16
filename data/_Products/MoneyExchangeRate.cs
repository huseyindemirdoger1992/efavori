using System;
using System.ComponentModel.DataAnnotations;

namespace data
{
    public class MoneyExchangeRate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Dolar_Usd { get; set; }
        public decimal Euro_Eur { get; set; }
        public decimal Manat_Azn { get; set; }
        public decimal Lira_Tl { get; set; }

        // Verinin çekildiği an
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}