using api;
using data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Altyapı ve Kodlama Ayarları ---
ConfigureInfrastructure(builder);

// --- 2. Localization (Çoklu Dil) Yapılandırması ---
ConfigureLocalization(builder.Services);

// --- 3. Veritabanı ve Veri Katmanı ---
ConfigurePersistence(builder);

// --- 4. Güvenlik ve Kimlik Doğrulama ---
ConfigureSecurity(builder.Services);

// --- 5. Web Sunucu ve Limitler ---
ConfigureServerLimits(builder.Services);

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddScoped<UserInfos>(); // UserInfos'un yaşam döngüsü
builder.Services.AddScoped<TakeLogs>();  // TakeLogs'un yaşam döngüsü

var app = builder.Build();

// --- 6. Middleware Pipeline (Sıralama Düzenlendi) ---
ConfigureMiddlewarePipeline(app);

// --- 7. SEO Uyumlu Endpoint Tanımları ---
ConfigureEndpoints(app);
app.MapControllers();

app.Run();

#region Configuration Methods

void ConfigureInfrastructure(WebApplicationBuilder b)
{
    b.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
    b.Services.AddHttpContextAccessor();
    b.Services.AddDistributedMemoryCache();
    b.Services.AddScoped<UserInfos>();

    var mvcBuilder = b.Services.AddControllersWithViews()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization();

    if (b.Environment.IsDevelopment())
    {
        mvcBuilder.AddRazorRuntimeCompilation();
    }

    b.Services.AddServerSideBlazor();
    b.Services.AddRazorComponents().AddInteractiveServerComponents();
}

void ConfigureLocalization(IServiceCollection services)
{
    services.AddLocalization(options => options.ResourcesPath = "Resources");
    services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" }
            .Select(c => new CultureInfo(c)).ToList();

        options.DefaultRequestCulture = new RequestCulture("en");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Clear();
        options.RequestCultureProviders.Add(new RouteDataRequestCultureProvider { RouteDataStringKey = "culture" });
        options.RequestCultureProviders.Add(new CookieRequestCultureProvider
        {
            CookieName = ".Efavori.Culture"
        });
    });
}

void ConfigurePersistence(WebApplicationBuilder b)
{
    var connectionString = b.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    b.Services.AddDbContextPool<_ApplicationConnectionDb>(options =>
        ConfigureDbOptions(options, connectionString, b.Environment));

    b.Services.AddDbContextFactory<_ApplicationConnectionDb>(
        options => ConfigureDbOptions(options, connectionString, b.Environment),
        ServiceLifetime.Singleton);
}

void ConfigureDbOptions(DbContextOptionsBuilder options, string connectionStr, IWebHostEnvironment env)
{
    options.UseSqlServer(connectionStr, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });

    if (env.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
}

void ConfigureSecurity(IServiceCollection services)
{
    services.AddHttpClient();
    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = ".Efavori.Auth.Identity";
            options.Cookie.Path = "/";
            options.LoginPath = "/";
            options.LogoutPath = "/tr/Account/Logout";
            options.AccessDeniedPath = "/tr/Account/Logout";
            options.Cookie.HttpOnly = true;

            // Geliştirme ortamında çerezlerin yazılabilmesi için 'SameAsRequest' yapıldı.
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;

            options.ExpireTimeSpan = TimeSpan.FromDays(365);
            options.SlidingExpiration = true;
            options.Cookie.MaxAge = options.ExpireTimeSpan;
        });

    services.AddAuthorization();
}

void ConfigureServerLimits(IServiceCollection services)
{
    const long maxRequestLimit = 134217728;
    services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = maxRequestLimit);
    services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = maxRequestLimit);
}

void ConfigureMiddlewarePipeline(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    // --- KRİTİK SIRALAMA: Localization, Auth'dan önce gelmeli ---
    var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(locOptions.Value);
    app.UseAuthentication();
    app.UseAuthorization();
}

void ConfigureEndpoints(WebApplication app)
{
    app.MapControllerRoute(
        name: "areaRoute",
        pattern: "{culture}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{culture}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "account",
        pattern: "{culture}/Account/{action=Login}",
        defaults: new { controller = "Account" });

    app.MapGet("/", context => {
        var culture = GetPreferredCulture(context);
        context.Response.Redirect($"/{culture}/Customer/Home/Index");
        return Task.CompletedTask;
    });

    app.MapGet("/{culture}", (string culture, HttpContext context) => {
        var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };
        if (!supportedCultures.Contains(culture.ToLower()))
        {
            culture = GetPreferredCulture(context);
        }
        context.Response.Redirect($"/{culture}/Customer/Home/Index");
        return Task.CompletedTask;
    });

    app.MapBlazorHub();
}

string GetPreferredCulture(HttpContext context)
{
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };
    const string defaultCulture = "en";

    if (context.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) &&
        !string.IsNullOrEmpty(cookieValue))
    {
        var culture = CookieRequestCultureProvider.ParseCookieValue(cookieValue);
        if (culture != null)
        {
            var lang = culture.Cultures.FirstOrDefault().Value?.ToLower();
            if (!string.IsNullOrEmpty(lang) && supportedCultures.Contains(lang))
            {
                return lang;
            }
        }
    }

    return defaultCulture;
}
#endregion