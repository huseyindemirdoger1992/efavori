using api;
using data;
using data._Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace razor._Shared.tr.Modals.Account
{
    public partial class NewUserRegister : ComponentBase
    {
        [Inject] public IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private Users _user = new Users{};

        protected List<Country> _Countries = new();
        protected List<States> _States = new();
        protected List<Cities> _Cities = new();

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
                    StateHasChanged();
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

        private string? _Sponsored = string.Empty;
        private string? _SponsorFullName = null;
        private string? _SelectedPhoneCode = string.Empty;
        private string? _UserEmail = string.Empty;
        private string? _UserPhoneNumber = string.Empty;
        public async Task UserSave()
        {        
            TextualFunctions tf = new TextualFunctions();
            _user.ContactInformation = new ContactInformation();
            _user.ContactInformation.CountryPhoneCode = !string.IsNullOrWhiteSpace(_SelectedPhoneCode) ? _SelectedPhoneCode : "90";
            _user.ContactInformation.PhoneNumber = tf.NormalizePhoneNumberEditor(_UserPhoneNumber);
            _user.ContactInformation.Email = _UserEmail;
            _user.UserSponsorEmail = _Sponsored;
            _user.HeaderMenuType = _user.UsersType;
            _user.RegistrationDate = DateTime.UtcNow;
            using var db = await DbFactory.CreateDbContextAsync();
            db.Users.Add(_user);
            await db.SaveChangesAsync();
        }
    }
}