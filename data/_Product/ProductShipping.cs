using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün/varyant lojistik (kargo) bilgileri: ağırlık ve ölçüler.
    /// VariantId = null -> ürün/taban ölçüsü; VariantId dolu -> o varyanta özel ölçü.
    /// Desi SAKLANMAZ; ham ölçü ve bölenlerden hesaplanır:
    ///   Yurtiçi desi  = (En * Boy * Yükseklik) / DomesticDesiDivisor (3000)
    ///   Yurtdışı desi = (En * Boy * Yükseklik) / InternationalDesiDivisor (5000)
    /// Kargo ücretinde ağırlık ile desiden büyük olan esas alınır.
    /// </summary>
    public class ProductShipping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Bilgiyi ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // Bilginin ait olduğu ana ürünün ID'si (her zaman dolu)
        public Guid? VariantId { get; set; } // Bilginin ait olduğu varyantın ID'si (basit üründe null)

        public decimal WeightKg { get; set; } // Ağırlık (kg)
        public decimal WidthCm { get; set; } // En (cm)
        public decimal LengthCm { get; set; } // Boy (cm)
        public decimal HeightCm { get; set; } // Yükseklik (cm)
        public string? PackageType { get; set; } // Paket tipi (Örn: "Koli", "Zarf", "Palet")

        public int DomesticDesiDivisor { get; set; } = 3000; // Yurtiçi desi böleni
        public int InternationalDesiDivisor { get; set; } = 5000; // Yurtdışı desi böleni

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
