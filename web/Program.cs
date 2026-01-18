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

// --- 1. Görünüm, Kodlama ve Çoklu Dil Ayarlarý ---
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

builder.Services.AddHttpContextAccessor();

// Kendi sýnýfýný sisteme kaydet
builder.Services.AddScoped<api.UserInfos>();

// Dil dosyalarýnýn (Resources) aranacaðý klasörü belirtiyoruz
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var mvcBuilder = builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddServerSideBlazor();

// --- 2. Veritabaný Yapýlandýrmasý ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Baðlantý cümlesi 'DefaultConnection' bulunamadý.");

builder.Services.AddDbContextPool<_ApplicationConnectionDb>(options =>
{
    options.UseSqlServer(connectionString, sql =>
    {
        sql.CommandTimeout(30);
        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// --- 3. Güvenlik ve Kimlik Doðrulama ---
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

// --- 4. Dosya Yükleme Limitleri ---
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = 134217728; });
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = 134217728; });

// --- 5. Dil Seçeneklerini Yapýlandýr (TR, EN, DE) ---
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("az"),
        new CultureInfo("de"),
        new CultureInfo("es"),
        new CultureInfo("fr"),
        new CultureInfo("hi"),
        new CultureInfo("pt"),
        new CultureInfo("ru"),
        new CultureInfo("tr"),
        new CultureInfo("zh"),
        new CultureInfo("en")
    };

    // Varsayýlan dil EN
    options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // URL'den kültür bilgisini okumak için Provider ekliyoruz
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
});

// --- 6. Uygulama Hattý (Middleware) ---
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Dil ayarlarýný etkinleþtir
var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseAuthentication();
app.UseAuthorization();

// --- 7. SEO Uyumlu Route Yapýsý ({culture} parametresi eklendi) ---
// Areas rotasý (Eski default rotanýn ÜSTÜNE ekle)
app.MapControllerRoute(
    name: "areas",
    pattern: "{culture=en}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Mevcut default rotan (Bunu koru)
app.MapControllerRoute(
    name: "default",
    pattern: "{culture=en}/{controller=Home}/{action=Index}/{id?}");

app.MapBlazorHub();

app.Run();