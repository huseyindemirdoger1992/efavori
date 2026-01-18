using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data._Shared
{
    [Owned]
    public class IsDeleted
    {
        // Silinme durumu (Soft Delete)
        public bool IsDeletedStatu { get; set; } = false;

        // Silinme tarihi
        public DateTime? DeletedAtDate { get; set; }
    }
}
