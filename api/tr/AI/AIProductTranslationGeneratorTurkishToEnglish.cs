using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace api.tr.AI
{
    public class AIProductTranslationGeneratorTurkishToEnglish
    {
        private readonly string _apiKey = "AQ.Ab8RN6Il8ODEGLl5GSn9vYaoGs1WRIQJYUKqxdSP8pI18udrJQ";
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIProductTranslationGeneratorTurkishToEnglish> _logger;

        // DI ile IHttpClientFactory → HttpClient enjeksiyonu
        public AIProductTranslationGeneratorTurkishToEnglish(HttpClient httpClient, ILogger<AIProductTranslationGeneratorTurkishToEnglish> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // AI çeviri çıktısını map'lemek için model
        public class ProductTranslationData
        {
            public string? Name { get; set; }
            public string? ShortDescription { get; set; }
            public string? FullDescription { get; set; }
            public string? Tags { get; set; }
        }

        /// <summary>
        /// tr içerikleri hedef dile, anlam bütünlüğünü bozmadan, minimum token ile çevirir.
        /// Boş/null gelen alanlar boş bırakılır — gereksiz token harcanmaz.
        /// </summary>
        public async Task<ProductTranslationData?> TranslateAsync(
            string targetLanguageCode,
            string? name,
            string? shortDescription,
            string? fullDescription,
            string? tags,
            CancellationToken cancellationToken = default)
        {
            // Çevrilecek hiçbir şey yoksa AI'ı hiç çağırma
            if (string.IsNullOrWhiteSpace(name)
                && string.IsNullOrWhiteSpace(shortDescription)
                && string.IsNullOrWhiteSpace(fullDescription)
                && string.IsNullOrWhiteSpace(tags))
            {
                _logger.LogWarning("Çeviri atlandı — tüm alanlar boş.");
                return null;
            }

            // Yalnızca dolu alanları JSON'a koy → minimum token
            var sourceObj = new
            {
                Name = name ?? "",
                ShortDescription = shortDescription ?? "",
                FullDescription = fullDescription ?? "",
                Tags = tags ?? ""
            };
            string sourceJson = JsonSerializer.Serialize(sourceObj);

            string prompt = $@"Sen profesyonel bir e-ticaret çevirmenisin.
Aşağıdaki JSON içindeki Türkçe ürün metinlerini ""{targetLanguageCode}"" dil koduna çevir.

KURALLAR:
1. Anlam bütünlüğünü koru, doğal ve akıcı çevir.
2. Alan yapısını AYNEN koru: Name, ShortDescription, FullDescription, Tags.
3. Boş gelen alanı boş bırak.
4. Tags virgülle ayrılmış anahtar kelimelerdir; her birini hedef dile çevir, virgül düzenini koru.
5. Marka, model ve ölçü gibi çevrilmemesi gereken özel adları olduğu gibi bırak.
6. Açıklama, not, giriş cümlesi ekleme.
7. Markdown, emoji veya biçimlendirme kullanma.
8. SADECE geçerli JSON döndür, JSON dışına çıkma.
9. Kaçış karakterleri geçerli olmalı.

ÇEVRİLECEK JSON:
{sourceJson}

DÖNDÜRÜLECEK FORMAT:
{{
  ""Name"": ""..."",
  ""ShortDescription"": ""..."",
  ""FullDescription"": ""..."",
  ""Tags"": ""...""
}}

SADECE GEÇERLİ JSON DÖNDÜR.";

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
                    temperature = 0.3   // çeviri → düşük yaratıcılık, yüksek sadakat
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

                if (!root.TryGetProperty("candidates", out var candidates)
                    || candidates.GetArrayLength() == 0)
                {
                    _logger.LogWarning("Gemini boş candidates döndürdü → {Lang}", targetLanguageCode);
                    return null;
                }

                string aiResponseText = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    _logger.LogWarning("Gemini boş text döndürdü → {Lang}", targetLanguageCode);
                    return null;
                }

                var result = JsonSerializer.Deserialize<ProductTranslationData>(aiResponseText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null)
                {
                    _logger.LogWarning("AI çeviri çıktısı parse edilemedi → {Lang}", targetLanguageCode);
                    return null;
                }

                _logger.LogInformation("AI çeviri başarıyla üretildi → {Lang}", targetLanguageCode);
                return result;
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("AI çeviri isteği zaman aşımına uğradı (30s) → {Lang}", targetLanguageCode);
                return null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Gemini API HTTP hatası → {Lang}", targetLanguageCode);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "AI çeviri yanıtı JSON parse hatası → {Lang}", targetLanguageCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI çeviride beklenmeyen hata → {Lang}", targetLanguageCode);
                return null;
            }
        }
    }
}