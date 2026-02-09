using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    public class WorkstationEmployeeGroup
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? GroupName { get; set; } // Çalışan grubunun adı
        public string? Description { get; set; } // Çalışan grubunun açıklaması
        public ContactInformation? ContactInformation { get; set; }
        public ProfileCoverGallery? ProfileCoverGallery { get; set; }
        public AddressInfo? AddressInfo { get; set; }
        public IsDeleted? IsDeleted { get; set; }
        public DateTime? CreateAtDate { get; set; } 
    }
}
