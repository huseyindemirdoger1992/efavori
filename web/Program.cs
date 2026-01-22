using api;
using data;
using Microsoft.AspNetCore.Authentication.Cookies;
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

// --- 1. Altyapý ve Kodlama Ayarlarý ---
ConfigureInfrastructure(builder);

// --- 2. Localization (Çoklu Dil) Yapýlandýrmasý ---
ConfigureLocalization(builder.Services);

// --- 3. Veritabaný ve Veri Katmaný ---
ConfigurePersistence(builder);

// --- 4. Güvenlik ve Kimlik Doðrulama (1 Yýllýk Çerez) ---
ConfigureSecurity(builder.Services);

// --- 5. Web Sunucu ve Limitler ---
ConfigureServerLimits(builder.Services);

var app = builder.Build();

// --- 6. Middleware Pipeline ---
ConfigureMiddlewarePipeline(app);

// --- 7. SEO Uyumlu Endpoint Tanýmlarý ---
ConfigureEndpoints(app);

app.Run();

#region Configuration Methods

void ConfigureInfrastructure(WebApplicationBuilder b)
{
    b.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
    b.Services.AddHttpContextAccessor();

    // Session kaldýrýldý, yerine verileri çerezde tutacaðýz. 
    // Ancak bazý kütüphaneler için cache altyapýsý kalabilir.
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

        // Provider sýrasý önemli: Route > Cookie > Header
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
            options.LoginPath = "/Security/Login/";
            options.LogoutPath = "/Security/Logout/";
            options.AccessDeniedPath = "/Security/Logout/";
            options.Cookie.Name = ".Efavori.Auth.Identity";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;

            // --- ÖMÜR AYARI ---
            options.ExpireTimeSpan = TimeSpan.FromDays(365);
            options.SlidingExpiration = true; // Kullanýcý siteye girdikçe 1 yýl süresi resetlenir
            options.Cookie.MaxAge = options.ExpireTimeSpan; // Tarayýcý tarafýnda kalýcýlýk saðlar
        });

    services.AddAuthorization();
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); 

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
    // app.UseSession(); // Çerez yapýsýna geçildiði için kaldýrýldý
    app.UseRouting();

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

    // Ana sayfa - Cookie'den dil tercihini oku
    app.MapGet("/", context => {
        var culture = GetPreferredCulture(context);
        context.Response.Redirect($"/{culture}/Customer/Home/Index");
        return Task.CompletedTask;
    });

    // Dil parametresi ile giriþ
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

// Cookie'den kullanýcýnýn dil tercihini al
string GetPreferredCulture(HttpContext context)
{
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };
    const string defaultCulture = "en";

    // Cookie'yi oku
    if (context.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) &&
        !string.IsNullOrEmpty(cookieValue))
    {
        // ASP.NET Core formatý: c=tr|uic=tr
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