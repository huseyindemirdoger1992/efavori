using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data
{
    public class StoreIntegration
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid UserId { get; set; }

        // Entegratör adı (Örn: "Parasut", "Logo", "Uyumsoft")
        public string Provider { get; set; }

        // --- Kimlik Doğrulama Bilgileri (Hepsi şifrelenerek saklanmalı) ---

        public string EncryptedApiKey { get; set; }

        public string EncryptedApiSecret { get; set; }

        // Bazı entegratörler (Örn: Logo) için zorunludur
        public string EncryptedUserName { get; set; }

        public string EncryptedPassword { get; set; }

        // Şube kodu veya firma ID gibi özel değerler için
        public string CompanyCode { get; set; }

        // --- Operasyonel Alanlar ---

        public bool IsDefault { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();

    }
}
