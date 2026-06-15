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
Aşağıdaki ürün için e-ticaret sitesine uygun SEO odaklı içerikler üret. 
Ürün: '{productName}'

Lütfen sadece ve sadece aşağıdaki formata uygun geçerli bir JSON döndür. Başka hiçbir açıklama metni, selamlama veya markdown işareti ekleme:
{{
    ""Title"": ""SEO uyumlu yeni ve temiz ürün başlığı"",
    ""ShortDesc"": ""1-3 cümlelik vurucu ve satış odaklı kısa açıklama"",
    ""LongDesc"": ""Detaylı, satış odaklı ve SEO uyumlu en az 300 kelimelik uzun açıklama"",
    ""Keywords"": ""virgülle ayrılmış, aranma hacmi yüksek anahtar kelimeler""
}}";

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
