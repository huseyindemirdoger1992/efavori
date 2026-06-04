using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Depo/lojistik noktası tanımı. Mağaza bazlıdır.
    /// Adres bilgisi için mevcut AddressInfo owned type'ı yeniden kullanılır.
    /// </summary>
    public class Warehouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Depoyu ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Deponun bağlı olduğu mağazanın ID bilgisi

        public bool IsDefault { get; set; } = false; // Varsayılan depo mu
        public bool IsActive { get; set; } = true; // Depo aktif mi

        public AddressInfo? AddressInfo { get; set; } = new(); // Depo adres bilgisi (mevcut owned type)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
