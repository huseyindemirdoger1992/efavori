using ImageMagick;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace api
{
    internal class Media
    {
        // <summary>
        // Resmi AVIF yapar ve TÜM META VERİLERİ (EXIF, Konum, Tarih vb.) koruyarak kaydeder.
        // </summary>

        // DESTEKLENEN FORMATLAR (Genişletilmiş Küresel Liste):
        // --------------------------------------------------------------------------------------------------
        // Modern Web: AVIF, WEBP, HEIC, HEIF (iPhone), SVG
        // Standart:   JPG, JPEG, PNG, BMP, GIF, ICO
        // Profesyonel: PSD (Photoshop), AI (Illustrator), EPS, TIFF, TIF, PDF
        // Fotoğrafçı: DNG (Adobe Digital Negative), CR2, NEF, ARW (Raw Kamera Formatları - Temel Destek)
        // Teknik:     TGA, DDS (Oyun Kaplamaları), HDR, EXR (Yüksek Dinamik Aralıklı Görüntüler)
        // --------------------------------------------------------------------------------------------------
        public async Task<string> ConvertToAvifAsync(IFormFile file, string userEmail)
        {
            // 1. Dosya geçerlilik kontrolü
            if (file == null || file.Length == 0)
                throw new ArgumentException("Geçersiz dosya.");

            // 2. Güvenli dosya adı oluşturma (Yasaklı karakter içermeyen tarih formatı)
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileNameOnly = Path.GetFileNameWithoutExtension(file.FileName);
            string uniqueFileName = $"{timestamp}_{Guid.NewGuid().ToString().Substring(0, 8)}_{fileNameOnly}.avif";

            // 3. Klasör hiyerarşisi: wwwroot/uploads/{userEmail}/Images
            string uploadFolder = Path.Combine("wwwroot", "uploads", userEmail, "Images");

            // 4. Klasörleri oluştur (Recursive çalışır)
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string fullPath = Path.Combine(uploadFolder, uniqueFileName);

            // 5. Görsel işleme süreci
            using (var stream = file.OpenReadStream())
            {
                using (var image = new MagickImage(stream))
                {
                    // 6. Format ve Kalite ayarı
                    image.Format = MagickFormat.Avif;
                    image.Quality = 64;

                    // --- ÖNEMLİ DEĞİŞİKLİK ---
                    // image.Strip(); satırı KALDIRILDI. 
                    // Bu sayede EXIF, GPS ve kamera bilgileri dosya içinde kalmaya devam eder.
                    // -------------------------

                    // 7. Renk profilini koru/düzelt (Görüntünün renklerinin değişmemesi için kritik)
                    image.ColorSpace = ColorSpace.sRGB;

                    // 8. Dosyayı asenkron olarak kaydet
                    await image.WriteAsync(fullPath);
                }
            }

            // 9. Veritabanı için dosya adını döndür
            return uniqueFileName;
        }
    }
}