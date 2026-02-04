using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace razor._Shared
{
    public partial class ThisPageIsBeingPrepared : ComponentBase
    {
        // Gelen kullanıcı parametresi
        [Parameter] public Users? use { get; set; }
        // kullanıcı bilgileri

    }
}