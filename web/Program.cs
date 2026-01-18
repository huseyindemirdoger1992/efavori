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

// --- 1. Görünüm, Kodlama ve Çoklu Dil Ayarlarý (View, Encoding & Localization) ---

// Türkçe karakter sorunlarýný önlemek için tüm Unicode aralýklarýný kapsayan HtmlEncoder
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

builder.Services.AddHttpContextAccessor();

// Kullanýcý bilgilerini yönetmek için özel servis kaydý
builder.Services.AddScoped<api.UserInfos>();

// Dil dosyalarýnýn (Resources) aranacaðý klasörü belirtiyoruz
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var mvcBuilder = builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Geliþtirme aþamasýnda Razor dosyalarýnýn anlýk güncellenmesini saðlar
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddServerSideBlazor();

// --- 2. Veritabaný Yapýlandýrmasý (Database Configuration) ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Standart MVC kullanýmý için (Scoped)
builder.Services.AddDbContextPool<_ApplicationConnectionDb>(options =>
    ConfigureDbOptions(options, connectionString, builder.Environment));

// Blazor ve Asenkron süreçler için Factory (Singleton/Transient uyumlu)
// ServiceLifetime.Scoped parametresini sildik veya Singleton yaptýk
builder.Services.AddDbContextFactory<_ApplicationConnectionDb>(options =>
    ConfigureDbOptions(options, connectionString, builder.Environment));

// --- 3. Güvenlik ve Kimlik Doðrulama (Security & Authentication) ---

builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Security/Login/";
        options.AccessDeniedPath = "/Security/Logout/";
        options.LogoutPath = "/Security/Logout/";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// --- 4. Dosya Yükleme Limitleri (Upload Limits) ---
// 128 MB limit tanýmlanýyor
const long MaxRequestLimit = 134217728;
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = MaxRequestLimit; });
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = MaxRequestLimit; });

// --- 5. Dil Seçeneklerini Yapýlandýr (Localization Options) ---

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("tr"),
        new CultureInfo("en"),
        new CultureInfo("az"),
        new CultureInfo("de"),
        new CultureInfo("es"),
        new CultureInfo("fr"),
        new CultureInfo("hi"),
        new CultureInfo("pt"),
        new CultureInfo("ru"),
        new CultureInfo("zh")
    };

    options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // URL tabanlý dil yönetimi için (örn: site.com/tr/Home/Index)
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
});

// --- 6. Uygulama Ýþlem Hattý (Middleware Pipeline) ---

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Dil ayarlarýný (Localization) devreye al
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

app.UseAuthentication();
app.UseAuthorization();

// --- 7. SEO Uyumlu Route Yapýsý (Routing) ---

// Area (Bölge) desteði olan SEO uyumlu rotalama
app.MapControllerRoute(
    name: "areas",
    pattern: "{culture=en}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Varsayýlan SEO uyumlu rotalama
app.MapControllerRoute(
    name: "default",
    pattern: "{culture=en}/{controller=Home}/{action=Index}/{id?}");

app.MapBlazorHub();

app.Run();

// --- 8. Yardýmcý Metotlar (Helper Methods) ---

// Veritabaný konfigürasyonunu merkezi bir yerden yönetmek için
void ConfigureDbOptions(DbContextOptionsBuilder options, string connectionStr, IWebHostEnvironment env)
{
    options.UseSqlServer(connectionStr, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        // Baðlantý kopmalarýna karþý otomatik yeniden deneme mekanizmasý
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });

    if (env.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
}