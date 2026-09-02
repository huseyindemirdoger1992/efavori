using data;
using data.Owned;
using data._Carts;
using data._Categories;
using data._Follows;
using data._Galleries;
using data._Helper;
using data._Locations;
using data._Products;
using data._Shares;
using data._Store;
using data._Systems;
using data._Tasks;
using data._Users;

using ImageMagick;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text.Json;

namespace api.tr;

/// <summary>
/// Medya yükleme servisi — V3 (Azure Blob).
///
/// Bu sürümde tüm dosya türleri (görsel/AVIF türevleri, video, ses, belge, diğer)
/// yerel diske ("_files/users/...") değil, merkezî <see cref="AzureBlobService"/>
/// üzerinden <c>efavoridata</c> hesabındaki <c>data</c> container'ına yüklenir.
/// Veritabanına, <c>AzureStorage:BaseUrl</c>'e göre üretilen TAM erişim linki yazılır.
///
/// Depolama meta verisi (StorageProvider / StorageBucket / StorageKey) doldurulur;
/// böylece asset temizliği ve dedup, merkezî Media modeline (§14) uygun çalışır.
/// </summary>
public class Media(UserInfos userInfos, AzureBlobService blob)
{
    private readonly UserInfos _userInfos = userInfos ?? throw new ArgumentNullException(nameof(userInfos));
    private readonly AzureBlobService _blob = blob ?? throw new ArgumentNullException(nameof(blob));

    private const string StorageProviderName = "AzureBlob";

    // ── Ortak yardımcılar ────────────────────────────────────────────────────

    /// <summary>Blob adı için kullanıcı e-postasını güvenli hale getirir.</summary>
    private static string SafeSegment(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "unknown";
        foreach (var c in Path.GetInvalidFileNameChars())
            value = value.Replace(c, '_');
        return value.Trim().TrimEnd('.');
    }

    private static string BuildStoredName(string originalFileName, string extension)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string guidPart = Guid.NewGuid().ToString("N");
        string cleanFileName = SafeSegment(Path.GetFileNameWithoutExtension(originalFileName));
        return $"{timestamp}_{guidPart}_{cleanFileName}{extension}";
    }

    private Logs NewLog(string action, Guid targetUserId)
    {
        var detail = _userInfos.GetCurrentUserDetails();
        return new Logs
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            PageNameSpaceTitle = "namespace api.tr",
            Action = action,
            IpAddress = detail.IpAddress,
            UserAgent = detail.UserAgent,
            RequestPath = detail.RequestPath,
            Languages = detail.Languages,
            Date = DateTime.UtcNow
        };
    }

    /// <summary>Basit, akan (streaming) yüklemeler için ortak gövde.</summary>
    private async Task UploadSimpleAsync(
        IFormFile file,
        string userEmail,
        Guid? userId,
        string subFolder,
        MediaAssetType mediaType,
        string defaultContentType,
        string action)
    {
        var targetUserId = userId ?? Guid.Empty;
        var logEntry = NewLog(action, targetUserId);

        try
        {
            string emailSafe = SafeSegment(userEmail);
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string storedName = BuildStoredName(file.FileName, extension);
            string blobPath = $"users/{emailSafe}/{subFolder}/{storedName}";
            string contentType = string.IsNullOrWhiteSpace(file.ContentType) ? defaultContentType : file.ContentType;

            // Doğrudan blob'a stream (dosya belleğe tam yüklenmez).
            await using (var stream = file.OpenReadStream())
            {
                await _blob.UploadAsync(stream, blobPath, contentType);
            }

            string publicUrl = _blob.BuildUrl(blobPath);

            using var db = new data._ApplicationConnectionDb();

            var mediaEntry = new data._Galleries.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                CreatedByUserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = storedName,

                StorageProvider = StorageProviderName,
                StorageBucket = _blob.ContainerName,
                StorageKey = blobPath,

                FileUrl = publicUrl,
                FilePhysicalPathRoad = publicUrl,
                OrjFileUrl = publicUrl,
                OrjFilePhysicalPathRoad = publicUrl,

                FileExtensionType = extension,
                ContentType = contentType,
                MediaType = mediaType,
                OriginalSize = file.Length,
                CompressedSize = file.Length,

                ProcessingStatus = MediaProcessingStatus.Ready,
                ProcessedAt = DateTime.UtcNow,
                Visibility = MediaVisibility.Public,

                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = new data.Owned.IsDeleted { IsDeletedStatu = false }
            };

            logEntry.Exception = "Success";
            logEntry.Action = $"{action}Success";

            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;
            logEntry.Action = $"{action}Error";

            try
            {
                using var db = new data._ApplicationConnectionDb();
                db.Logs.Add(logEntry);
                await db.SaveChangesAsync();
            }
            catch { /* DB log hatası yutulur */ }

            throw;
        }
    }

    // ── Görsel → AVIF + türevler (Azure) ──────────────────────────────────────

    public async Task<(string? FileName, Logs LogData)> ConvertToAvifWithLogAsync(IFormFile file, string userEmail, Guid? userId)
    {
        var targetUserId = userId ?? Guid.Empty;
        var logEntry = NewLog("ConvertToAvifWithLogAsync", targetUserId);
        string? uniqueFileNameAvif = null;

        try
        {
            string emailSafe = SafeSegment(userEmail);
            string imagesPrefix = $"users/{emailSafe}/images";
            string avifPrefix = $"{imagesPrefix}/avif";
            string originalPrefix = $"{imagesPrefix}/original";

            // İsimlendirme (tek zaman damgası + guid; tüm türevler ortak köke sahip).
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string guidPart = Guid.NewGuid().ToString("N");
            string cleanFileName = SafeSegment(Path.GetFileNameWithoutExtension(file.FileName));
            string originalExtension = Path.GetExtension(file.FileName);

            uniqueFileNameAvif = $"{timestamp}_{guidPart}_{cleanFileName}.avif";
            string uniqueFileNameOriginal = $"{timestamp}_{guidPart}_{cleanFileName}{originalExtension}";

            string fileNameAvif_1_2 = $"{timestamp}_{guidPart}_{cleanFileName}_1_2.avif";
            string fileNameAvif_1_4 = $"{timestamp}_{guidPart}_{cleanFileName}_1_4.avif";
            string fileNameAvif_1_8 = $"{timestamp}_{guidPart}_{cleanFileName}_1_8.avif";
            string fileNameAvif_1_16 = $"{timestamp}_{guidPart}_{cleanFileName}_1_16.avif";

            // 1. Orijinali belleğe al.
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var originalBytes = memoryStream.ToArray();

            // Aynı içeriğin tespiti (dedup) için SHA-256.
            string sha256 = Convert.ToHexString(SHA256.HashData(originalBytes)).ToLowerInvariant();

            // 2. Orijinali blob'a yükle.
            string originalBlobPath = $"{originalPrefix}/{uniqueFileNameOriginal}";
            string originalContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType;
            await _blob.UploadBytesAsync(originalBytes, originalBlobPath, originalContentType);

            // 3. AVIF dönüştürme.
            memoryStream.Position = 0;
            using var image = new MagickImage();
            await image.ReadAsync(memoryStream);

            image.Format = MagickFormat.Avif;
            image.Quality = 64;
            image.ColorSpace = ColorSpace.sRGB;

            int width = (int)image.Width;
            int height = (int)image.Height;

            // Verilen AVIF bytes'ını blob'a yükler (image tipinden bağımsız → Magick.NET
            // sürüm farklarından etkilenmez).
            async Task<(string blobPath, long size)> PutAvifAsync(byte[] bytes, string name)
            {
                string path = $"{avifPrefix}/{name}";
                await _blob.UploadBytesAsync(bytes, path, "image/avif");
                return (path, bytes.Length);
            }

            // Ana (tam boyut) AVIF.
            byte[] mainBytes;
            using (var ms = new MemoryStream())
            {
                await image.WriteAsync(ms);
                mainBytes = ms.ToArray();
            }
            var (avifBlobPath, avifSize) = await PutAvifAsync(mainBytes, uniqueFileNameAvif);

            // Türevler.
            string blobAvif_1_2, blobAvif_1_4, blobAvif_1_8, blobAvif_1_16;

            using (var img = image.Clone())
            {
                img.Resize((uint)Math.Max(1, width / 2), (uint)Math.Max(1, height / 2));
                using var ms = new MemoryStream();
                await img.WriteAsync(ms);
                (blobAvif_1_2, _) = await PutAvifAsync(ms.ToArray(), fileNameAvif_1_2);
            }
            using (var img = image.Clone())
            {
                img.Resize((uint)Math.Max(1, width / 4), (uint)Math.Max(1, height / 4));
                using var ms = new MemoryStream();
                await img.WriteAsync(ms);
                (blobAvif_1_4, _) = await PutAvifAsync(ms.ToArray(), fileNameAvif_1_4);
            }
            using (var img = image.Clone())
            {
                img.Resize((uint)Math.Max(1, width / 8), (uint)Math.Max(1, height / 8));
                using var ms = new MemoryStream();
                await img.WriteAsync(ms);
                (blobAvif_1_8, _) = await PutAvifAsync(ms.ToArray(), fileNameAvif_1_8);
            }
            using (var img = image.Clone())
            {
                img.Resize((uint)Math.Max(1, width / 16), (uint)Math.Max(1, height / 16));
                using var ms = new MemoryStream();
                await img.WriteAsync(ms);
                (blobAvif_1_16, _) = await PutAvifAsync(ms.ToArray(), fileNameAvif_1_16);
            }

            // 4. Tam erişim linkleri.
            string urlAvif = _blob.BuildUrl(avifBlobPath);
            string urlAvif_1_2 = _blob.BuildUrl(blobAvif_1_2);
            string urlAvif_1_4 = _blob.BuildUrl(blobAvif_1_4);
            string urlAvif_1_8 = _blob.BuildUrl(blobAvif_1_8);
            string urlAvif_1_16 = _blob.BuildUrl(blobAvif_1_16);
            string urlOriginal = _blob.BuildUrl(originalBlobPath);

            var renditionsJson = JsonSerializer.Serialize(new
            {
                full = urlAvif,
                ratio_1_2 = urlAvif_1_2,
                ratio_1_4 = urlAvif_1_4,
                ratio_1_8 = urlAvif_1_8,
                ratio_1_16 = urlAvif_1_16
            });

            // 5. Media kaydı.
            using var db = new data._ApplicationConnectionDb();

            var mediaEntry = new data._Galleries.Media
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                CreatedByUserId = targetUserId,
                FileName = file.FileName,
                FileStoredName = uniqueFileNameAvif,

                StorageProvider = StorageProviderName,
                StorageBucket = _blob.ContainerName,
                StorageKey = avifBlobPath,

                FileUrl = urlAvif,
                FileUrl_Ratio_1_2 = urlAvif_1_2,
                FileUrl_Ratio_1_4 = urlAvif_1_4,
                FileUrl_Ratio_1_8 = urlAvif_1_8,
                FileUrl_Ratio_1_16 = urlAvif_1_16,
                RenditionsJson = renditionsJson,

                FilePhysicalPathRoad = urlAvif,
                OrjFileUrl = urlOriginal,
                OrjFilePhysicalPathRoad = urlOriginal,

                FileExtensionType = ".avif",
                ContentType = "image/avif",
                MediaType = MediaAssetType.Image,

                Width = width,
                Height = height,
                Sha256 = sha256,

                OriginalSize = file.Length,
                CompressedSize = avifSize,

                ProcessingStatus = MediaProcessingStatus.Ready,
                ProcessedAt = DateTime.UtcNow,
                Visibility = MediaVisibility.Public,

                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = new data.Owned.IsDeleted { IsDeletedStatu = false }
            };

            logEntry.Exception = "Success";
            logEntry.Action = "ImageUploadSuccess";

            db.Media.Add(mediaEntry);
            db.Logs.Add(logEntry);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logEntry.Exception = ex.Message;
            logEntry.StackTrace = ex.StackTrace;
            logEntry.Action = "ImageUploadError";

            try
            {
                using var db = new data._ApplicationConnectionDb();
                db.Logs.Add(logEntry);
                await db.SaveChangesAsync();
            }
            catch { /* DB log hatası yutulur */ }
        }

        return (uniqueFileNameAvif, logEntry);
    }

    // ── Video / Ses / Belge / Diğer (Azure, streaming) ────────────────────────

    public Task allowedExtensions_videos(IFormFile file, string userEmail, Guid? userId) =>
        UploadSimpleAsync(file, userEmail, userId,
            subFolder: "videos",
            mediaType: MediaAssetType.Video,
            defaultContentType: "video/mp4",
            action: "VideoUpload");

    public Task allowedExtensions_sountds(IFormFile file, string userEmail, Guid? userId) =>
        UploadSimpleAsync(file, userEmail, userId,
            subFolder: "sounds",
            mediaType: MediaAssetType.Audio,
            defaultContentType: "audio/mpeg",
            action: "SoundUpload");

    public Task allowedExtensions_documents(IFormFile file, string userEmail, Guid? userId) =>
        UploadSimpleAsync(file, userEmail, userId,
            subFolder: "documents",
            mediaType: MediaAssetType.Document,
            defaultContentType: "application/octet-stream",
            action: "DocumentUpload");

    public Task allowedExtensions_others(IFormFile file, string userEmail, Guid? userId) =>
        UploadSimpleAsync(file, userEmail, userId,
            subFolder: "others",
            mediaType: MediaAssetType.Other,
            defaultContentType: "application/octet-stream",
            action: "OtherFileUpload");
}
