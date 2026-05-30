using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class StockQuantityForProduct
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public double QuantityOfProductsPurchased { get; set; } // Satın alınan ürün miktarı (örneğin 2.5 kg, 3 adet, 1.5 litre gibi)
        public string QuantityType { get; set; } // Kg, Adet, uzunluk, litre gibi birim türleri

        public Guid ProductId { get; set; } // İşlemin hangi ürüne ait olduğu net olmalı
        public Guid ProcessId { get; set; } // Sipariş veya Fatura ID'si (İlişkili işlem)
                                            
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Verinin ilk kaydedilme tarihi

        public DateTime? ModifiedDate { get; set; } // Ürün miktarındaki değişiklikler (örneğin iade, iptal gibi durumlarda) için son güncelleme tarihi

        public bool IsActive { get; set; } // sipariş iptal edildiğinde bu kaydı pasif yaparak stok güncellemesini engellemek için
    }
}
