using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace api.tr
{
    /// <summary>
    /// efavori — Merkezî Azure Blob Storage servisi.
    ///
    /// Tüm medya asset'leri (görsel/video/ses/belge/diğer) artık yerel diske değil,
    /// <c>efavoridata</c> depolama hesabındaki <c>data</c> container'ına yüklenir.
    /// Container "web erişimine açık" (public blob read) olduğundan üretilen linkler
    /// SAS gerektirmeden doğrudan çalışır.
    ///
    /// Link formatı YAPILANDIRILABİLİR: <c>AzureStorage:BaseUrl</c> dolu ise linkler
    /// onun üzerinden (ör. ileride bir CDN / özel alan adı) üretilir; boş ise blob'un
    /// ham URI'si kullanılır. Böylece CDN'e geçildiğinde tek satır ayar değişir,
    /// veritabanındaki mantık ve kod aynı kalır.
    ///
    /// DI: Singleton olarak kaydedilir (BlobServiceClient thread-safe'dir).
    ///     builder.Services.AddSingleton&lt;api.tr.AzureBlobService&gt;();
    /// </summary>
    public class AzureBlobService
    {
        private readonly BlobContainerClient _container;
        private readonly string _baseUrl;

        public string ContainerName { get; }

        public AzureBlobService(IConfiguration configuration)
        {
            var connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "AzureStorage:ConnectionString appsettings.json içinde tanımlı değil.");

            ContainerName = configuration["AzureStorage:Container"] ?? "data";

            // BaseUrl boş bırakılabilir; o durumda ham blob URI'si kullanılır.
            _baseUrl = (configuration["AzureStorage:BaseUrl"] ?? string.Empty).TrimEnd('/');

            var serviceClient = new BlobServiceClient(connectionString);
            _container = serviceClient.GetBlobContainerClient(ContainerName);

            // Container yoksa oluştur; public (anonim) blob okumasına aç.
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        /// <summary>
        /// Bir stream'i verilen blob yoluna yükler ve blob anahtarını (container'a göre
        /// göreli yol) döndürür. StorageKey olarak bu değer saklanır.
        /// </summary>
        public async Task<string> UploadAsync(
            Stream content,
            string blobPath,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (content.CanSeek)
                content.Position = 0;

            var blob = _container.GetBlobClient(blobPath);

            await blob.UploadAsync(
                content,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                },
                cancellationToken);

            return blobPath;
        }

        /// <summary>Byte dizisini blob'a yükler (bellekte hazır olan veriler için).</summary>
        public async Task<string> UploadBytesAsync(
            byte[] data,
            string blobPath,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream(data, writable: false);
            return await UploadAsync(ms, blobPath, contentType, cancellationToken);
        }

        /// <summary>Blob anahtarından, veritabanına yazılacak tam erişim linkini üretir.</summary>
        public string BuildUrl(string blobPath)
        {
            if (!string.IsNullOrWhiteSpace(_baseUrl))
                return $"{_baseUrl}/{blobPath.TrimStart('/')}";

            // BaseUrl yoksa: https://{hesap}.blob.core.windows.net/{container}/{blobPath}
            return _container.GetBlobClient(blobPath).Uri.ToString();
        }

        /// <summary>Fiziksel blob'u siler (asset temizliğinde UsageCount = 0 olunca).</summary>
        public async Task<bool> DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
        {
            var blob = _container.GetBlobClient(blobPath);
            var response = await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            return response.Value;
        }
    }
}
