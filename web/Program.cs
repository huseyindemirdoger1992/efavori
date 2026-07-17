using api;
using api.tr;
using api.tr.AI;
using data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using razor._Shared;
using razor._Shared.tr.Media;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;


// ============================================================================
// STARTUP ERROR HANDLING - Production Resilient
// ============================================================================
try
{
    var builder = WebApplication.CreateBuilder(args);

    // ============================================================================
    // CRITICAL: Validate Configuration Before Startup
    // ============================================================================
    ValidateConfiguration(builder.Configuration);

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
    builder.Services.AddScoped<TakeLogs>();
    builder.Services.AddScoped<EmailSender>();
    builder.Services.AddScoped<api.tr.Media>();
    builder.Services.AddScoped<api.tr.UserInfos>();
    builder.Services.AddScoped<Media>();
    builder.Services.AddScoped<FileUploadService>();


    // Api Services
    builder.Services.AddHttpClient<AIProductKnowledgeGenerator>();
    builder.Services.AddHttpClient<AIProductTranslationGeneratorTurkishToEnglish>();
    builder.Services.AddHostedService<AllBackgroundServices>();

    var app = builder.Build();

    // ============================================================================
    // CRITICAL: Validate Database Connection at Startup
    // ============================================================================
    await ValidateDatabaseConnection(app);

    // --- 6. Middleware Pipeline (Sıralama Düzenlendi) ---
    ConfigureMiddlewarePipeline(app);

    // --- 7. SEO Uyumlu Endpoint Tanımları ---
    ConfigureEndpoints(app);
    app.MapControllers();

    Console.WriteLine("✓ Application started successfully");
    app.Run();

    #region Configuration Methods

    void ValidateConfiguration(IConfiguration config)
    {
        Console.WriteLine("=== Validating Configuration ===");

        var connString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
        {
            throw new InvalidOperationException(
                "FATAL ERROR: Connection string 'DefaultConnection' not found in appsettings.json. " +
                "Please ensure appsettings.json exists and contains valid connection string.");
        }

        // Validate connection string format
        if (!connString.Contains("Server=") || !connString.Contains("Database="))
        {
            throw new InvalidOperationException(
                "FATAL ERROR: Invalid connection string format. " +
                $"Current: {connString.Substring(0, Math.Min(50, connString.Length))}...");
        }

        Console.WriteLine("✓ Configuration validated");
    }

    async Task ValidateDatabaseConnection(WebApplication app)
    {
        Console.WriteLine("=== Validating Database Connection ===");

        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<_ApplicationConnectionDb>();

            // Test connection with timeout
            var canConnect = await dbContext.Database.CanConnectAsync();

            if (!canConnect)
            {
                Console.WriteLine("⚠ WARNING: Cannot connect to database. Application will start but database operations may fail.");
                Console.WriteLine("⚠ Please check:");
                Console.WriteLine("  1. SQL Server is running and accessible");
                Console.WriteLine("  2. Firewall allows connection to port 1433");
                Console.WriteLine("  3. Connection string credentials are correct");

                // Don't throw in production - let app start and show error page instead
                if (app.Environment.IsDevelopment())
                {
                    throw new InvalidOperationException("Database connection failed in Development mode");
                }
            }
            else
            {
                Console.WriteLine("✓ Database connection successful");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Database validation error: {ex.Message}");

            if (app.Environment.IsDevelopment())
            {
                throw;
            }

            // In production, log but continue - error will show on first DB access
            Console.WriteLine("⚠ Application will start but database errors are expected");
        }
    }

    void ConfigureInfrastructure(WebApplicationBuilder b)
    {
        // Karakter kodlama ve temel servisler
        b.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
        b.Services.AddHttpContextAccessor();

        // GlobalStateHub'ın ihtiyaç duyduğu cache yapısı
        b.Services.AddSingleton<GlobalStateHub>();
        b.Services.AddDistributedMemoryCache();
        b.Services.AddScoped<UserInfos>();

        // MVC ve Localization ayarları
        var mvcBuilder = b.Services.AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        if (b.Environment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        // ✅ Doğru Blazor Yapılandırması
        b.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        b.Services.AddServerSideBlazor(options =>
        {
            // Sunucu için kritik ayarlar:
            options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromDays(7);

            // E-ticaret gibi yoğun veri trafiğinde mesaj boyutunu sınırla (opsiyonel)
            options.DetailedErrors = b.Environment.IsDevelopment();
            options.DisconnectedCircuitMaxRetained = 999999; // Aynı anda kaç kopuk devre RAM'de beklesin?
        });
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

        Console.WriteLine($"Database Server: {ExtractServerFromConnectionString(connectionString)}");

        b.Services.AddDbContextPool<_ApplicationConnectionDb>(options =>
            ConfigureDbOptions(options, connectionString, b.Environment),
            poolSize: 128); // Optimize pool size

        b.Services.AddDbContextFactory<_ApplicationConnectionDb>(
            options => ConfigureDbOptions(options, connectionString, b.Environment),
            ServiceLifetime.Singleton);
    }

    void ConfigureDbOptions(DbContextOptionsBuilder options, string connectionStr, IWebHostEnvironment env)
    {
        options.UseSqlServer(connectionStr, sqlOptions =>
        {
            sqlOptions.CommandTimeout(30);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);

            // Improve connection resilience
            sqlOptions.MinBatchSize(1);
            sqlOptions.MaxBatchSize(100);
        });

        // Only enable detailed logging in Development
        if (env.IsDevelopment())
        {
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        }
        else
        {
            // Production optimizations
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
                options.LoginPath = "/en";
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
        const long maxRequestLimit = 134217728; // 128MB
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = maxRequestLimit;
            options.AllowSynchronousIO = false; // Security best practice
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxRequestLimit;
            options.AddServerHeader = false; // Security: hide server header
        });
    }

    void ConfigureMiddlewarePipeline(WebApplication app)
    {
        // Production-specific error handling
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
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "_files")),
            RequestPath = "/_files"
        });
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
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            await next();
        });
    }

    void ConfigureEndpoints(WebApplication app)
    {
        // Area route (Admin, Public vb.)
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

        app.MapControllerRoute(
        name: "xml",
        pattern: "/_files/xml/{controller=Home}/{action=Index}/{id?}");


        // Root redirect
        app.MapGet("/", context =>
        {
            var culture = GetPreferredCulture(context);
            context.Response.Redirect($"/{culture}/Public/Home/Index");
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
            context.Response.Redirect($"/{culture}/Public/Home/Index");
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

    string ExtractServerFromConnectionString(string connectionString)
    {
        var serverPart = connectionString.Split(';')
            .FirstOrDefault(s => s.Trim().StartsWith("Server=", StringComparison.OrdinalIgnoreCase));
        return serverPart?.Replace("Server=", "").Trim() ?? "Unknown";
    }

    #endregion
}
catch (Exception ex)
{
    // ============================================================================
    // CRITICAL STARTUP ERROR LOGGING
    // ============================================================================
    var errorMessage = $@"
================================================================================
FATAL ERROR DURING APPLICATION STARTUP
================================================================================
Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
Error: {ex.Message}
Type: {ex.GetType().Name}

Stack Trace:
{ex.StackTrace}

Inner Exception: {ex.InnerException?.Message ?? "None"}
================================================================================

TROUBLESHOOTING STEPS:
1. Check appsettings.json exists and has valid connection string
2. Verify SQL Server is accessible: mssql02.trwww.com:1433
3. Check ASP.NET Core Hosting Bundle is installed
4. Review logs folder for detailed error logs
5. Ensure all required DLLs are present in application folder

For detailed diagnosis, use web.debug.config temporarily.
================================================================================
";

    Console.WriteLine(errorMessage);

    // Try to write to log file
    try
    {
        var logPath = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logPath);
        var logFile = Path.Combine(logPath, $"startup-error-{DateTime.UtcNow:yyyyMMdd-HHmmss}.log");
        File.WriteAllText(logFile, errorMessage);
        Console.WriteLine($"Error logged to: {logFile}");
    }
    catch
    {
        // If we can't write log, at least we printed to console
    }

    throw; // Re-throw to ensure IIS sees the error
}