using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    /// <summary>
    /// 10 dili destekleyen, yeniden kullanılabilir çok dilli metin owned sınıfı.
    /// Aynı entity içerisinde birden fazla property olarak kullanılabilir
    /// (Örn: Name, Description, Tooltip, Placeholder, HelpText).
    /// EF Core her kullanım için kolonlara otomatik prefix verir (Name_Tr, Tooltip_En vb.).
    /// </summary>
    [Owned]
    public class LangText
    {
        public string? Tr { get; set; } = string.Empty;
        public string? En { get; set; } = string.Empty;
        public string? Az { get; set; } = string.Empty;
        public string? De { get; set; } = string.Empty;
        public string? Es { get; set; } = string.Empty;
        public string? Fr { get; set; } = string.Empty;
        public string? Hi { get; set; } = string.Empty;
        public string? Pt { get; set; } = string.Empty;
        public string? Ru { get; set; } = string.Empty;
        public string? Zh { get; set; } = string.Empty;
    }
}
