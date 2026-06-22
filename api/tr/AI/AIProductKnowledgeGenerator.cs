using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace api
{
    public class AIProductKnowledgeGenerator
    {
        private readonly string _apiKey = "AQ.Ab8RN6Il8ODEGLl5GSn9vYaoGs1WRIQJYUKqxdSP8pI18udrJQ";
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIProductKnowledgeGenerator> _logger;

        // DI ile IHttpClientFactory → HttpClient enjeksiyonu
        public AIProductKnowledgeGenerator(HttpClient httpClient, ILogger<AIProductKnowledgeGenerator> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // AI çıktısını map'lemek için model
        public class ProductSeoData
        {
            public string Title { get; set; } = "";
            public string ShortDesc { get; set; } = "";
            public string LongDesc { get; set; } = "";
            public string Keywords { get; set; } = "";
        }

        public async Task<ProductSeoData?> GenerateSeoContentAsync(
            string productName,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                _logger.LogWarning("AI içerik üretimi atlandı — ürün adı boş.");
                return null;
            }

            string prompt = $@"
Sen 50+ yıllık deneyime sahip profesyonel bir e-ticaret içerik editörü, SEO uzmanı, ürün metni yazarı ve dijital pazarlama danışmanısın.

Görevin aşağıdaki ürün adı için yüksek dönüşüm oranına sahip, tamamen özgün, insan tarafından yazılmış gibi doğal görünen profesyonel ürün içerikleri oluşturmaktır.

Ürün Adı:
{productName}

KURALLAR:

1. İçerik tamamen Türkçe olmalıdır.
2. Ürün başlığı SEO uyumlu, profesyonel ve satış odaklı olmalıdır.
3. Başlık gereksiz tekrar içermemelidir.
4. Kısa açıklama 1-3 cümle arasında olmalıdır.
5. Kısa açıklama müşteriyi satın almaya yönlendirmelidir.
6. Uzun açıklama minimum 300 kelime olmalıdır.
7. Uzun açıklama akıcı, doğal ve profesyonel olmalıdır.
8. Uzun açıklama yapay zeka tarafından yazılmış gibi görünmemelidir.
9. Uzun açıklama içerisinde ürünün sağlayacağı faydalar ön planda tutulmalıdır.
10. Gerektiğinde kullanım alanları, avantajlar ve satın alma sebepleri anlatılmalıdır.
11. Anahtar kelimeler doğal şekilde metne dağıtılmalıdır.
12. Keyword alanında yüksek aranma potansiyeline sahip SEO anahtar kelimeleri virgülle ayrılmış şekilde verilmelidir.
13. Metin içerisinde emoji kullanma.
14. Metin içerisinde markdown kullanma.
15. Metin içerisinde yıldız (*), çift yıldız (**), alt çizgi (_), hashtag (#), madde işareti (-), bullet (•), backtick (`), HTML etiketi veya özel biçimlendirme kullanma.
16. JSON dışına çıkma.
17. Açıklama, giriş cümlesi, not veya ek bilgi yazma.
18. 'İşte JSON', 'Sonuç', 'Açıklama', 'Not' gibi ifadeler kullanma.
19. Çıktı yalnızca geçerli JSON olmalıdır.
20. JSON içerisindeki tüm metinler kaçış karakterleri açısından geçerli olmalıdır.
21. Ürün hakkında kesin olarak bilinmeyen teknik özellikler uydurma.
22. Marka, model, ölçü, kapasite veya teknik veri ürün adında geçmiyorsa varsayım yapma.
23. Ürün açıklamasını gerçek bir e-ticaret editörü yazıyormuş gibi oluştur.
24. İçerik hem SEO hem dönüşüm optimizasyonu açısından üst düzey kalitede olmalıdır.
25. İçerik tamamen evergreen (zamandan bağımsız) olmalıdır.
26. Yıl, tarih, dönem, sezon veya zamana bağlı ifadeler kullanma.
27. ""2026"", ""2027"", ""Yeni sezon"", ""Bu yıl"", ""Günümüzde"", ""Son dönemde"", ""En yeni"", ""En güncel"", ""Trend"", ""Trend ürün"", ""Yılın ürünü"" gibi ifadeler kullanma.
28. İçeriğin gelecekte de geçerliliğini korumasını sağla.
29. Ürün adında açıkça belirtilmeyen hiçbir teknik özellik üretme.
30. Ürün adında bulunmayan marka bilgisi ekleme.
31. Ürün adında bulunmayan model bilgisi ekleme.
32. Ürün adında bulunmayan ölçü, ebat, hacim, kapasite veya ağırlık bilgisi ekleme.
33. Ürün adında bulunmayan malzeme bilgisi ekleme.
34. Emin olmadığın hiçbir bilgiyi yazma.
35. Varsayım yapma.
36. Tahminde bulunma.
37. Teknik veri uydurma.
38. Özellik uydurma.
39. Sertifika, garanti, lisans veya kalite standardı bilgisi uydurma.
40. Kesin olarak doğrulanamayan hiçbir iddiada bulunma.
41. Ürünün faydalarını anlatırken yalnızca ürün isminden mantıksal olarak çıkarılabilen bilgiler kullan.
42. Şüpheli durumlarda teknik özellik vermek yerine genel kullanım faydalarına odaklan.
43. İçerik güvenilir, profesyonel ve gerçek bir editör tarafından hazırlanmış gibi görünmelidir.
44. Abartılı, gerçek dışı veya ispatlanamaz pazarlama ifadeleri kullanma.
45. ""En iyi"", ""Rakipsiz"", ""Mükemmel"", ""Bir numara"", ""Piyasadaki en güçlü"" gibi doğrulanamayan üstünlük iddialarından kaçın.
46. Ürünün kesin olarak sağlayamayacağı sonuçları vaat etme.
47. Tüm içerik dürüst, gerçekçi ve ticari kullanıma uygun olmalıdır.

DÖNDÜRÜLECEK FORMAT:

{{
  ""Title"": ""..."",
  ""ShortDesc"": ""..."",
  ""LongDesc"": ""..."",
  ""Keywords"": ""...""
}}

SADECE GEÇERLİ JSON DÖNDÜR.
";

            string requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    temperature = 0.7
                }
            };

            string jsonPayload = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                // 30 sn timeout — background servis bloke olmasın
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

                HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content, timeoutCts.Token);
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync(timeoutCts.Token);

                using JsonDocument doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                // Gemini "candidates" array → ilk candidate → content → parts[0] → text
                if (!root.TryGetProperty("candidates", out var candidates)
                    || candidates.GetArrayLength() == 0)
                {
                    _logger.LogWarning("Gemini boş candidates döndürdü → {Product}", productName);
                    return null;
                }

                string aiResponseText = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    _logger.LogWarning("Gemini boş text döndürdü → {Product}", productName);
                    return null;
                }

                var result = JsonSerializer.Deserialize<ProductSeoData>(aiResponseText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Minimal doğrulama — Title boşsa geçersiz say
                if (result == null || string.IsNullOrWhiteSpace(result.Title))
                {
                    _logger.LogWarning("AI çıktısı parse edilemedi veya Title boş → {Product}", productName);
                    return null;
                }

                _logger.LogInformation("AI içerik başarıyla üretildi → {Product}", productName);
                return result;
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // Timeout (30 sn aşıldı) — uygulama kapanması değil
                _logger.LogWarning("AI isteği zaman aşımına uğradı (30s) → {Product}", productName);
                return null;
            }
            catch (OperationCanceledException)
            {
                // Uygulama kapanıyor — yukarı fırlat
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Gemini API HTTP hatası → {Product}", productName);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "AI yanıtı JSON parse hatası → {Product}", productName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI içerik üretiminde beklenmeyen hata → {Product}", productName);
                return null;
            }
        }
    }
}
