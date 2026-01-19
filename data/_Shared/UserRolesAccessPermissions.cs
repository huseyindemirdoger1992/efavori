using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data._Shared
{
    [Owned]
    public class UserRolesAccessPermissions
    {
        public bool? IsItStaff { get; set; }
    }
}
