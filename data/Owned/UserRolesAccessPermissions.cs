using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class UserRolesAccessPermissions
    {
        public bool? IsItStaff { get; set; }
    }
}
