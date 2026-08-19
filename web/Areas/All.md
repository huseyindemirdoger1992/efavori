## Dosya: All.bat
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\
```
@echo off
chcp 65001 >nul
echo Dosyalar birlestiriliyor...

(
    for /r %%f in (*.*) do (
        if /i not "%%~nxf"=="All.md" (
            echo "%%~dpf" | findstr /i /c:"\obj\" /c:"\Migrations\" /c:"\bin\" >nul || (
                echo ## Dosya: %%~nxf
                echo Konum: %%~dpf
                echo ```
                type "%%f"
                echo ```
                echo.
            )
        )
    )
) > All.md

echo Islem tamamlandi: All.md olusturuldu.
pause```

## Dosya: _Layout.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Security.Claims
@using System.Text.Encodings.Web
@using web.wwwroot
@using Microsoft.EntityFrameworkCore
@using System.Web
@using System.Text.Json;
@using Microsoft.AspNetCore.Http
@inject Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor
@{
    // ═══════════════════════════════════════════════════════════════
    // 1. DEĞİŞKEN BAŞLATMA
    // ═══════════════════════════════════════════════════════════════
    string BackgroundImage = "/assets/images/bg-themes/1.png";
    string lang = "en";
    string TotalLang = "en_EN";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    Users? use = null;

    // ═══════════════════════════════════════════════════════════════
    // 2. DİL VE VERİ ÇEKME MANTIĞI
    // ═══════════════════════════════════════════════════════════════
    try
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid userId;
        bool isGuid = Guid.TryParse(userIdStr, out userId);
        if (isGuid)
        {
            using (var db = new data._ApplicationConnectionDb())
            {
                use = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (use != null)
                {
                    if (!string.IsNullOrEmpty(use.BackgroundImagePath))
                        BackgroundImage = use.BackgroundImagePath;

                    if (!string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
                    {
                        lang = use.Language.ToLower();
                    }

                    try
                    {
                        using (var dbLog = new data._ApplicationConnectionDb())
                        {
                            var successLog = new Logs
                            {
                                Id = Guid.NewGuid(),
                                PageNameSpaceTitle = "Layout Loaded Successfully",
                                Action = "FetchUserData_Success",
                                UserId = use.Id,
                                Date = DateTime.UtcNow,
                                IpAddress = Context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                                UserAgent = Context.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                                RequestPath = Context.Request.Path,
                                Languages = Context.Request.Headers["Accept-Language"].ToString(),
                            };
                            await dbLog.Logs.AddAsync(successLog);
                            await dbLog.SaveChangesAsync();
                        }
                    }
                    catch { }
                }
            }
        }
        else
        {
            var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
            if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
            {
                lang = routeLang;
            }
            else if (Context.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        lang = langCode;
                }
            }
        }

        TotalLang = $"{lang}_{lang.ToUpper()}";
    }
    catch (Exception ex)
    {
        try
        {
            using (var dbLog = new data._ApplicationConnectionDb())
            {
                var errorLog = new Logs
                {
                    Id = Guid.NewGuid(),
                    PageNameSpaceTitle = "Layout Data Error",
                    Action = "FetchUserData_Error",
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace,
                    Date = DateTime.UtcNow,
                    IpAddress = Context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = Context.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                    RequestPath = Context.Request.Path,
                    Languages = Context.Request.Headers["Accept-Language"].ToString(),
                };
                await dbLog.Logs.AddAsync(errorLog);
                await dbLog.SaveChangesAsync();
            }
        }
        catch { }
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. SEO DEĞİŞKENLERİ (ViewData'dan dinamik besleme)
    // ═══════════════════════════════════════════════════════════════

    // --- Canonical URL: Query string olmadan, temiz path ---
    var baseUrl = $"{Context.Request.Scheme}://{Context.Request.Host}";
    var canonicalPath = Context.Request.Path.ToString().TrimEnd('/');
    if (string.IsNullOrEmpty(canonicalPath)) canonicalPath = "";
    var canonicalUrl = ViewData["CanonicalUrl"] as string ?? $"{baseUrl}{canonicalPath}";

    // --- Sayfa meta bilgileri ---
    var pageTitle = ViewData["Title"]?.ToString() ?? "efavori.com";
    var pageDescription = ViewData["Description"]?.ToString() ?? "efavori.com — Binlerce mağaza, milyonlarca ürün. Güvenli alışverişin adresi.";
    var pageKeywords = ViewData["Keywords"]?.ToString() ?? "efavori, online alışveriş, pazar yeri, marketplace";

    // --- Open Graph ---
    var ogType = ViewData["OgType"]?.ToString() ?? "website";
    var ogImage = ViewData["OgImage"]?.ToString() ?? "https://efavori.com/_files/main/logo/og-default.png";
    var ogImageWidth = ViewData["OgImageWidth"]?.ToString() ?? "1200";
    var ogImageHeight = ViewData["OgImageHeight"]?.ToString() ?? "630";

    // --- LCP Preload (ürün sayfaları ana görseli buradan geçirir) ---
    var preloadImage = ViewData["PreloadImage"] as string;

    // --- JSON-LD (bileşenler sunucu tarafında render edip bu alana basar) ---
    var jsonLdScript = ViewData["JsonLd"] as string;

    // --- Robots override (noindex sayfalar için) ---
    var robotsContent = ViewData["Robots"]?.ToString() ?? "index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1";

    // --- hreflang için lang-locale eşlemesi ---
    var langLocaleMap = new Dictionary<string, string>
    {
        { "tr", "tr-TR" }, { "en", "en-US" }, { "az", "az-AZ" },
        { "de", "de-DE" }, { "es", "es-ES" }, { "fr", "fr-FR" },
        { "hi", "hi-IN" }, { "pt", "pt-BR" }, { "ru", "ru-RU" },
        { "zh", "zh-CN" }
    };

    // ═══════════════════════════════════════════════════════════════
    // 4. DİNAMİK CSS/JS (MainCssJs) — Veritabanından aktif kodlar
    //    IsCssOrJs == true  -> CSS  (<head> içine <style> olarak)
    //    IsCssOrJs == false -> JS   (</body> öncesi <script> olarak)
    //    Soft delete edilmiş (IsDeletedStatu == true) ve pasif kayıtlar hariç.
    // ═══════════════════════════════════════════════════════════════
    var activeCssList = new List<MainCssJs>();
    var activeJsList = new List<MainCssJs>();
    try
    {
        using (var dbCssJs = new data._ApplicationConnectionDb())
        {
            var cssJsItems = await dbCssJs.MainCssJs
                .AsNoTracking()
                .Where(x => x.IsActive == true
                         && x.IsDeleted.IsDeletedStatu != true)
                .OrderBy(x => x.GetDateTime)
                .ToListAsync();

            activeCssList = cssJsItems
                .Where(x => x.IsCssOrJs == true && !string.IsNullOrWhiteSpace(x.UserCodes))
                .ToList();

            activeJsList = cssJsItems
                .Where(x => x.IsCssOrJs == false && !string.IsNullOrWhiteSpace(x.UserCodes))
                .ToList();
        }
    }
    catch { }
}
<!doctype html>
<html lang="@langLocaleMap.GetValueOrDefault(lang, "en-US")" dir="@(lang == "hi" ? "ltr" : "ltr")">

<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">

    @* ═══════════════════════════════════════════════════════════════
       SEO: TEMEL META ETİKETLERİ
       ═══════════════════════════════════════════════════════════════ *@
    <title>@pageTitle | efavori.com</title>
    <meta name="description" content="@pageDescription">
    <meta name="keywords" content="@pageKeywords">
    <meta name="author" content="efavori.com">
    <meta name="robots" content="@robotsContent">
    <meta name="publisher" content="efavori.com">

    @* ═══════════════════════════════════════════════════════════════
       SEO: CANONICAL URL — Duplicate Content Engelleyici
       Query string'ler (filtreler, sayfalama) temizlenerek
       yalnızca temiz path verilir.
       ═══════════════════════════════════════════════════════════════ *@
    <link rel="canonical" href="@canonicalUrl" />

    @* ═══════════════════════════════════════════════════════════════
       SEO: HREFLANG — 10 Dil + x-default
       Her sayfanın tüm dil varyasyonları arama motorlarına bildirilir.
       URL pattern: /{lang}/... şeklinde varsayılmıştır.
       ═══════════════════════════════════════════════════════════════ *@
    @{
        // Mevcut path'ten dil segmentini çıkar, temiz path elde et
        var pathSegments = canonicalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        string langFreePath;
        if (pathSegments.Length > 0 && supportedCultures.Contains(pathSegments[0].ToLower()))
        {
            langFreePath = "/" + string.Join("/", pathSegments.Skip(1));
        }
        else
        {
            langFreePath = canonicalPath;
        }
        if (string.IsNullOrEmpty(langFreePath)) langFreePath = "";
    }
    @foreach (var culture in supportedCultures)
    {
        var hreflangLocale = langLocaleMap.GetValueOrDefault(culture, $"{culture}-{culture.ToUpper()}");
        var hreflangUrl = $"{baseUrl}/{culture}{langFreePath}";
        <link rel="alternate" hreflang="@hreflangLocale" href="@hreflangUrl" />
    }
    <link rel="alternate" hreflang="x-default" href="@baseUrl/en@(langFreePath)" />

    @* ═══════════════════════════════════════════════════════════════
       SEO: OPEN GRAPH (Facebook, LinkedIn, WhatsApp vs.)
       ═══════════════════════════════════════════════════════════════ *@
    <meta property="og:locale" content="@TotalLang">
    @foreach (var culture in supportedCultures.Where(c => c != lang))
    {
        var altLocale = $"{culture}_{culture.ToUpper()}";
        <meta property="og:locale:alternate" content="@altLocale">
    }
    <meta property="og:type" content="@ogType">
    <meta property="og:title" content="@pageTitle | efavori.com">
    <meta property="og:description" content="@pageDescription">
    <meta property="og:url" content="@canonicalUrl">
    <meta property="og:site_name" content="efavori.com">
    <meta property="og:image" content="@ogImage">
    <meta property="og:image:width" content="@ogImageWidth">
    <meta property="og:image:height" content="@ogImageHeight">
    <meta property="og:image:alt" content="@pageTitle">

    @* ═══════════════════════════════════════════════════════════════
       SEO: TWITTER CARD
       ═══════════════════════════════════════════════════════════════ *@
    <meta name="twitter:card" content="summary_large_image">
    <meta name="twitter:site" content="@@efavori">
    <meta name="twitter:title" content="@pageTitle | efavori.com">
    <meta name="twitter:description" content="@pageDescription">
    <meta name="twitter:image" content="@ogImage">
    <meta name="twitter:image:alt" content="@pageTitle">

    @* ═══════════════════════════════════════════════════════════════
       PERFORMANCE: DNS PREFETCH + PRECONNECT
       ═══════════════════════════════════════════════════════════════ *@
    <link rel="preconnect" href="https://fonts.googleapis.com" crossorigin>
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link rel="dns-prefetch" href="https://www.google-analytics.com">
    <link rel="dns-prefetch" href="https://www.googletagmanager.com">

    @* ═══════════════════════════════════════════════════════════════
       PERFORMANCE: LCP PRELOAD
       Ürün sayfalarında ana görselin gecikmesiz yüklenmesi için
       ViewData["PreloadImage"] ile <head>'e preload enjekte edilir.
       ═══════════════════════════════════════════════════════════════ *@
    @if (!string.IsNullOrEmpty(preloadImage))
    {
        <link rel="preload" as="image" href="@preloadImage" fetchpriority="high" />
    }

    @* ═══════════════════════════════════════════════════════════════
       FAVICON & PWA
       ═══════════════════════════════════════════════════════════════ *@
    <link rel="icon" href="https://efavori.com/_files/main/logo/logo_100.png" type="image/png" sizes="2000x2000" />
    <link rel="icon" href="https://efavori.com/_files/main/logo/logo_100.png" type="image/png" sizes="2000x2000" />
    <link rel="apple-touch-icon" href="https://efavori.com/_files/main/logo/logo_100.png" sizes="2000x2000">
    <meta name="theme-color" content="#0000FF">
    <meta name="application-name" content="efavori">

    @* ═══════════════════════════════════════════════════════════════
       JSON-LD: YAPISAL VERİ (SSR)
       Bileşenler (ProductViewer vb.) ViewData["JsonLd"] aracılığıyla
       sunucu tarafında render edilmiş JSON-LD bloğunu bu noktaya basar.
       Bu sayede bot'lar JS çalıştırmadan yapısal veriyi okur.
       ═══════════════════════════════════════════════════════════════ *@
    @if (!string.IsNullOrEmpty(jsonLdScript))
    {
        @Html.Raw(jsonLdScript)
        <br />
    }

    @* ═══════════════════════════════════════════════════════════════
       CSS
       ═══════════════════════════════════════════════════════════════ *@

    @(await Html.RenderComponentAsync<Css>(RenderMode.Static, new { use = use != null ? use : null }))


    @* ═══════════════════════════════════════════════════════════════
       DİNAMİK CSS — MainCssJs (veritabanı: IsActive && IsCssOrJs == true)
       Temel CSS'ten SONRA basılır ki override edebilsin.
       ═══════════════════════════════════════════════════════════════ *@
    @foreach (var css in activeCssList)
    {
        @Html.Raw(css.UserCodes)
    }

    <!-- Google Search Console XML (gtag.js) -->
    <meta name="google-site-verification" content="5NNGMkgIA5vY339WHPLmpN2efO1qVgj23Bf-pJjisyg" />
    <!-- Google tag (gtag.js) -->
    <script async src="https://www.googletagmanager.com/gtag/js?id=G-3S3KS4DHSF"></script>
    <script>
            window.dataLayer = window.dataLayer || [];
            function gtag() { dataLayer.push(arguments); }
            gtag('js', new Date());

            gtag('config', 'G-3S3KS4DHSF');
    </script>

</head>
<body class="bg-theme @(use != null && !string.IsNullOrEmpty(use.BackgroundImagePath) ? use.BackgroundImagePath : "bg-theme1")">
    <div id="page-transition-overlay"></div>
    <div class="wrapper">
        @* ═══════════════════════════════════════════════════════════
           SIDEBAR + HEADER — Tüm diller aynı bileşeni kullanıyor,
           switch yerine tek çağrı ile sadeleştirildi.
           ═══════════════════════════════════════════════════════════ *@
        @(await Html.RenderComponentAsync<razor._Shared.tr.Sidebar.SideBarMenu>(RenderMode.Static, new { _use = use }))
        @(await Html.RenderComponentAsync<razor._Shared.tr.Header.HeaderMenu>(RenderMode.Static, new { _use = use }))

        <div class="page-wrapper">
            <div class="page-content" role="main">
                @RenderBody()
                @if (use != null)
                {
                    if (use?.AccountActivationMailStatu != true)
                    {
                        @(await Html.RenderComponentAsync<razor._Shared.tr.Header.AccountActivationMailStatu>(RenderMode.Static, new { use = use }))
                    }
                }
            </div>
        @* ═══════════════════════════════════════════════════════════════
           FOOTER — Blazor bileşeni olarak render edilir
           ═══════════════════════════════════════════════════════════════ *@

@switch (lang)
{
    case "tr":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "az":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "de":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "es":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "fr":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "hi":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "pt":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "ru":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    case "zh":
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;

    default:
        @(await Html.RenderComponentAsync<razor._Shared.tr.Footer.FooterArea>(RenderMode.Static))
        break;
}





        </div>
        <div class="overlay toggle-icon"></div>
        <a href="javaScript:;" class="back-to-top" aria-label="Sayfanın başına dön"><i class='bx bxs-up-arrow-alt' aria-hidden="true"></i></a>

    </div>

    @* ═══════════════════════════════════════════════════════════════
       MODALS — Tüm diller aynı bileşeni kullanıyor, sadeleştirildi
       ═══════════════════════════════════════════════════════════════ *@
    @switch (lang)
    {
        case "tr":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.LogOutTimerInfo>(RenderMode.Static))
            ;
            break;
        case "az":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "de":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "es":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "fr":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "hi":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "pt":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "ru":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        case "zh":
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
        default:
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.Login>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.NewUserRegister>(RenderMode.Static))
            @(await Html.RenderComponentAsync<razor._Shared.tr.Modals.Account.ResetPassword>(RenderMode.Static))
            ;
            break;
    }

    @(await Html.RenderComponentAsync<Js>(RenderMode.Static, new { use = use != null ? use : null }))

    @* ═══════════════════════════════════════════════════════════════
       DİNAMİK JS — MainCssJs (veritabanı: IsActive && IsCssOrJs == false)
       Temel JS bileşeninden SONRA basılır ki jQuery/Bootstrap hazır olsun.
       ═══════════════════════════════════════════════════════════════ *@

    @foreach (var js in activeJsList)
    {
        @Html.Raw(js.UserCodes)
        <br />
    }

    @(await Html.RenderComponentAsync<razor._Shared.Notification>(RenderMode.Server))
    <base href="~/" />
    <script src="~/_framework/blazor.server.js"></script>
</body>
</html>```

## Dosya: AccountPermissions.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class AccountPermissions : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult Permissions()
        {
            return View();
        }
    }
}
```
.
## Dosya: AdminSettings.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class AdminSettings : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult AllBackgroundServicesFrequencyRateIndex()
        {
            return View();
        }
    }
}
```
.
## Dosya: ArticlesCategories.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class ArticlesCategories : Controller
    {
        public IActionResult ControllerArticlesCategoriesTr()
        {
            return View();
        }
    }
}
```
.
## Dosya: Categories.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Categories : Controller
    {
        public IActionResult CategoryManagementProducts()
        {
            return View();
        }
    }
}
```
.
## Dosya: DraftPage.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class DraftPage : Controller
    {
        public IActionResult ThisPageIsBeingPrepared()
        {
            return View();
        }
        public IActionResult ThisPageIsBeingPreparedProfile()
        {
            return View();
        }
        public IActionResult ThisPageIsBeingPreparedProfileMedia()
        {
            return View();
        }
        public IActionResult ArtificialIntelligence()
        {
            return View();
        }
    }
}
```
.
## Dosya: Home.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```
.
## Dosya: LogManagement.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class LogManagement : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult List()
        {
            return View();
        }
    }
}
```
.
## Dosya: MainCssJs.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class MainCssJs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```
.
## Dosya: MyMemberBusinesses.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class MyMemberBusinesses : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
```
.
## Dosya: PersonnelManagement.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class PersonnelManagement : Controller
    {
        public IActionResult AllUsers()
        {
            return View();
        }
    }
}
```
.
## Dosya: Product.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class Product : Controller
    {
        public IActionResult AddProduct()
        {
            return View();
        }
        public IActionResult BulkWordPressProductImport()
        {
            return View();
        } 
        public IActionResult ListProduct()
        {
            return View();
        } 
        public IActionResult ProductHistoryList()
        {
            return View();
        } 
    }
}
```
.
## Dosya: ProductAttributes.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class ProductAttributes : Controller
    {
        public IActionResult Attributes()
        {
            return View();
        }
    }
}
```
.
## Dosya: Store.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class Store : Controller
    {
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.StoreId = id;
            using (var db = new _ApplicationConnectionDb())
            {
                var store = await db.Store
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
                ViewBag.StoreData = store;
            }

            return View();
        }
    }
}
```
.
## Dosya: StoreIntegration.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class StoreIntegration : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```
.
## Dosya: SystemEmailHistory.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class SystemEmailHistory : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult List()
        {
            return View();
        }
    }
}
```
.
## Dosya: TableCleaner.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class TableCleaner : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```
.
## Dosya: Warehouse.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Warehouse : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
```
.
## Dosya: Permissions.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\AccountPermissions\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AccountPermissions.Permissions>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
        }
    }
}```
.
## Dosya: AllBackgroundServicesFrequencyRateIndex.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\AdminSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.AdminSettings.AllBackgroundServicesTasks>(RenderMode.Server, new { use = (use != null ? use : null) }))

                ;
                break;
        }
    }
}```
.
## Dosya: ControllerArticlesCategoriesTr.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\ArticlesCategories\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ArticlesCategories.RazorArticlesCategories>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: CategoryManagementProducts.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Categories\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Categories.CategoriesProductPage>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: ArtificialIntelligence.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\DraftPage\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ArtificialIntelligence>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: ThisPageIsBeingPrepared.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\DraftPage\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPrepared>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: ThisPageIsBeingPreparedProfile.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\DraftPage\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfile>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: ThisPageIsBeingPreparedProfileMedia.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\DraftPage\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.ThisPageIsBeingPreparedProfileMedia>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Index.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Home\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@{
    ViewData["Title"] = "Home Page";

    // YÖNTEM: Dili doğrudan URL (Route) verisinden çekiyoruz
    // Eğer URL'de kültür yoksa Program.cs'deki varsayılanı (en) kullanır
    var currentLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower() ?? "en";


    List<Country> _Country;
    using (var db = new data._ApplicationConnectionDb())
    {
        _Country = db.Country.ToList();
    }
}

<div class="text-center">
    @* Hata Giderildi: @ sembolünden sonra boşluk silindi *@
    <h1 class="display-4">
        @if (currentLang == "tr")
        {
            @:Hoş Geldiniz
        }
        else if (currentLang == "az")
        {
            @:Xoş gəlmisiniz
        }
        else if (currentLang == "de")
        {
            @:Willkommen
        }
        else if (currentLang == "es")
        {
            @:Bienvenido
        }
        else if (currentLang == "fr")
        {
            @:Bienvenue
        }
        else if (currentLang == "hi")
        {
            @:स्वागत है
        }
        else if (currentLang == "pt")
        {
            @:Bem-vindo
        }
        else if (currentLang == "ru")
        {
            @:Добро пожаловать
        }
        else if (currentLang == "zh")
        {
            @:欢迎
        }
        else
        {
            @:Welcome
        }
    </h1>

    <hr />

    @if (!User.Identity.IsAuthenticated)
    {
        <div class="alert alert-info">
            @if (currentLang == "tr")
            {
                <p>Özel içerikleri görmek için lütfen <a asp-controller="Security" asp-action="Login">giriş yapın</a>.</p>
            }
            else if (currentLang == "az")
            {
                <p>Xüsusi məzmunu görmək üçün zəhmət olmasa <a asp-controller="Security" asp-action="Login">daxil olun</a>.</p>
            }
            else if (currentLang == "de")
            {
                <p>Bitte <a asp-controller="Security" asp-action="Login">einloggen</a>, um private Inhalte zu sehen.</p>
            }
            else if (currentLang == "es")
            {
                <p>Por favor <a asp-controller="Security" asp-action="Login">inicie sesión</a> para ver contenido privado.</p>
            }
            else if (currentLang == "fr")
            {
                <p>Veuillez vous <a asp-controller="Security" asp-action="Login">connecter</a> pour voir le contenu privé.</p>
            }
            else if (currentLang == "hi")
            {
                <p>निजी सामग्री देखने के लिए कृपया <a asp-controller="Security" asp-action="Login">लॉगिन करें</a>।</p>
            }
            else if (currentLang == "pt")
            {
                <p>Por favor, faça <a asp-controller="Security" asp-action="Login">login</a> para ver o conteúdo privado.</p>
            }
            else if (currentLang == "ru")
            {
                <p>Пожалуйста, <a asp-controller="Security" asp-action="Login">войдите</a>, чтобы увидеть личный контент.</p>
            }
            else if (currentLang == "zh")
            {
                <p>请<a asp-controller="Security" asp-action="Login">登录</a>查看私人内容。</p>
            }
            else // Varsayılan: en
            {
                <p>Please <a asp-controller="Security" asp-action="Login">login</a> to see private content.</p>
            }
        </div>
    }
    else
    {
        <div class="alert alert-success">
            @if (currentLang == "tr")
            {
                <span>Tekrar hoş geldin, @User.Identity.Name!</span>
            }
            else if (currentLang == "az")
            {
                <span>Yenidən xoş gəldiniz, @User.Identity.Name!</span>
            }
            else if (currentLang == "de")
            {
                <span>Willkommen zurück, @User.Identity.Name!</span>
            }
            else if (currentLang == "es")
            {
                <span>Bienvenido de nuevo, @User.Identity.Name!</span>
            }
            else if (currentLang == "fr")
            {
                <span>Bon retour, @User.Identity.Name!</span>
            }
            else if (currentLang == "hi")
            {
                <span>वापसी पर स्वागत है, @User.Identity.Name!</span>
            }
            else if (currentLang == "pt")
            {
                <span>Bem-vindo de volta, @User.Identity.Name!</span>
            }
            else if (currentLang == "ru")
            {
                <span>С возвращением, @User.Identity.Name!</span>
            }
            else if (currentLang == "zh")
            {
                <span>欢迎回来, @User.Identity.Name!</span>
            }
            else // Varsayılan: en
            {
                <span>Welcome back, @User.Identity.Name!</span>
            }
        </div>
    }

    @* Veri Listeleme *@
    <div class="row justify-content-center mt-4">
        @foreach (var item in _Country)
        {
            <div class="col-md-4 mb-2">
                <div class="card p-2 shadow-sm">
                    <strong>@item.name</strong>
                </div>
            </div>
        }
    </div>

    <p class="mt-4">
        Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.
    </p>
</div>```
.
## Dosya: List.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\LogManagement\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.LogManagement.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Index.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\MainCssJs\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MainCssJs.RazorMainCssJs>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: List.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\MyMemberBusinesses\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.MyMemberBusinesses.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: AllUsers.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\PersonnelManagement\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.PersonnelManagement.AllUsers>(RenderMode.Server))

                ;
                break;
        }
    }
}```
.
## Dosya: AddProduct.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Product\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: BulkWordPressProductImport.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Product\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.BulkWordPressProductImport>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: ListProduct.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Product\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ListProduct>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: ProductHistoryList.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Product\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Product.ProductHistoryList>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Attributes.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\ProductAttributes\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.ProductAttributes.Attributes>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Add.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Store\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Add>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Edit.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Store\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;
    Store? store = ViewBag.StoreData;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.Edit>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null), SelectedStore = (store != null ? store : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: List.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Store\
```
﻿@using data
@using data.Owned;
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Store.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Index.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\StoreIntegration\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.StoreIntegration.Controller>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: List.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\SystemEmailHistory\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.SystemEmailHistory.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Index.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\TableCleaner\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.TableCleaner.Index>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: List.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Admin\Views\Warehouse\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Admin.Warehouse.List>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: Search.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public
{
    [Area("Public")]
    [Route("")]
    [Route("{TaskFrameworkId:guid}")]
    [Route("{Value?}/{TaskFrameworkId?}")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    [Route("{culture}/Public/[controller]/[action]/{Value}/{TaskFrameworkId}")]
    [Route("{culture}/Public/[controller]/[action]/{TaskId:guid}")]
    public class Search : Controller
    {
        public IActionResult Wanted(string SearchWantedText)
        {
            ViewBag.SearchWantedText = SearchWantedText;
            return View();
        }
    }
}
```
.
## Dosya: ChatMessage.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class ChatMessage : Controller
    {
        [Area("Public")]
        [Route("{culture}/Public/[controller]/[action]")]
        public IActionResult LiveChatMessage()
        {
            return View();
        }
    }
}
```
.
## Dosya: FixedPages.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    // ═══════════════════════════════════════════════════════════════════════
    //  FixedPages — Sabit / kurumsal içerik sayfaları (efavori.com)
    //  ─────────────────────────────────────────────────────────────────────
    //  FooterArea.razor'daki statik linkler tek noktadan bu controller üzerinden
    //  karşılanır. Yalnızca kullanıcıya göre değişmeyen; DB listesi, oturum veya
    //  form-POST gerektirmeyen bilgilendirme / pazarlama / yasal sayfalar burada.
    //
    //  Her aksiyonun üstündeki yorum, footer'daki bağlantı metnini gösterir.
    // ═══════════════════════════════════════════════════════════════════════
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class FixedPages : Controller
    {

        // ── Kurumsal ────────────────────────────────────────────────────────

        // Hakkımızda
        public IActionResult About()
        {
            return View();
        }

        // Kariyer
        public IActionResult Careers()
        {
            return View();
        }

        // İletişim
        public IActionResult Contact()
        {
            return View();
        }

        // Basın Odası
        public IActionResult Press()
        {
            return View();
        }

        // Sürdürülebilirlik
        public IActionResult Sustainability()
        {
            return View();
        }

        // Yatırımcı İlişkileri
        public IActionResult InvestorRelations()
        {
            return View();
        }

        // ── Yardım ve Destek ────────────────────────────────────────────────

        // Yardım Merkezi
        public IActionResult HelpCenter()
        {
            return View();
        }

        // İade ve Değişim
        public IActionResult Returns()
        {
            return View();
        }

        // Kargo ve Teslimat
        public IActionResult Shipping()
        {
            return View();
        }

        // Sıkça Sorulan Sorular
        public IActionResult Faq()
        {
            return View();
        }

        // Talep ve Şikâyetler
        public IActionResult Complaints()
        {
            return View();
        }

        // ── efavori'de Satış Yap ────────────────────────────────────────────

        // Mağaza Aç
        public IActionResult Sell()
        {
            return View();
        }

        // Satıcı Akademisi
        public IActionResult SellerAcademy()
        {
            return View();
        }

        // Komisyon Oranları
        public IActionResult SellerCommissions()
        {
            return View();
        }

        // Kurumsal Satış
        public IActionResult Business()
        {
            return View();
        }

        // İş Ortaklığı Programı
        public IActionResult Affiliate()
        {
            return View();
        }

        // Reklam Ver
        public IActionResult Advertising()
        {
            return View();
        }

        // ── Ödeme ve Avantajlar ─────────────────────────────────────────────

        // Ödeme Seçenekleri
        public IActionResult PaymentMethods()
        {
            return View();
        }

        // Taksit Seçenekleri
        public IActionResult PaymentInstallments()
        {
            return View();
        }

        // Hediye Kartı
        public IActionResult GiftCard()
        {
            return View();
        }

        // Alışveriş Kredisi
        public IActionResult PaymentCredit()
        {
            return View();
        }

        // ── Kargo ve Lojistik ───────────────────────────────────────────────

        // Teslimat Süreleri
        public IActionResult DeliveryTimes()
        {
            return View();
        }

        // Kargo Ücretleri
        public IActionResult ShippingFees()
        {
            return View();
        }

        // Aynı Gün Teslimat
        public IActionResult SameDayDelivery()
        {
            return View();
        }

        // Teslimat Noktaları
        public IActionResult PickupPoints()
        {
            return View();
        }

        // Yurt Dışı Gönderim
        public IActionResult InternationalShipping()
        {
            return View();
        }

        // Anlaşmalı Kargo Firmaları
        public IActionResult Carriers()
        {
            return View();
        }

        // ── Güvenlik ve Yasal (Footer sütunu) ───────────────────────────────

        // Güvenli Alışveriş Rehberi
        public IActionResult SafeShopping()
        {
            return View();
        }

        // Alıcı Koruma Programı
        public IActionResult BuyerProtection()
        {
            return View();
        }

        // Gizlilik Merkezi
        public IActionResult PrivacyCenter()
        {
            return View();
        }

        // Fikri Mülkiyet Hakları
        public IActionResult IpProtection()
        {
            return View();
        }

        // Yasaklı ve Kısıtlı Ürünler
        public IActionResult ProhibitedItems()
        {
            return View();
        }

        // Şeffaflık Raporu
        public IActionResult Transparency()
        {
            return View();
        }

        // ETBİS Kaydı
        public IActionResult Etbis()
        {
            return View();
        }

        // ── Yasal (Alt bar) ─────────────────────────────────────────────────

        // Kullanım Koşulları
        public IActionResult Terms()
        {
            return View();
        }

        // Gizlilik Politikası
        public IActionResult Privacy()
        {
            return View();
        }

        // Çerez Politikası
        public IActionResult Cookies()
        {
            return View();
        }

        // KVKK Aydınlatma Metni
        public IActionResult Kvkk()
        {
            return View();
        }

        // Mesafeli Satış Sözleşmesi
        public IActionResult DistanceSales()
        {
            return View();
        }

        // Üyelik Sözleşmesi
        public IActionResult Membership()
        {
            return View();
        }

        // Çerez Tercihleri
        public IActionResult CookieSettings()
        {
            return View();
        }

        // Erişilebilirlik
        public IActionResult Accessibility()
        {
            return View();
        }
    }
}```
.
## Dosya: FriendShip.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class FriendShip : Controller
    {
        public IActionResult Requests()
        {
            return View();
        }
    }
}
```
.
## Dosya: Home.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "The Global Marketplace for Everything You Need";
            ViewData["Description"] = "Shop millions of products from thousands of sellers on efavori.com. Discover the best deals on electronics, fashion, home goods, and more with secure global shipping.";
            ViewData["Keywords"] = "efavori, online shopping, global marketplace, multi-vendor platform, best deals, e-commerce, buy online";

            return View();
        }
        public IActionResult GetGitHubCommits()
        {
            return View();
        }
    }
}
```
.
## Dosya: MainPrompter.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class MainPrompter : Controller
    {
        public IActionResult MainPrompterPageCSHTML()
        {
            return View();
        }
    }
}
```
.
## Dosya: MediaManagement.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class MediaManagement : Controller
    {
        [Area("Public")]
        [Route("{culture}/Public/[controller]/[action]")]
        public IActionResult MediaGallery()
        {
            return View();
        }
    }
}
```
.
## Dosya: TaskBoard.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    [Route("{culture}/Public/[controller]/[action]/{Value}/{TaskFrameworkId}")]
    [Route("{culture}/Public/[controller]/[action]/{TaskId:guid}")]
    public class TaskBoard : Controller
    {
        public IActionResult CentralSystemTaskBoard(string? Value, Guid? TaskFrameworkId)
        {
            ViewBag.Value = Value;
            ViewBag.TaskFrameworkId = TaskFrameworkId;
            return View();
        }
        public IActionResult PrintTask(Guid? TaskId)
        {
            ViewBag.TaskId = TaskId;
            return View();
        }
    }
}
```
.
## Dosya: UserProfileSettings.cs
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Controllers\
```
﻿using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class UserProfileSettings : Controller
    {

        public IActionResult BasicInfo() => View();
        public IActionResult Social() => View();
        public IActionResult Privacy() => View();
        public IActionResult Customize() => View();
        public IActionResult Security() => View();


        public IActionResult UserAddressMethod() => View();

        public IActionResult UserPaymentMethod() => View();

    }
}
```
.
## Dosya: LiveChatMessage.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\ChatMessage\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.ChatMessage.LiveChatMessage>(RenderMode.Server, new { use = use }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.ChatMessage.LiveChatMessage>(RenderMode.Server, new { use = use }))
}```
.
## Dosya: Requests.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\FriendShip\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.FriendShip.Requests>(RenderMode.Server, new { use = use}))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.FriendShip.Requests>(RenderMode.Server, new { use = use}))
}```
.
## Dosya: GetGitHubCommits.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\Home\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.GitHubCommitsList.GitHubCommitsList>(RenderMode.Server))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.GitHubCommitsList.GitHubCommitsList>(RenderMode.Server))
}```
.
## Dosya: Index.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\Home\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public._Lists.ProductListing>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: MainPrompterPageCSHTML.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\MainPrompter\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;
@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.MainPrompter.Teleprompter>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
## Dosya: MediaGallery.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\MediaManagement\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{

}
else
{

    @(await Html.RenderComponentAsync<razor._Shared.tr.Media.Upload>(
    RenderMode.ServerPrerendered,
    new { use = use }
    ))

    @(await Html.RenderComponentAsync<razor._Shared.tr.Media.List>(
    RenderMode.ServerPrerendered,
    new { use = use, IsMultiSelect = (bool?)null }
    ))
}```
.
## Dosya: CentralSystemTaskBoard.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\TaskBoard\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.TaskBoard.CentralSystemTaskBoard>(RenderMode.Server, new { use = use, TaskCategoriesValue = ViewBag.Value, TaskFrameworkId = ViewBag.TaskFrameworkId }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.TaskBoard.CentralSystemTaskBoard>(RenderMode.Server, new { use = use, TaskCategoriesValue = ViewBag.Value, TaskFrameworkId = ViewBag.TaskFrameworkId }))
}```
.
## Dosya: PrintTask.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\TaskBoard\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };
    Guid? taskId = ViewBag.TaskId as Guid?;  // ✅ Cast et


    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.TaskBoard.PrintTask>(
    RenderMode.Static,
    new { TaskId = taskId }  // ✅ Cast edilmiş değeri geç
))```
.
## Dosya: BasicInfo.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\UserProfileSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.BasicInfo>(RenderMode.Server, new { use = use }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.BasicInfo>(RenderMode.Server, new { use = use }))
}```
.
## Dosya: Customize.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\UserProfileSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Customize>(RenderMode.Server, new { use = use }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Customize>(RenderMode.Server, new { use = use }))
}```
.
## Dosya: Privacy.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\UserProfileSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Privacy>(RenderMode.Server, new { use = use }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Privacy>(RenderMode.Server, new { use = use }))
}```
.
## Dosya: Security.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\UserProfileSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Security>(RenderMode.Server, new { use = use }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Security>(RenderMode.Server, new { use = use }))
}```
.
## Dosya: Social.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\UserProfileSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}
@if (!User.Identity.IsAuthenticated)
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Social>(RenderMode.Server, new { use = use }))
}
else
{
    @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.Social>(RenderMode.Server, new { use = use }))
}```
.
## Dosya: UserAddressMethod.cshtml
Konum: C:\Users\husey\source\repos\huseyindemirdoger1992\efavori\web\Areas\Public\Views\UserProfileSettings\
```
﻿@using data
@using data._Carts;
@using data._Categories;
@using data._Follows;
@using data._Galleries;
@using data._Helper;
@using data._Locations;
@using data._Products;
@using data._Shares;
@using data._Store;
@using data._Systems;
@using data._Tasks;
@using data._Users;

@using System.Globalization
@using System.Security.Claims
@using Microsoft.AspNetCore.Http
@using Microsoft.EntityFrameworkCore
@using System.Linq

@{
    Layout = "~/Areas/_Layout.cshtml";

    // --- 1. Dil belirleme ---
    string currentLang = "en";
    var supportedCultures = new[] { "tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh" };

    // Oturum kontrolü
    var userEmail = User.FindFirstValue(ClaimTypes.Email);
    Users? use = null;

    if (!string.IsNullOrEmpty(userEmail))
    {
        // Oturum açık: Veritabanından dil al
        using (var db = new data._ApplicationConnectionDb())
        {
            use = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == userEmail);

            if (use != null && !string.IsNullOrEmpty(use.Language) && supportedCultures.Contains(use.Language.ToLower()))
            {
                currentLang = use.Language.ToLower();
            }
        }
    }
    else
    {
        // Oturum kapalı: Önce URL, sonra cookie, en son varsayılan
        var routeLang = ViewContext.RouteData.Values["culture"]?.ToString()?.ToLower();
        if (!string.IsNullOrEmpty(routeLang) && supportedCultures.Contains(routeLang))
        {
            currentLang = routeLang;
        }
        else
        {
            // Cookie'den oku
            var httpContext = Context;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".Efavori.Culture", out var cookieValue) && !string.IsNullOrEmpty(cookieValue))
            {
                // ASP.NET Core formatı: c=tr|uic=tr
                var parts = cookieValue.Split('|');
                var cPart = parts.FirstOrDefault(p => p.StartsWith("c="));
                if (cPart != null)
                {
                    var langCode = cPart.Substring(2).ToLower();
                    if (supportedCultures.Contains(langCode))
                        currentLang = langCode;
                }
            }
        }
    }
}

@if (!User.Identity.IsAuthenticated)
{
    @* Oturum Açmamış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}
else
{
    @* Oturum Açmış*@
    {
        switch (currentLang)
        {
            case "tr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "az":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "de":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "es":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "fr":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "hi":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "pt":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "ru":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            case "zh":
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
            default:
                @(await Html.RenderComponentAsync<razor._Shared.tr.Pages.Public.UserProfileSettings.UserAddress>(
                                                RenderMode.Server,
                                                new { use = (use != null ? use : null) }))
                ;
                break;
        }
    }
}```
.
