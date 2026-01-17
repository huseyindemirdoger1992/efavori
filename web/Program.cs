using data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Görünüm ve Kodlama Ayarlarý ---
// Türkçe karakterlerin HTML çýktýsýnda bozulmamasý için (Örn: þ, ð, ü)
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

builder.Services.AddHttpContextAccessor();

// Geliþtirme aþamasýnda Razor sayfalarýnýn yenilenmesi için
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();
}
else
{
    builder.Services.AddControllersWithViews();
}

builder.Services.AddServerSideBlazor();

// --- 2. Veritabaný ve Baðlantý Yapýlandýrmasý ---
// Baðlantý cümlesini appsettings.json veya User Secrets'tan çeker
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Baðlantý cümlesi 'DefaultConnection' bulunamadý.");

// NOT: Hem AddDbContext hem AddDbContextPool kullanýlmaz. 
// Yüksek performans için AddDbContextPool tercih edilmiþtir.
builder.Services.AddDbContextPool<_ApplicationConnectionDb>(options =>
{
    options.UseSqlServer(connectionString, sql =>
    {
        sql.CommandTimeout(30); // Sorgu zaman aþýmý
        // Baðlantý kopmalarýna karþý otomatik yeniden deneme (Resiliency)
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors(); // Hatalarý detaylandýr
        options.EnableSensitiveDataLogging(); // Loglarda SQL parametrelerini gör (Geliþtirme için)
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
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Sadece HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Oturum süresi
        options.SlidingExpiration = true; // Hareket varsa süreyi uzat
    });

builder.Services.AddAuthorization();

// --- 4. Dosya Yükleme Limitleri (128 MB) ---
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = 134217728; });
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = 134217728; });

// --- 5. Uygulama Hattý (Middleware) ---
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Önemli: Authentication her zaman Authorization'dan önce gelmeli
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapBlazorHub();

app.Run();