using data;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace api
{
    /// <summary>
    /// Medya işleme operasyonlarını yüksek performans ve detaylı izlenebilirlik ile yönetir.
    /// </summary>
    public class Media
    {
        public readonly UserInfos _userInfos;

        public Media(UserInfos userInfos)
        {
            _userInfos = userInfos ?? throw new ArgumentNullException(nameof(userInfos));
        }

        // <summary>
        // Gelen görseli AVIF formatına dönüştürür, meta verileri korur ve süreci atomik olarak loglar.
        // </summary>
        
        // --------------------------------------------------------------------------------------------------
        // TANIMLANAN/DESTEKLENEN RESİM DOSYA FORMATLARI
        // Modern Web: AVIF, WEBP, HEIC, HEIF (iPhone), SVG
        // Standart:   JPG, JPEG, PNG, BMP, GIF, ICO
        // Profesyonel: PSD (Photoshop), AI (Illustrator), EPS, TIFF, TIF, PDF
        // Fotoğrafçı: DNG (Adobe Digital Negative), CR2, NEF, ARW (Raw Kamera Formatları - Temel Destek)
        // Teknik:     TGA, DDS (Oyun Kaplamaları), HDR, EXR (Yüksek Dinamik Aralıklı Görüntüler)
        // --------------------------------------------------------------------------------------------------
        public async Task<(string FileName, Logs LogData)> ConvertToAvifWithLogAsync( IFormFile file, string userEmail, Guid? userId)
        {
            // Performans ölçümü ve kullanıcı detaylarını başlangıçta yakala
            var watch = Stopwatch.StartNew();
            var detail = _userInfos.GetCurrentUserDetails();

            // Log nesnesini zengin içerikle ilklendir
            var logEntry = new Logs
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.Empty,
                PageNameSpaceTitle = "MediaService",
                Action = "ConvertToAvif",
                IpAddress = detail.IpAddress,
                UserAgent = detail.UserAgent,
                RequestPath = detail.RequestPath,
                Languages = detail.Languages,
                Date = DateTime.UtcNow
            };

            try
            {
                // 1. Validasyon: Dosya varlığı ve içeriği
                if (file == null || file.Length == 0)
                {
                    throw new FileNotFoundException("İşlem yapılacak dosya bulunamadı veya boş.");
                }

                // 2. Güvenli Dosya Adı Yönetimi (Collision riskini minimize eder)
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                string fileNameOnly = Path.GetFileNameWithoutExtension(file.FileName);
                // Dosya adındaki geçersiz karakterleri temizle ve benzersiz hale getir
                string sanitizedFileName = string.Join("_", fileNameOnly.Split(Path.GetInvalidFileNameChars()));
                string uniqueFileName = $"{timestamp}_{Guid.NewGuid():N}_{sanitizedFileName}.avif";

                // 3. Dizin Hiyerarşisi (Sistem güvenliği için wwwroot köklü)
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", userEmail, "images");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fullPath = Path.Combine(uploadFolder, uniqueFileName);

                // 4. Görsel İşleme (Magick.NET Bellek Yönetimi)
                using (var stream = file.OpenReadStream())
                {
                    using (var image = new MagickImage(stream))
                    {
                        // Modern web standartları: AVIF, sRGB ve Optimize edilmiş kalite
                        image.Format = MagickFormat.Avif;
                        image.Quality = 64; // Visual/Size dengesi için ideal oran
                        image.ColorSpace = ColorSpace.sRGB;

                        // Dosyayı asenkron olarak diske yaz
                        await image.WriteAsync(fullPath);
                    }
                }

                // Operasyon başarıyla tamamlandı
                watch.Stop();
                logEntry.Exception = "Success";

                return (uniqueFileName, logEntry);
            }
            catch (Exception ex)
            {
                // Kritik hata yönetimi: Hatanın tüm izlerini kaydet
                watch.Stop();
                logEntry.Exception = ex.Message;
                logEntry.StackTrace = ex.StackTrace;

                // Üst katmanın (Controller) hatadan haberdar olması sağlanır
                throw;
            }
            finally
            {
                using (data._ApplicationConnectionDb db = new data._ApplicationConnectionDb())
                {
                    db.Logs.Add(logEntry);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}