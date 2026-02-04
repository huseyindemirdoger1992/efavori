using data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace razor._Shared.tr.Media
{
    public partial class List
    {
        [Parameter] public Users? use { get; set; }
        [Parameter] public bool? IsMultiSelect { get; set; }
    }
}
