using System;

namespace data._Users
{
    /// <summary>
    /// KULLANICI ADRES DEFTERİ (§36).
    ///
    /// Ülke/il/ilçe artık serbest METİN DEĞİL, <c>data._Locations</c> tablolarına FK'dir;
    /// böylece kargo bölgesi (ShippingZoneAreas) eşleşmesi ve vergi hesabı güvenilir olur.
    /// Gösterim için kullanılan denormalize metin alanları da tutulur — konum kayıtları
    /// sonradan yeniden adlandırılsa bile adresin yazıldığı andaki hâli korunur.
    ///
    /// SİPARİŞ İLİŞKİSİ (§60): Sipariş bu tabloya FK VERMEZ. Checkout sırasında adresin
    /// tam kopyası <c>data._Orders.Orders</c> içindeki snapshot alanlarına yazılır.
    /// Kullanıcı adresini sonradan değiştirse veya silse bile geçmiş sipariş bozulmaz.
    /// </summary>
    public class UserAddress : UserEntityBase
    {
        /// <summary>Adresin sahibi (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Kullanıcının adrese verdiği ad ("Ev Adresim", "İş Adresim").</summary>
        public string AddressName { get; set; } = string.Empty;

        /// <summary>Bireysel / kurumsal ayrımı. Eski <c>string AddressType</c> yerine geçer.</summary>
        public AddressType AddressType { get; set; } = AddressType.Individual;

        // ── Alıcı bilgileri ───────────────────────────────────────────────────
        /// <summary>Teslim alacak kişinin adı.</summary>
        public string ReceiverFirstName { get; set; } = string.Empty;

        /// <summary>Teslim alacak kişinin soyadı.</summary>
        public string ReceiverLastName { get; set; } = string.Empty;

        /// <summary>Teslim alacak kişinin e-posta adresi (kargo bildirimi için).</summary>
        public string? ReceiverEmail { get; set; }

        /// <summary>Alıcı telefon ülke kodu (ör. "+90").</summary>
        public string? ReceiverPhoneCountryCode { get; set; }

        /// <summary>Alıcı telefon numarası.</summary>
        public string ReceiverPhoneNumber { get; set; } = string.Empty;

        // ── Yapısal konum (FK) ────────────────────────────────────────────────
        /// <summary>Ülke (data._Locations.Country.Id).</summary>
        public int CountryId { get; set; }

        /// <summary>İl/eyalet (data._Locations.States.Id).</summary>
        public int? StateId { get; set; }

        /// <summary>İlçe/şehir (data._Locations.Cities.Id).</summary>
        public int? CityId { get; set; }

        /// <summary>Bölge/semt (data._Locations.Regions.Id).</summary>
        public int? RegionId { get; set; }

        // ── Denormalize gösterim metinleri (yazıldığı andaki hâl) ─────────────
        /// <summary>Ülke adı (gösterim kopyası).</summary>
        public string? CountryName { get; set; }

        /// <summary>İl/eyalet adı (gösterim kopyası).</summary>
        public string? StateName { get; set; }

        /// <summary>İlçe/şehir adı (gösterim kopyası).</summary>
        public string? CityName { get; set; }

        // ── Adres metni ───────────────────────────────────────────────────────
        /// <summary>Açık adres — sokak, cadde, bina no.</summary>
        public string AddressLine1 { get; set; } = string.Empty;

        /// <summary>Adres devamı — daire, kat, tarif (opsiyonel).</summary>
        public string? AddressLine2 { get; set; }

        /// <summary>Posta kodu.</summary>
        public string? PostalCode { get; set; }

        /// <summary>Kurye için serbest teslimat notu ("Zili çalmayın, kapıya bırakın").</summary>
        public string? DeliveryNote { get; set; }

        // ── Coğrafi konum (harita ve kargo optimizasyonu) ─────────────────────
        /// <summary>Enlem (decimal(9,6)).</summary>
        public decimal? Latitude { get; set; }

        /// <summary>Boylam (decimal(9,6)).</summary>
        public decimal? Longitude { get; set; }

        // ── Kurumsal fatura bilgileri (AddressType = Corporate iken zorunlu) ──
        /// <summary>Fatura unvanı (şirket adı).</summary>
        public string? CompanyName { get; set; }

        /// <summary>Vergi dairesi.</summary>
        public string? TaxOffice { get; set; }

        /// <summary>Vergi/TCKN numarası. KİŞİSEL VERİ — yalnızca faturalama servisinde okunur.</summary>
        public string? TaxNumber { get; set; }

        // ── Varsayılanlar ─────────────────────────────────────────────────────
        /// <summary>Varsayılan TESLİMAT adresi mi? Kullanıcı başına en fazla bir tane true.</summary>
        public bool IsDefaultShipping { get; set; }

        /// <summary>Varsayılan FATURA adresi mi? Kullanıcı başına en fazla bir tane true.</summary>
        public bool IsDefaultBilling { get; set; }

        /// <summary>Adres doğrulama servisinden geçti mi? (kargo firması API doğrulaması)</summary>
        public bool IsValidated { get; set; }

        /// <summary>Adresin doğrulandığı an (UTC).</summary>
        public DateTime? ValidatedAtUtc { get; set; }
    }
}
