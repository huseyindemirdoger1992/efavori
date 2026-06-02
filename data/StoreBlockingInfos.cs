using data._Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace data
{
    public class StoreBlockingInfos
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } // Mağazayı açan kullanıcı (satıcı) ID'si
        public Guid? AdminId { get; set; } // Onay işlemini yürüten yönetici ID'si
        public Guid StoreId { get; set; } // Hangi mağazanın engellendiği/onaylanmadığı ID bilgisi

        public string BlockInfoDescription { get; set; } // Engelleme veya onaylanmama nedeni açıklaması

        public bool IsSucces { get; set; } // Engelleme veya onaylanmama sebebinin çözülüp çözülmediği bilgisi
        public DateTime AtDateTime { get; set; } // Engelleme veya onaylanmama işleminin gerçekleştiği tarih ve saat bilgisi
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
