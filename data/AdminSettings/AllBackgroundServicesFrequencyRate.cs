using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data.AdminSettings
{
    public class AllBackgroundServicesFrequencyRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = 1; // Tek satır olacağı için ID'yi 1'e sabitleyip kapatıyoruz.

        // Döviz kuru çekme durumu izni
        public bool IsCurrencyFetchEnabled { get; set; }

        // Döviz kuru çekme sıklığı (saniye cinsinden)
        public int CurrencyFetchIntervalInSeconds { get; set; }

        // AI ürün içerik üretimi durumu izni
        public bool IsAiContentGenerationEnabled { get; set; }

        // AI ürün içerik üretimi sıklığı (saniye cinsinden)
        public int AiContentGenerationIntervalInSeconds { get; set; }

        // AI ürün içerik üretimi için maksimum deneme sayısı
        public int AiContentGenerationIntervalMaxAiRetry { get; set; }
    }
}
