using api.tr;
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
try
{
    var builder = WebApplication.CreateBuilder(args);

    // --- 1. Altyapı ve Kodlama Ayarları ---
    ConfigureInfrastructure(builder);

    // --- 2. Localization (Çoklu Dil) Yapılandırması ---
    ConfigureLocalization(builder.Services);

    // --- 3. Veritabanı ve Veri Katmanı ---
    ConfigurePersistence(builder);

    // --- 4. Güvenlik ve Kimlik Doğrulama ---
    ConfigureSecurity(builder.Services, builder.Environment);

    // --- 5. Web Sunucu ve Limitler ---
    ConfigureServerLimits(builder.Services);

    builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

    // UserInfos zaten ConfigureInfrastructure içinde eklendi - tekrar eklemeyin
    builder.Services.AddScoped<TakeLogs>();
    builder.Services.AddScoped<EmailSender>();

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

        // UserInfos'u sadece bir kez ekle
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

            // Provider sıralaması kritik: Route -> Cookie -> Accept-Language Header
            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new RouteDataRequestCultureProvider
            {
                RouteDataStringKey = "culture",
                Options = options
            });
            options.RequestCultureProviders.Add(new CookieRequestCultureProvider
            {
                CookieName = ".Efavori.Culture",
                Options = options
            });
            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider
            {
                Options = options
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

    void ConfigureSecurity(IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddHttpClient();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = ".Efavori.Auth.Identity";
                options.Cookie.Path = "/";
                options.LoginPath = "/en";  // Default culture
                options.LogoutPath = "/en/Account/Logout";
                options.AccessDeniedPath = "/en/Account/Logout";
                options.Cookie.HttpOnly = true;

                // Development'ta SameAsRequest, Production'da Always
                options.Cookie.SecurePolicy = env.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;

                options.Cookie.SameSite = SameSiteMode.Lax;

                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.SlidingExpiration = true;
                options.Cookie.MaxAge = options.ExpireTimeSpan;

                // Culture-aware yönlendirmeler
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        var culture = context.HttpContext.Request.RouteValues["culture"]?.ToString() ?? "en";
                        context.Response.Redirect($"/{culture}");
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        var culture = context.HttpContext.Request.RouteValues["culture"]?.ToString() ?? "en";
                        context.Response.Redirect($"/{culture}/Account/Logout");
                        return Task.CompletedTask;
                    },
                    OnRedirectToLogout = context =>
                    {
                        var culture = context.HttpContext.Request.RouteValues["culture"]?.ToString() ?? "en";
                        context.Response.Redirect($"/{culture}");
                        return Task.CompletedTask;
                    }
                };
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
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // --- KRİTİK SIRALAMA: Localization, Auth'dan ÖNCE gelmeli ---
        var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOptions.Value);

        app.UseAuthentication();
        app.UseAuthorization();

        // Güvenlik Headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            await next();
        });
    }

    void ConfigureEndpoints(WebApplication app)
    {
        // Area route (Admin, Customer vb.)
        app.MapControllerRoute(
            name: "areaRoute",
            pattern: "{culture}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

        // Default route
        app.MapControllerRoute(
            name: "default",
            pattern: "{culture}/{controller=Home}/{action=Index}/{id?}");

        // Account route
        app.MapControllerRoute(
            name: "account",
            pattern: "{culture}/Account/{action=Login}",
            defaults: new { controller = "Account" });

        // Root redirect
        app.MapGet("/", context =>
        {
            var culture = GetPreferredCulture(context);
            context.Response.Redirect($"/{culture}/Customer/Home/Index");
            return Task.CompletedTask;
        });

        // Culture root redirect
        app.MapGet("/{culture}", (string culture, HttpContext context) =>
        {
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

        // 1. Cookie'den culture al
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

        // 2. Browser Accept-Language header'dan al
        var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            var browserLang = acceptLanguage.Split(',').FirstOrDefault()?.Split('-').FirstOrDefault()?.ToLower();
            if (!string.IsNullOrEmpty(browserLang) && supportedCultures.Contains(browserLang))
            {
                return browserLang;
            }
        }

        return defaultCulture;
    }
    #endregion
}
catch (Exception ex)
{
    // CRITICAL: Startup hatalarını loglayın
    Console.WriteLine($"FATAL ERROR during startup: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}
