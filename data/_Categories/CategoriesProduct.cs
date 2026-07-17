using data.Owned;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Net.WebRequestMethods;

namespace data._Categories
{
    public class CategoriesProduct
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Üst kategori ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Aktif / Pasif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Menüde gösterilsin mi?
        /// </summary>
        public bool ShowInMenu { get; set; } = true;


        /// <summary>
        //        [SİSTEM ROLÜ]
        //        Sen dünya çapında uzman bir e-ticaret katalog ve ürün veri(taxonomi) mühendisisin.Tek bir uzmanlık alanın ve tek bir görevin var: Sana verilen ürün kategori yolunu analiz ederek, o kategoriye ait ürünlerin sahip olabileceği TÜM özellikleri(attributes) eksiksiz bir şekilde listelemek.


        //        [GÖREVİN]
        //Sana "Kategori Yolu" formatında bir girdi verilecek. Bu girdiyi kullanarak, o kategoride satılacak bir ürün için veri tabanında tutulması ve satıcı tarafından doldurulması gereken tüm teknik, fiziksel, fonksiyonel ve ticari özellikleri oluşturacaksın.


        //        [KURALLAR VE BEKLENTİLER - DİKKATLE UYGULA]
        //        1. KAPSAM VE EKSİKSİZLİK: Yalnızca temel veya yaygın özelliklerle yetinmek kesinlikle yasaktır. Kategoriye özel en ince detaya kadar inmelisin.
        //        2. ÖRNEK REFERANS: Örneğin girdi "Elektronik > Telefon > Akıllı Telefon" ise; sadece "Ekran Boyutu, Batarya, Renk" ile yetinemezsin.Aşağıdaki gibi eksiksiz bir yapı oluşturmalısın:
        //"İşlemci Mimarisi, CPU Frekansı, RAM Tipi, Dahili Depolama, Ekran Teknolojisi (OLED/AMOLED vb.), Ekran Yenileme Hızı (Hz), Piksel Yoğunluğu (PPI), Arka Kamera Çözünürlükleri, OIS Desteği, Ön Kamera Çözünürlüğü, Video Kayıt Çözünürlüğü (4K/8K), İşletim Sistemi Sürümü, Ağ Desteği (5G/4G), SIM Türü (Nano/eSIM), Wi-Fi Standardı, Bluetooth Versiyonu, NFC Desteği, GPS, USB Bağlantı Tipi, Parmak İzi Okuyucu, Su ve Toz Dayanıklılık Sertifikası (IP68 vb.), Batarya Kapasitesi (mAh), Hızlı Şarj Gücü (W), Kablosuz Şarj Desteği, Kasa Materyali, Ağırlık (gr), Boyutlar."
        //3. HEDEF: Ürettiğin özellikler, o kategorideki bir ürünün tüm anatomisini ortaya çıkarmalıdır. Atlanmış hiçbir potansiyel özellik kalmamalıdır.


        //        [KESİNLİKLE YAPILMAYACAKLAR(KATI YASAKLAR)]
        //        Aşağıdaki işlemlerin yapılması kesinlikle YASAKTIR. Sadece attribute listesi üretilecektir:
        //- Ürün açıklaması (description) YAZILMAYACAK.
        //- SEO metni, başlık, anahtar kelime veya etiket ÜRETİLMEYECEK.
        //        - Ürün varyasyonu (variation), filtre tasarımı veya SKU kurgusu YAPILMAYACAK.
        //- "İşte özellikler", "Anladım", "Başka sorunuz var mı?" gibi hiçbir giriş, çıkış, onay veya sohbet cümlesi KULLANILMAYACAK.


        //        [GİRDİ]
        //        Kategori Yolu: { BURAYA_KATEGORİ_YOLU_GELECEK}

        //        [ÇIKTI]
        //        Sadece özellikleri içeren, temiz, yapılandırılmış liste:        
        /// </summary>
        public string? Path { get; set; } = string.Empty;

        /// <summary>
        /// AI tarafından oluşturulan özelliklerin tamamlanıp tamamlanmadığını belirten bir bayrak. Eğer AI tarafından oluşturulan özellikler oluşturuldu ise true olarak işaretlenir ve bu, sistemin bu özellikleri kullanmaya hazır olduğunu gösterir. döngü içerisinde ise != true olarak kontrol edilir. Ve true olmayanlar işlenir.
        /// </summary>
        public bool? AiAttributesIsOk { get; set; } = null; 


        /// <summary>
        /// Dil bilgileri ve SEO URL bilgilerini içeren Categories Owned sınıfı
        /// </summary>
        public Categories? Categories { get; set; } = new();

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Güncellenme tarihi
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
