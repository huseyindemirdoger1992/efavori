using data.Owned;
using System.ComponentModel.DataAnnotations;

namespace data._Galleries
{
    /// <summary>
    /// Sistemdeki fiziksel veya harici medya kaynağının teknik, depolama, işleme ve dosya meta verilerini tutar.
    /// </summary>
    public class Media
    {
        // Medyanın benzersiz kimliğidir.
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Medyayı sisteme yükleyen veya oluşturan kullanıcıyı belirtir.
        public Guid? UserId { get; set; }

        // Medyanın insan tarafından görülen orijinal dosya adıdır.
        [MaxLength(512)]
        public string? FileName { get; set; }

        // Medyanın depolama sisteminde kullanılan benzersiz fiziksel dosya adıdır.
        [MaxLength(512)]
        public string? FileStoredName { get; set; }

        // Medyanın genel türünü belirtir ve Image, Video, Audio, Document, File veya VideoEmbed gibi değerler içerebilir.
        [MaxLength(50)]
        public string? MediaType { get; set; }

        // Medyanın MIME içerik türünü belirtir ve image/jpeg, video/mp4 veya application/pdf gibi değerler taşıyabilir.
        [MaxLength(255)]
        public string? ContentType { get; set; }

        // Medyanın dosya uzantısını belirtir ve jpg, png, mp4 veya pdf gibi değerler taşıyabilir.
        [MaxLength(20)]
        public string? FileExtensionType { get; set; }

        // Medyanın depolandığı servis veya altyapının adını belirtir.
        [MaxLength(100)]
        public string? StorageProvider { get; set; }

        // Medyanın depolandığı bucket, container veya storage alanını belirtir.
        [MaxLength(255)]
        public string? StorageBucket { get; set; }

        // Medyanın depolama sistemindeki benzersiz anahtarını belirtir.
        [MaxLength(1024)]
        public string? StorageKey { get; set; }

        // Medyanın optimize edilmiş veya aktif olarak kullanılan public URL adresini belirtir.
        [MaxLength(2048)]
        public string? FileUrl { get; set; }

        // Medyanın orijinal ve değiştirilmemiş public URL adresini belirtir.
        [MaxLength(2048)]
        public string? OrjFileUrl { get; set; }

        // Medyanın sunucu veya fiziksel depolama üzerindeki aktif yolunu belirtir.
        [MaxLength(2048)]
        public string? FilePhysicalPathRoad { get; set; }

        // Medyanın orijinal ve değiştirilmemiş fiziksel depolama yolunu belirtir.
        [MaxLength(2048)]
        public string? OrjFilePhysicalPathRoad { get; set; }

        // Medyanın yarı boyutlu optimize edilmiş sürümünün URL adresini belirtir.
        [MaxLength(2048)]
        public string? FileUrl_Ratio_1_2 { get; set; }

        // Medyanın dörtte bir boyutlu optimize edilmiş sürümünün URL adresini belirtir.
        [MaxLength(2048)]
        public string? FileUrl_Ratio_1_4 { get; set; }

        // Medyanın sekizde bir boyutlu optimize edilmiş sürümünün URL adresini belirtir.
        [MaxLength(2048)]
        public string? FileUrl_Ratio_1_8 { get; set; }

        // Medyanın on altıda bir boyutlu optimize edilmiş sürümünün URL adresini belirtir.
        [MaxLength(2048)]
        public string? FileUrl_Ratio_1_16 { get; set; }

        // İleride yeni boyutlar eklendiğinde migration gerektirmeden kullanılabilecek ek medya sürümlerinin JSON bilgisini tutar.
        public string? RenditionsJson { get; set; }

        // Medyanın orijinal dosya boyutunu byte cinsinden belirtir.
        public long? OriginalSize { get; set; }

        // Medyanın aktif olarak kullanılan sıkıştırılmış veya optimize edilmiş dosya boyutunu byte cinsinden belirtir.
        public long? CompressedSize { get; set; }

        // Medyanın değişmediğini doğrulamak ve aynı dosyanın tekrar yüklenmesini tespit etmek için kullanılan SHA-256 özetidir.
        [MaxLength(64)]
        public string? Sha256 { get; set; }

        // Medyanın depolama katmanındaki ETag veya benzeri bütünlük bilgisini tutar.
        [MaxLength(255)]
        public string? ETag { get; set; }

        // Görsel veya videonun yatay piksel genişliğini belirtir.
        public int? Width { get; set; }

        // Görsel veya videonun dikey piksel yüksekliğini belirtir.
        public int? Height { get; set; }

        // Video veya ses medyasının toplam süresini milisaniye cinsinden belirtir.
        public long? DurationMs { get; set; }

        // Video medyasının saniyedeki kare sayısını belirtir.
        public decimal? FrameRate { get; set; }

        // Video veya ses medyasının bitrate değerini bit/saniye cinsinden belirtir.
        public long? Bitrate { get; set; }

        // Video dosyasının kullandığı codec bilgisini belirtir.
        [MaxLength(100)]
        public string? Codec { get; set; }

        // Çok sayfalı dokümanlarda toplam sayfa sayısını belirtir.
        public int? PageCount { get; set; }

        // Görselin şeffaf kanal içerip içermediğini belirtir.
        public bool? HasAlpha { get; set; }

        // Görselin EXIF veya işleme sonucunda belirlenen yön bilgisini tutar.
        public short? Orientation { get; set; }

        // Görselin düşük çözünürlüklü placeholder veya BlurHash benzeri önizleme bilgisini tutar.
        [MaxLength(512)]
        public string? BlurHash { get; set; }

        // Görselin baskın renk bilgisini tutar ve hızlı placeholder veya tasarım işlemlerinde kullanılabilir.
        [MaxLength(32)]
        public string? DominantColor { get; set; }

        // Video, YouTube, Vimeo veya başka harici medya sağlayıcılarının URL adresini tutar.
        [MaxLength(2048)]
        public string? ExternalUrl { get; set; }

        // Harici medya sağlayıcısının adını belirtir ve YouTube, Vimeo veya TikTok gibi değerler içerebilir.
        [MaxLength(100)]
        public string? ExternalProvider { get; set; }

        // Medyanın işleme durumunu belirtir ve Pending, Processing, Ready veya Failed gibi değerler içerebilir.
        [MaxLength(50)]
        public string? ProcessingStatus { get; set; }

        // Medya işleme başarısız olduğunda alınan hata mesajını tutar.
        public string? ProcessingError { get; set; }

        // Medyanın AI veya otomatik içerik işleme süreçlerinden geçirilip geçirilmediğini belirtir.
        public bool? AiAvif { get; set; }

        // Medyanın kullanıcı tarafından erişilebilir ve gösterilebilir durumda olup olmadığını belirtir.
        public bool IsPublic { get; set; } = true;

        // Medyanın oluşturulma tarihini UTC olarak belirtir.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Medyanın son teknik olarak güncellendiği tarihi UTC olarak belirtir.
        public DateTime? UpdatedAt { get; set; }

        // Medyanın işleme sürecinin tamamlandığı tarihi UTC olarak belirtir.
        public DateTime? ProcessedAt { get; set; }

        // Medyanın silinme durumunu ve soft-delete bilgilerini tutar.
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}