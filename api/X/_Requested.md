AddProduct.razor dosyası bir ürün yükleme sayfası olacak. Bu AddProduct.razor yani ürün yükleme sayfasını oluştururken şunlara dikkat etmeli:

Varyant Sistemi



Ürün varyant sistemi marketplace standartlarında olmalıdır.



Örnek:



Ürün: Kazak



Özellikler:



Renk

Kırmızı

Bordo

Siyah

Beden

S

M



Oluşabilecek varyantlar:



SKU	Renk	Beden

KZK-KRM-S	Kırmızı	S

KZK-KRM-M	Kırmızı	M

KZK-BRD-S	Bordo	S

KZK-BRD-M	Bordo	M

KZK-SYH-S	Siyah	S

KZK-SYH-M	Siyah	M

Varyant Görsel Yönetimi



Bu bölüm kritik öneme sahiptir.



Sistem tüm varyant kombinasyonları için ayrı görsel üretmeye zorlamamalıdır.



Örneğin:



Renk:



Kırmızı

Bordo

Siyah



Beden:



S

M



Bu senaryoda yalnızca:



Kırmızı

Bordo

Siyah



değerlerine görsel atanabilmelidir.



Yani:



Kırmızı S

Kırmızı M



aynı görsel grubunu kullanabilmelidir.



Aynı şekilde sistem yöneticisi isterse:



Beden



özelliğini görsel varyantı olarak tanımlayabilmelidir.



Özetle:



Her varyant özelliği için aşağıdaki yapı desteklenmelidir:



Görsel oluşturan özellik

Seçenek olarak kalan özellik



Bu seçim ürün bazında yapılabilmelidir.



Bir ürün için "Renk" görsel varyantı iken başka bir ürün için "Beden" görsel varyantı olabilmelidir.



Bir varyant değerine birden fazla görsel atanabilmelidir.



Medya ve Galeri Yapısı



Ürün:



Kapak görseli

Galeri görselleri

Varyant görselleri

Video URL'leri

360 derece medya

PDF/Doküman



destekleyebilmelidir.



Media.cs yapısı kullanılmalıdır.



Galeri yönetimi ItemGallery.cs üzerinden ilişkilendirilebilmelidir. Ve Galeri resimlerinin sırası değiştirilebilmeli veya galeriden seçilen herhangi biri kaldırılabilmelidir. 



Özellik Şablonları (Attribute Templates)



Özellik sistemi iki ayrı yapıdan oluşmalıdır:



1. Sistem Özellik Şablonları (Admin Yönetimli)



Bu yapı yalnızca sistem yöneticileri tarafından oluşturulabilir ve yönetilebilir.



Amaç; belirli ürün grupları için standart özellik kümeleri oluşturmaktır.



Örnek:



Televizyon



Ekran Boyutu

Çözünürlük

Panel Türü

Yenileme Hızı

Smart TV

İşletim Sistemi



Telefon



RAM

Depolama

İşlemci

Kamera

Batarya Kapasitesi



Laptop



İşlemci

RAM

Depolama

Ekran Boyutu

Ekran Kartı



Bu şablonlar merkezi olarak yönetilmeli ve satıcılar tarafından değiştirilememelidir.



Bir şablon üzerinde yapılan değişiklikler yalnızca yetkili yöneticiler tarafından yapılabilmelidir.



2. Satıcıya Özel Özellikler (Custom Attributes)



Satıcılar ürün oluştururken:



Sistem tarafından sunulan özellik şablonlarını kullanabilmelidir.

Ayrıca ihtiyaç duyduklarında kendi özel özelliklerini oluşturabilmelidir.



Örnek:



Admin tarafından oluşturulmuş Televizyon şablonu:



Ekran Boyutu

Çözünürlük

Panel Türü



Satıcı ürün oluştururken:



Ekran Boyutu → 55"

Çözünürlük → 4K

Panel Türü → OLED



değerlerini girebilir.



Ayrıca kendi ürününe özel olarak:



Kutu İçeriği

Montaj Tipi

Garanti Kapsamı

Boyut



gibi yeni özellikler de ekleyebilir.



Kritik Kural



Satıcının oluşturduğu özel özellikler hiçbir şekilde sistem şablonlarını etkilememelidir.



Örneğin:



Admin tarafından oluşturulmuş:



Televizyon



Ekran Boyutu

Çözünürlük

Panel Türü



şablonu bulunuyorsa,



bir satıcının:



Boyut

Kasa Kalınlığı

Duvar Aparatı Dahil



gibi özellikler eklemesi;



Televizyon şablonuna eklenmemeli,

Diğer satıcıların ürünlerinde görünmemeli,

Sistem genelinde ortak özellik haline gelmemelidir.



Bu özellikler yalnızca ilgili ürün veya ilgili satıcının ürünleri için geçerli olmalıdır.



Mevcut sistemimde aşağıdaki yapılar hazır durumdadır ve oluşturulacak yapıların bunlarla uyumlu çalışması gerekmektedir.



Mevcut Yapılar

Users.cs → Kullanıcı kayıtları

Store.cs → Mağaza kayıtları

CategoriesTr.cs → Kategori kayıtları

Media.cs → Dosya ve medya kayıtları

ItemGallery.cs → Galeri yönetimi

Warehouse.cs → Depo kayıtları



Yeni oluşturulacak ürün yapıları bu mevcut yapılar ile ilişkilendirilmelidir.Amaç, satıcıların ürün oluşturabileceği profesyonel bir ürün yönetim altyapısı oluşturmaktır.



Oluşturulacak yapı:



Küçük ölçekli satıcıları,

Büyük mağazaları,

Çok varyantlı ürünleri,

Gelecekteki marketplace entegrasyonlarını,



destekleyebilecek seviyede esnek ve sürdürülebilir olmalıdır.Her ürün aşağıdaki yapılar ile ilişkilendirilebilmelidir.



Kullanıcı



Ürünün sahibi olan kullanıcı.



İlişki:



Users.cs



Mağaza



Ürünün ait olduğu mağaza.



İlişki:



Store.cs



Kategori



Ürün en az bir kategoriye bağlı olmalıdır.



Ayrıca ürünün birden fazla kategori ile ilişkilendirilebilmesi desteklenmelidir.



İlişki:



CategoriesTr.cs



Ürün bir depoya bağlanabilmelidir.



İlişki:



Warehouse.cs



Çoklu depo desteği eklenebilecek şekilde tasarlanmalıdır.



Ürün Tipleri



Sistem aşağıdaki ürün tiplerini desteklemelidir.



Basit Ürün

Varyantlı Ürün



Varyant Görsel Sistemi



Bu bölüm kritik öneme sahiptir.



Sistem her kombinasyon için ayrı görsel oluşturmaya zorlamamalıdır.



Örnek:



Renk



Kırmızı

Bordo

Siyah



Beden



S

M



Satıcı isterse yalnızca:



Kırmızı

Bordo

Siyah



varyant değerlerine görsel atayabilmelidir.



Böylece:



Kırmızı S

Kırmızı M



aynı görsel grubunu kullanabilmelidir.



Aynı mantıkla başka bir üründe:



Beden



özelliği görsel varyantı olarak seçilebilmelidir.



Bu nedenle:



Her varyant özelliği için;



Görsel oluşturan özellik

Sadece seçenek olarak kullanılan özellik



ayrımı yapılabilmelidir.



Bu seçim ürün bazında yapılabilmelidir.



Bir varyant değerine birden fazla görsel atanabilmelidir.



Stok Yönetimi



Stok takibi varyant bazında yapılabilmelidir.



Örnek:



Kırmızı / S → 20

Kırmızı / M → 15

Bordo / S → 8



Her varyant için mümkün ise:



Stok Miktarı

Minimum Stok

Kritik Stok

Maksimum Sipariş Adedi

alanları bulunmalıdır.

Tek dosya (AddProduct.razor); proje, başka hiçbir dosyaya dokunulmadan derlenir ve çalışır.

Satıcı özel özelliğinin UserId/StoreId dolu yazıldığının, hiçbir şablon tablosuna ve başka satıcının ekranına yansımadığının doğrulanması.

Şablonlu kategoride şablon alanlarının DisplayOrder/IsRequired kurallarıyla geldiği; şablonsuz kategoride sihirbazın sorunsuz çalıştığı.
Aynı medyanın birden fazla slotta seçilebilmesi; galeri modalının yeniden açılışında ön-işaretli seçim (HashSet ID karşılaştırması) davranışı.

Hata durumunda transaction rollback + veri kaybı olmadan sihirbazda kalma; başarı durumunda log + cache temizliği + toaster + yönlendirme.

Önemli: GeminiAiService yapısını öyle bir uyarla ve içerisine, ürün ekleme konusunda öyle bir entegre metodlar ekle ki çok optimize ve efektif çalışsın. Yani örneğin: ürün adı girildiğinde ürün kısa açıklama alanında bir AI butonu olsun ve ona tıklandığında, ürün kısa açıklama ürün aranma potansiyeli en yüksek metinler ile veürün ile alakalı metinler ile anlamlı bir ürün kısa açıklaması olmalı.

Mevcut GeminiAiService mimarisini ilk olarak tamamen temizle ve yeniden yapılandırılmaya hazır hale getir, ürün ekleme süreçlerine tam entegre, yüksek performanslı ve optimize çalışacak şekilde yeniden yapılandır.

Örnek Kullanım Senaryosu: Ürün ekleme panelinde, 'Ürün Adı' girildikten sonra 'Kısa Açıklama' alanının yanında bir 'AI ile Üret' butonu yer almalıdır. Bu butona tıklandığında servis devreye girmeli; girilen ürün adını baz alarak, arama motoru optimizasyonuna (SEO) uygun, yüksek aranma potansiyeline sahip anahtar kelimelerle zenginleştirilmiş, ürünle tam uyumlu ve anlamlı bir kısa açıklama metni oluşturmalıdır. 
Gibi AI kullanılabilecek alanları AI servisi ileyapabilme opsiyonu olmuş olsun. Kullanıcı ilgili alan ileilgili AI butonunu tıklar isehali hazırdaki alan AI ile hazırlanmalı. Veya kullanıcı Direk manuel olarak kendiri oluşturabilmeli.

Not: mevcut AI api servisi bilgileri ile işlemler yürütülecektir.