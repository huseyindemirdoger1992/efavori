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

            // Küçültülmüş versiyonlar için isimlendirmeler
            string fileNameAvif_1_2 = $"{timestamp}_{guidPart}_{cleanFileName}_1_2.avif";
            string fileNameAvif_1_4 = $"{timestamp}_{guidPart}_{cleanFileName}_1_4.avif";
            string fileNameAvif_1_8 = $"{timestamp}_{guidPart}_{cleanFileName}_1_8.avif";
            string fileNameAvif_1_16 = $"{timestamp}_{guidPart}_{cleanFileName}_1_16.avif";

            string fullPathAvif = Path.Combine(avifFolder, uniqueFileNameAvif);
            string fullPathOriginal = Path.Combine(originalFolder, uniqueFileNameOriginal);

            // 3. Dosya İşlemleri (Orijinal Kaydetme)
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var originalBytes = memoryStream.ToArray();
            await File.WriteAllBytesAsync(fullPathOriginal, originalBytes);

            // 4. AVIF Dönüştürme ve Oranlama
            memoryStream.Position = 0;
            using var image = new MagickImage();
            await image.ReadAsync(memoryStream);

            image.Format = MagickFormat.Avif;
            image.Quality = 64;
            image.ColorSpace = ColorSpace.sRGB;

            // Ana (Tam Boyut) AVIF Kaydı
            await image.WriteAsync(fullPathAvif);

            // 1/2 Oranında Küçültme
            using (var img_1_2 = image.Clone())
            {
                img_1_2.Resize(Math.Max(1, image.Width / 2), Math.Max(1, image.Height / 2));
                await img_1_2.WriteAsync(Path.Combine(avifFolder, fileNameAvif_1_2));
            }

            // 1/4 Oranında Küçültme
            using (var img_1_4 = image.Clone())
            {
                img_1_4.Resize(Math.Max(1, image.Width / 4), Math.Max(1, image.Height / 4));
                await img_1_4.WriteAsync(Path.Combine(avifFolder, fileNameAvif_1_4));
            }

            // 1/8 Oranında Küçültme
            using (var img_1_8 = image.Clone())
            {
                img_1_8.Resize(Math.Max(1, image.Width / 8), Math.Max(1, image.Height / 8));
                await img_1_8.WriteAsync(Path.Combine(avifFolder, fileNameAvif_1_8));
            }

            // 1/16 Oranında Küçültme
            using (var img_1_16 = image.Clone())
            {
                img_1_16.Resize(Math.Max(1, image.Width / 16), Math.Max(1, image.Height / 16));
                await img_1_16.WriteAsync(Path.Combine(avifFolder, fileNameAvif_1_16));
            }

            // 5. Media Tablosuna Kayıt (Veritabanı İşlemi)
            using var db = new _ApplicationConnectionDb();

            var fileInfoAvif = new FileInfo(fullPathAvif);
            string urlBasePath = $"/{relativeBasePath.Replace("\\", "/")}/avif/";

            var mediaEntry = new Data.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = uniqueFileNameAvif,

                // Ana Dosya Yolları
                FileUrl = $"{urlBasePath}{uniqueFileNameAvif}",

                // Oranlı Dosya Yolları
                FileUrl_Ratio_1_2 = $"{urlBasePath}{fileNameAvif_1_2}",
                FileUrl_Ratio_1_4 = $"{urlBasePath}{fileNameAvif_1_4}",
                FileUrl_Ratio_1_8 = $"{urlBasePath}{fileNameAvif_1_8}",
                FileUrl_Ratio_1_16 = $"{urlBasePath}{fileNameAvif_1_16}",

                FilePhysicalPathRoad = "https://efavori.com" + $"{urlBasePath}{uniqueFileNameAvif}",
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