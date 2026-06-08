using data;
using data._Shared;
using Data;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace api.tr;

public class Media(UserInfos userInfos)
{
    private readonly UserInfos _userInfos = userInfos ?? throw new ArgumentNullException(nameof(userInfos));

    public async Task<(string? FileName, Logs LogData)> ConvertToAvifWithLogAsync(IFormFile file, string userEmail, Guid? userId)
    {
        var detail = _userInfos.GetCurrentUserDetails();
        string? uniqueFileNameAvif = null;
        var targetUserId = userId ?? Guid.Empty;

        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "ConvertToAvifWithLogAsync",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };

        try
        {
            // 1. Yol Tanımlamaları
            string relativeBasePath = Path.Combine("_files", "users", userEmail, "images");
            string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), relativeBasePath);
            string avifFolder = Path.Combine(baseUserPath, "avif");
            string originalFolder = Path.Combine(baseUserPath, "original");

            if (!Directory.Exists(avifFolder)) Directory.CreateDirectory(avifFolder);
            if (!Directory.Exists(originalFolder)) Directory.CreateDirectory(originalFolder);

            // 2. İsimlendirme
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string guidPart = Guid.NewGuid().ToString("N");
            string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);
            string originalExtension = Path.GetExtension(file.FileName);

            uniqueFileNameAvif = $"{timestamp}_{guidPart}_{cleanFileName}.avif";
            string uniqueFileNameOriginal = $"{timestamp}_{guidPart}_{cleanFileName}{originalExtension}";

            string fullPathAvif = Path.Combine(avifFolder, uniqueFileNameAvif);
            string fullPathOriginal = Path.Combine(originalFolder, uniqueFileNameOriginal);

            // 3. Dosya İşlemleri (Orijinal Kaydetme)
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var originalBytes = memoryStream.ToArray();
            await File.WriteAllBytesAsync(fullPathOriginal, originalBytes);

            // 4. AVIF Dönüştürme
            memoryStream.Position = 0;
            using var image = new MagickImage();
            await image.ReadAsync(memoryStream);

            image.Format = MagickFormat.Avif;
            image.Quality = 64;
            image.ColorSpace = ColorSpace.sRGB;
            await image.WriteAsync(fullPathAvif);

            // 5. Media Tablosuna Kayıt (Veritabanı İşlemi)
            using var db = new _ApplicationConnectionDb();

            // AVIF Dosyası İçin Kayıt
            var fileInfoAvif = new FileInfo(fullPathAvif);
            var mediaEntry = new Data.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                FileName = file.FileName, // Orijinal adı saklıyoruz
                FileStoredName = uniqueFileNameAvif,
                FileUrl = $"/{relativeBasePath.Replace("\\", "/")}/avif/{uniqueFileNameAvif}",
                FilePhysicalPathRoad = "https://efavori.com" + $"/{relativeBasePath.Replace("\\", "/")}/avif/{uniqueFileNameAvif}",
                OrjFileUrl = $"/{relativeBasePath.Replace("\\", "/")}/original/{uniqueFileNameOriginal}",
                OrjFilePhysicalPathRoad = "https://efavori.com" + $"/{relativeBasePath.Replace("\\", "/")}/original/{uniqueFileNameOriginal}",
                FileExtensionType = ".avif",
                ContentType = "image/avif",
                OriginalSize = file.Length,
                CompressedSize = fileInfoAvif.Length,
                CreatedAt = DateTime.UtcNow,
                IsDeletedStatu = false
            };

            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;

            // Hata durumunda logu yine de kaydetmek için
            using var db = new _ApplicationConnectionDb();
            db.Logs.Add(logEntry);
            await db.SaveChangesAsync();
        }

        return (uniqueFileNameAvif, logEntry);
    }
    public async Task allowedExtensions_videos(IFormFile file, string userEmail, Guid? userId)
    {
        var detail = _userInfos.GetCurrentUserDetails();
        var targetUserId = userId ?? Guid.Empty;

        // 1. Log Nesnesini Hazırla
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "allowedExtensions_videos",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };

        try
        {
            // 2. Klasör ve Yol Yönetimi (Relative path veritabanı için önemlidir)
            string relativeFolder = Path.Combine("_files", "users", userEmail, "videos");
            string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

            if (!Directory.Exists(baseUserPath))
                Directory.CreateDirectory(baseUserPath);

            // 3. Dosya İsimlendirme
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string guidPart = Guid.NewGuid().ToString("N");
            string extension = Path.GetExtension(file.FileName).ToLower(); // Küçük harf standartı
            string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

            string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
            string fullPath = Path.Combine(baseUserPath, uniqueFileName);

            // 4. Dosyayı Kaydetme (Stream kullanımı video için idealdir)
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Media Tablosuna Kayıt
            using var db = new _ApplicationConnectionDb();

            var mediaEntry = new Data.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = uniqueFileName,
                // URL oluştururken ters slash'ları web uyumlu düz slash'a çeviriyoruz
                FileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FileExtensionType = extension,
                ContentType = file.ContentType, // Video formatını (video/mp4 vb.) yakalar
                OriginalSize = file.Length,
                CompressedSize = file.Length, // Video sıkıştırma yoksa aynı kalır
                CreatedAt = DateTime.UtcNow,
                IsDeletedStatu = false
            };

            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Hata durumunda logu bağımsız kaydet
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;

            try
            {
                using var db = new _ApplicationConnectionDb();
                db.Logs.Add(logEntry);
                await db.SaveChangesAsync();
            }
            catch { /* Veritabanı erişilemezse yapacak bir şey yok */ }

            throw; // Hatayı üst katmana fırlatmak genellikle daha sağlıklıdır
        }
    }
    public async Task allowedExtensions_sountds(IFormFile file, string userEmail, Guid? userId)
    {
        var detail = _userInfos.GetCurrentUserDetails();
        var targetUserId = userId ?? Guid.Empty;

        // 1. Log nesnesini hazırla
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "allowedExtensions_sountds",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };

        try
        {
            // 2. Klasör Yolları (Relative path veritabanı sorguları için önemlidir)
            string relativeFolder = Path.Combine("_files", "users", userEmail, "sounds");
            string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

            if (!Directory.Exists(baseUserPath))
                Directory.CreateDirectory(baseUserPath);

            // 3. Dosya İsimlendirme
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string guidPart = Guid.NewGuid().ToString("N");
            string extension = Path.GetExtension(file.FileName).ToLower();
            string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

            string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
            string fullPath = Path.Combine(baseUserPath, uniqueFileName);

            // 4. Fiziksel Kayıt (FileStream)
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Veritabanı İşlemleri (Media + Log)
            using var db = new _ApplicationConnectionDb();

            var mediaEntry = new Data.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = uniqueFileName,
                FileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FileExtensionType = extension,
                ContentType = file.ContentType ?? "audio/mpeg", // Null gelirse varsayılan atar
                OriginalSize = file.Length,
                CompressedSize = file.Length, // Ses dosyalarında genelde işlem sonrası boyut aynıdır
                CreatedAt = DateTime.UtcNow,
                IsDeletedStatu = false
            };

            logEntry.Exception = "Success";
            logEntry.Action = "SoundUploadSuccess";

            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Hata durumunda sadece Log kaydı atmaya çalış
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;
            logEntry.Action = "SoundUploadError";

            try
            {
                using var db = new _ApplicationConnectionDb();
                db.Logs.Add(logEntry);
                await db.SaveChangesAsync();
            }
            catch { /* DB log hatası yutulur */ }

            throw; // Hatayı controller tarafına fırlat ki kullanıcıya 500 dönebilsin
        }
    }
    public async Task allowedExtensions_documents(IFormFile file, string userEmail, Guid? userId)
    {
        var detail = _userInfos.GetCurrentUserDetails();
        var targetUserId = userId ?? Guid.Empty;

        // 1. Log Nesnesini Hazırla
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "allowedExtensions_documents",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };

        try
        {
            // 2. Klasör Yolları
            string relativeFolder = Path.Combine("_files", "users", userEmail, "documents");
            string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

            if (!Directory.Exists(baseUserPath))
                Directory.CreateDirectory(baseUserPath);

            // 3. Dosya İsimlendirme
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string guidPart = Guid.NewGuid().ToString("N");
            string extension = Path.GetExtension(file.FileName).ToLower();
            string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

            string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
            string fullPath = Path.Combine(baseUserPath, uniqueFileName);

            // 4. Fiziksel Kayıt (Async Stream)
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Veritabanı İşlemleri (Media + Log)
            using var db = new _ApplicationConnectionDb();

            var mediaEntry = new Data.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = uniqueFileName,
                // Tarayıcı uyumlu URL oluşturma
                FileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FileExtensionType = extension,
                ContentType = file.ContentType ?? "application/octet-stream",
                OriginalSize = file.Length,
                CompressedSize = file.Length,
                CreatedAt = DateTime.UtcNow,
                IsDeletedStatu = false
            };

            logEntry.Exception = "Success";
            logEntry.Action = "DocumentUploadSuccess";

            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);

            // Media ve Log aynı anda kaydedilir (Atomik İşlem)
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Hata durumunda log detaylarını doldur ve bağımsız kaydet
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;
            logEntry.Action = "DocumentUploadError";

            try
            {
                using var db = new _ApplicationConnectionDb();
                db.Logs.Add(logEntry);
                await db.SaveChangesAsync();
            }
            catch { /* DB Log hatası yutulur */ }

            throw; // Hata yukarı fırlatılır
        }
    }
    public async Task allowedExtensions_others(IFormFile file, string userEmail, Guid? userId)
    {
        var detail = _userInfos.GetCurrentUserDetails();
        var targetUserId = userId ?? Guid.Empty;

        // 1. Log nesnesini başlat
        var logEntry = new Logs
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            PageNameSpaceTitle = "namespace api.tr",
            Action = "allowedExtensions_others",
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };

        try
        {
            // 2. Klasör Yollarının Hazırlanması (Relative path veritabanı için kritik)
            string relativeFolder = Path.Combine("_files", "users", userEmail, "others");
            string baseUserPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

            if (!Directory.Exists(baseUserPath))
                Directory.CreateDirectory(baseUserPath);

            // 3. Dosya İsimlendirme
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string guidPart = Guid.NewGuid().ToString("N");
            string extension = Path.GetExtension(file.FileName).ToLower();
            string cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);

            string uniqueFileName = $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
            string fullPath = Path.Combine(baseUserPath, uniqueFileName);

            // 4. Dosyayı Kaydetme (Async Stream)
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Media ve Log Kayıtlarını Hazırla
            using var db = new _ApplicationConnectionDb();

            var mediaEntry = new Data.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = uniqueFileName,
                FileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFileUrl = $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                OrjFilePhysicalPathRoad = "https://efavori.com" + $"/{relativeFolder.Replace("\\", "/")}/{uniqueFileName}",
                FileExtensionType = extension,
                ContentType = file.ContentType ?? "application/octet-stream",
                OriginalSize = file.Length,
                CompressedSize = file.Length,
                CreatedAt = DateTime.UtcNow,
                IsDeletedStatu = false
            };

            logEntry.Exception = "Success";
            logEntry.Action = "OtherFileUploadSuccess";

            // İki kaydı da aynı anda ekleyip kaydediyoruz
            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Hata durumunda logu bağımsız kaydetmeye çalış
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;
            logEntry.Action = "OtherFileUploadError";

            try
            {
                using var db = new _ApplicationConnectionDb();
                db.Logs.Add(logEntry);
                await db.SaveChangesAsync();
            }
            catch { /* Veritabanı log hatası sessizce geçilir */ }

            throw; // Hatayı controller katmanına bildir
        }
    }
}