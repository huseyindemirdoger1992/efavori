using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace razor._Shared.tr.Modals.Account
{
    public partial class ResetPassword : ComponentBase
    {
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private Notification? notificationRef;

        private bool isProcessing = false;

        private async Task ResetPasswordAction()
        {
            string email = "";
        }
    }
}
