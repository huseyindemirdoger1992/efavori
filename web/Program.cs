using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Encodings.Web;
using System.Text.Unicode;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddHttpContextAccessor();
// 1. MVC Yapýlandýrmasý
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();
}
else
{
    builder.Services.AddControllersWithViews();
}
// 2. Blazor Server
builder.Services.AddServerSideBlazor();

// 4. HTTP Client
builder.Services.AddHttpClient();
// 5. AUTHENTICATION (Kullanýcý oturum yönetimi ve Yönlendirme)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Oturum açmamýþ kullanýcý [Authorize] bir sayfaya girerse buraya gider:
        options.LoginPath = "/Security/Login/";
        // Yetkisi olmayan bir alana (Role hatasý) girmeye çalýþýrsa buraya gider:
        options.AccessDeniedPath = "/Security/Logout/";
        // Çýkýþ yapýldýðýnda yönlendirilecek varsayýlan adres:
        options.LogoutPath = "/Security/Logout/";
    });
builder.Services.AddAuthorization();
// 6. DOSYA YÜKLEME LÝMÝTLERÝ (128 MB)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 134217728;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 134217728;
});
var app = builder.Build();
// 7. MIDDLEWARE (Sýralama kritiktir)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Authentication her zaman Authorization'dan önce gelmelidir
app.UseAuthentication();
app.UseAuthorization();
// 8. ENDPOINT TANIMLAMALARI
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapBlazorHub();
app.Run();