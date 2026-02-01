using data;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace api.tr;

public class Media(UserInfos userInfos)
{
    private readonly UserInfos _userInfos = userInfos ?? throw new ArgumentNullException(nameof(userInfos));

    public async Task<(string FileName, Logs LogData)> ConvertToAvifWithLogAsync(IFormFile file, string userEmail, Guid? userId)
    {
        var watch = Stopwatch.StartNew();
        var detail = _userInfos.GetCurrentUserDetails();

        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.Empty,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "ConvertToAvifWithLogAsync",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };

        // 1. Klasör Yollarının Hazırlanması
        string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), "_files", "users", userEmail, "images");
        string avifFolder = Path.Combine(baseUserPath, "avif");
        string originalFolder = Path.Combine(baseUserPath, "original");

        // Klasörler yoksa oluştur
        if (!Directory.Exists(avifFolder)) Directory.CreateDirectory(avifFolder);
        if (!Directory.Exists(originalFolder)) Directory.CreateDirectory(originalFolder);

        // 2. Dosya İsimlerinin Hazırlanması
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string guidPart = Guid.NewGuid().ToString("N");
        string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

        string originalExtension = Path.GetExtension(file.FileName);
        string uniqueFileNameAvif = $"{timestamp}_{guidPart}_{cleanFileName}.avif";
        string uniqueFileNameOriginal = $"{timestamp}_{guidPart}_{cleanFileName}{originalExtension}";

        string fullPathAvif = Path.Combine(avifFolder, uniqueFileNameAvif);
        string fullPathOriginal = Path.Combine(originalFolder, uniqueFileNameOriginal);

        // 3. İşlem Akışı
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        // --- Orijinal Dosyayı Kaydet ---
        // MemoryStream'i diske yazıyoruz (Dosya akışını bozmamak için kopyasını alıyoruz)
        await File.WriteAllBytesAsync(fullPathOriginal, memoryStream.ToArray());

        // --- ImageMagick Dönüşüm İşlemi ---
        memoryStream.Position = 0;
        using var image = new MagickImage();
        await image.ReadAsync(memoryStream);

        image.Format = MagickFormat.Avif;
        image.Quality = 64;
        image.ColorSpace = ColorSpace.sRGB;

        await image.WriteAsync(fullPathAvif);

        watch.Stop();
        logEntry.Exception = "Success";

        // 4. Veritabanı Kaydı
        using var db = new _ApplicationConnectionDb();
        db.Logs.Add(logEntry);
        await db.SaveChangesAsync();

        return (uniqueFileNameAvif, logEntry);
    }

    public async Task allowedExtensions_videos(IFormFile file, string userEmail, Guid? userId)
    {
        var watch = Stopwatch.StartNew();
        var detail = _userInfos.GetCurrentUserDetails();

        // 1. Klasör Yollarının Hazırlanması
        // Hedef: _files/users/{email}/videos
        string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), "_files", "users", userEmail, "videos");

        if (!Directory.Exists(baseUserPath))
            Directory.CreateDirectory(baseUserPath);

        // 2. Dosya İsimlerinin Hazırlanması (Çakışmayı önlemek için benzersiz isim)
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string guidPart = Guid.NewGuid().ToString("N");
        string extension = Path.GetExtension(file.FileName);
        string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

        string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
        string fullPath = Path.Combine(baseUserPath, uniqueFileName);

        // 3. Dosyayı Kaydetme İşlemi
        // MemoryStream yerine doğrudan FileStream kullanarak RAM kullanımını minimize ediyoruz
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        watch.Stop();

        // 4. Log Kaydı
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.Empty,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "VideoUploadSuccess",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow,
            Exception = "Success"
        };

        using var db = new _ApplicationConnectionDb();
        db.Logs.Add(logEntry);
        await db.SaveChangesAsync();
    }

    public async Task allowedExtensions_sountds(IFormFile file, string userEmail, Guid? userId)
    {
        var watch = Stopwatch.StartNew();
        var detail = _userInfos.GetCurrentUserDetails();

        // 1. Klasör Yollarının Hazırlanması
        // Hedef: _files/users/{email}/sounds
        string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), "_files", "users", userEmail, "sounds");

        if (!Directory.Exists(baseUserPath))
            Directory.CreateDirectory(baseUserPath);

        // 2. Dosya İsimlendirme (Benzersizlik ve Güvenlik için)
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string guidPart = Guid.NewGuid().ToString("N");
        string extension = Path.GetExtension(file.FileName);
        string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

        string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
        string fullPath = Path.Combine(baseUserPath, uniqueFileName);

        // 3. Kayıt İşlemi (Doğrudan Stream kullanarak)
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        watch.Stop();

        // 4. Log Kaydı
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.Empty,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "SoundUploadSuccess",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow,
            Exception = "Success"
        };

        using var db = new _ApplicationConnectionDb();
        db.Logs.Add(logEntry);
        await db.SaveChangesAsync();
    }

    public async Task allowedExtensions_documents(IFormFile file, string userEmail, Guid? userId)
    {
        var watch = Stopwatch.StartNew();
        var detail = _userInfos.GetCurrentUserDetails();

        // 1. Klasör Yollarının Hazırlanması
        // Hedef: _files/users/{email}/documents
        string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), "_files", "users", userEmail, "documents");

        if (!Directory.Exists(baseUserPath))
            Directory.CreateDirectory(baseUserPath);

        // 2. Dosya İsimlendirme
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string guidPart = Guid.NewGuid().ToString("N");
        string extension = Path.GetExtension(file.FileName);
        string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

        string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
        string fullPath = Path.Combine(baseUserPath, uniqueFileName);

        // 3. Dosyayı Kaydetme (Async Stream)
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        watch.Stop();

        // 4. Log Kaydı
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.Empty,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "DocumentUploadSuccess",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow,
            Exception = "Success"
        };

        using var db = new _ApplicationConnectionDb();
        db.Logs.Add(logEntry);
        await db.SaveChangesAsync();
    }

    public async Task allowedExtensions_others(IFormFile file, string userEmail, Guid? userId)
    {
        var watch = Stopwatch.StartNew();
        var detail = _userInfos.GetCurrentUserDetails();

        // 1. Klasör Yollarının Hazırlanması
        // Hedef: _files/users/{email}/others
        string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), "_files", "users", userEmail, "others");

        if (!Directory.Exists(baseUserPath))
            Directory.CreateDirectory(baseUserPath);

        // 2. Dosya İsimlendirme
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string guidPart = Guid.NewGuid().ToString("N");
        string extension = Path.GetExtension(file.FileName);
        string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

        // Güvenlik: Dosya ismindeki geçersiz karakterleri temizlemek gerekirse buraya müdahale edilebilir.
        string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
        string fullPath = Path.Combine(baseUserPath, uniqueFileName);

        // 3. Dosyayı Kaydetme
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        watch.Stop();

        // 4. Log Kaydı
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.Empty,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "OtherFileUploadSuccess",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow,
            Exception = "Success"
        };

        using var db = new _ApplicationConnectionDb();
        db.Logs.Add(logEntry);
        await db.SaveChangesAsync();
    }
}