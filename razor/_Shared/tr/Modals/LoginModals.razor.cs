using data; // Kendi veri modellerinin olduğu namespace
using api; // Kendi veri modellerinin olduğu namespace
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace razor._Shared.tr.Modals
{
    public partial class LoginModals : ComponentBase
    {
        [Inject] public IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private string _SelectedPhoneCode = string.Empty;
        private Users _user = new Users();

        protected List<Country> _Countries = new();
        protected List<States> _States = new();
        protected List<Cities> _Cities = new();

        private string _Sponsored = string.Empty;
        private string? _SponsorFullName = null;

        private async Task CheckSponsorAsync()
        {
            _SponsorFullName = null;
            if (!string.IsNullOrWhiteSpace(_Sponsored))
            {
                using var db = await DbFactory.CreateDbContextAsync();
                var sponsor = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.ContactInformation != null && u.ContactInformation.Email == _Sponsored);

                if (sponsor != null)
                {
                    TextualFunctions tf = new TextualFunctions();
                    _SponsorFullName = $"{tf.FirstLastLetter(sponsor.FirstName)} {tf.FirstLastLetter(sponsor.LastName)}";

                }
            }
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnParametersSetAsync()
        {
            await CheckSponsorAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            using var db = await DbFactory.CreateDbContextAsync();
            _Countries = await db.Country.AsNoTracking().OrderBy(c => c.name).ToListAsync();
        }

        protected async Task OnCountrySelected(ChangeEventArgs e)
        {
            int.TryParse(e.Value?.ToString(), out int countryId);
            _States.Clear();
            _Cities.Clear();

            if (countryId > 0)
            {
                using var db = await DbFactory.CreateDbContextAsync();
                _States = await db.States.AsNoTracking()
                    .Where(s => s.country_id == countryId)
                    .OrderBy(s => s.name).ToListAsync();
            }
            StateHasChanged();
        }

        protected async Task OnStateSelected(ChangeEventArgs e)
        {
            int.TryParse(e.Value?.ToString(), out int stateId);
            _Cities.Clear();

            if (stateId > 0)
            {
                using var db = await DbFactory.CreateDbContextAsync();
                _Cities = await db.Cities.AsNoTracking()
                    .Where(c => c.state_id == stateId)
                    .OrderBy(c => c.name).ToListAsync();
            }
            StateHasChanged();
        }
    }
}