using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace api.tr.AI
{
    public class AIPostContentCreator
    {
        private readonly string _apiKey = "AQ.Ab8RN6Il8ODEGLl5GSn9vYaoGs1WRIQJYUKqxdSP8pI18udrJQ";
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIPostContentCreator> _logger;

        // Kelime sayısı eşiği — çok katı tutmak gereksiz retry'lara yol açar
        private const int MinWordCount = 300;

        public AIPostContentCreator(HttpClient httpClient, ILogger<AIPostContentCreator> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public class ArticleSeoData
        {
            public string Title { get; set; } = "";
            public string ShortDescription { get; set; } = "";
            public string ContentHtml { get; set; } = "";
            public string MetaTitle { get; set; } = "";
            public string MetaDescription { get; set; } = "";
            public string FocusKeywords { get; set; } = "";
        }

        // Sonuç zarfı — başarısızlık durumunda GERÇEK sebebi taşır.
        // Çağıran taraf artık "boş" bir null yerine ne olduğunu bilir.
        public class ArticleGenerationResult
        {
            public bool Success { get; set; }
            public ArticleSeoData? Data { get; set; }
            public string? FailureReason { get; set; }

            public static ArticleGenerationResult Ok(ArticleSeoData data) =>
                new() { Success = true, Data = data };

            public static ArticleGenerationResult Fail(string reason) =>
                new() { Success = false, FailureReason = reason };
        }

        public async Task<ArticleGenerationResult> GenerateArticleContentAsync(
            string title,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning("AI makale üretimi atlandı — başlık boş.");
                return ArticleGenerationResult.Fail("Başlık boş geldi.");
            }

            string prompt = $@"
Sen 50+ yıllık deneyime sahip, saygın bir yayın grubunda çalışmış kıdemli bir editör, SEO uzmanı ve içerik stratejistisin.

Görevin aşağıdaki başlık için tamamen özgün, profesyonel, insan tarafından yazılmış gibi doğal görünen, yayına hazır bir blog/makale içeriği oluşturmaktır.

Başlık:
{title}

KURALLAR — İÇERİK:

1. İçerik tamamen Türkçe olmalıdır.
2. ContentHtml alanı minimum 500 kelime olmalıdır.
3. ContentHtml akıcı, doğal, profesyonel bir editör üslubuyla yazılmalıdır; yapay zeka tarafından yazılmış gibi görünmemelidir.
4. İçerik konuyu gerçekten bilen birinin yazdığı izlenimini vermelidir; okuyucuya somut değer sunmalıdır.
5. Konuya uygun mantıklı bir yapı kur: giriş, en az 3 alt başlık altında gövde, kısa bir kapanış.
6. Anahtar kelimeler metne doğal şekilde, zorlama hissettirmeden dağıtılmalıdır.
7. Emoji kullanma.
8. Markdown kullanma (yıldız, çift yıldız, alt çizgi, hashtag, madde imi tire, backtick yasak).

KURALLAR — HTML FORMATI:

9. ContentHtml SADECE bir HTML parçası (fragment) olmalıdır.
10. ContentHtml <html>, <head>, <body>, <script>, <style>, <link>, <meta> etiketlerini KESİNLİKLE içermemelidir.
11. ContentHtml doğrudan <p>...</p> ile başlamalıdır, başka hiçbir şeyle değil.
12. Sadece şu etiketler kullanılabilir: h2, h3, p, ul, ol, li, strong, em, blockquote, table, thead, tbody, tr, th, td.
13. İzin verilen yerlerde Bootstrap 5 utility class'ları kullanılabilir: örn. class=""lead"" (giriş paragrafı için), class=""text-muted"", class=""table table-striped"".
14. <img> etiketi ve inline style=""..."" KULLANMA.
15. Tüm HTML etiketleri düzgün açılıp kapatılmalıdır, geçersiz/kırık HTML üretme.

KURALLAR — DOĞRULUK VE GÜVENİLİRLİK:

16. Kesin olarak bilinmeyen istatistik, tarih, oran, isim, kurum veya araştırma uydurma.
17. Doğrulanamayan sayısal veri, yüzde veya rakam verme.
18. ""En iyi"", ""Rakipsiz"", ""Bilimsel olarak kanıtlanmıştır"" gibi ispatlanamaz iddialardan kaçın.
19. Emin olmadığın hiçbir bilgiyi kesin bir dille sunma; gerekirse genel geçer, tartışmasız bilgiye dayan.
20. İçerik tamamen evergreen (zamandan bağımsız) olmalıdır; yıl, tarih, sezon, ""bu yıl"", ""güncel"", ""son dönemde"", ""trend"" gibi zamana bağlı ifadeler kullanma.

KURALLAR — SEO ALANLARI:

21. Title: SEO uyumlu, ilgi çekici, 70 karakteri geçmeyen bir başlık (girdideki başlığı iyileştirebilirsin, anlamını değiştirme).
22. ShortDescription: 1-3 cümlelik, okuyucuyu içeriği okumaya teşvik eden bir özet.
23. MetaTitle: Google'da görünecek, 60 karakteri geçmeyen başlık.
24. MetaDescription: Arama sonucu açıklaması, 150-160 karakter aralığında, tıklamayı teşvik edici.
25. FocusKeywords: virgülle ayrılmış, konuyla doğrudan ilgili 5-10 SEO anahtar kelimesi.

ÇIKTI KURALLARI:

26. JSON dışına çıkma. Açıklama, giriş cümlesi, not veya ek bilgi yazma.
27. 'İşte JSON', 'Sonuç', 'Not' gibi ifadeler kullanma. Markdown kod bloğu (```) kullanma.
28. Çıktı yalnızca geçerli JSON olmalıdır, tüm metinler kaçış karakterleri açısından geçerli olmalıdır.

DÖNDÜRÜLECEK FORMAT:

{{
  ""Title"": ""..."",
  ""ShortDescription"": ""..."",
  ""ContentHtml"": ""..."",
  ""MetaTitle"": ""..."",
  ""MetaDescription"": ""..."",
  ""FocusKeywords"": ""...""
}}

SADECE GEÇERLİ JSON DÖNDÜR.
";

            string requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    temperature = 0.7,
                    maxOutputTokens = 8192
                }
            };

            string jsonPayload = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                // Uzun içerik üretimi zaman alabilir — timeout 90 sn
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(90));

                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync(requestUrl, content, timeoutCts.Token);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Gemini API'ye bağlanılamadı → {Title}", title);
                    return ArticleGenerationResult.Fail($"API bağlantı hatası: {ex.Message}");
                }

                string responseJson = await response.Content.ReadAsStringAsync(timeoutCts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    // Gemini genelde hata gövdesinde açıklayıcı bir mesaj döner (kota, geçersiz key, vs.)
                    string snippet = responseJson.Length > 400 ? responseJson[..400] : responseJson;
                    _logger.LogError(
                        "Gemini API HTTP {Status} döndü → {Title} | Gövde: {Body}",
                        (int)response.StatusCode, title, snippet);
                    return ArticleGenerationResult.Fail($"API HTTP {(int)response.StatusCode}: {snippet}");
                }

                using JsonDocument doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                {
                    // "candidates" boşsa çoğunlukla safety filtrelerine (promptFeedback) takılmıştır
                    string reason = "Gemini boş candidates döndürdü.";
                    if (root.TryGetProperty("promptFeedback", out var feedback))
                    {
                        reason += $" promptFeedback: {feedback.GetRawText()}";
                    }
                    _logger.LogWarning("{Reason} → {Title}", reason, title);
                    return ArticleGenerationResult.Fail(reason);
                }

                var firstCandidate = candidates[0];

                // finishReason == MAX_TOKENS ise içerik yarım kesilmiş demektir — bunu ayrı teşhis edelim
                if (firstCandidate.TryGetProperty("finishReason", out var finishReasonEl))
                {
                    string? finishReason = finishReasonEl.GetString();
                    if (!string.IsNullOrEmpty(finishReason) && finishReason != "STOP")
                    {
                        _logger.LogWarning(
                            "Gemini normal dışı finishReason ile döndü → {Title} | finishReason: {FinishReason}",
                            title, finishReason);

                        if (finishReason == "MAX_TOKENS")
                        {
                            return ArticleGenerationResult.Fail("İçerik token limitine takıldı (yarım kesildi). maxOutputTokens artırılmalı.");
                        }
                        if (finishReason == "SAFETY" || finishReason == "RECITATION")
                        {
                            return ArticleGenerationResult.Fail($"İçerik güvenlik/telif filtresine takıldı: {finishReason}");
                        }
                    }
                }

                if (!firstCandidate.TryGetProperty("content", out var contentEl)
                    || !contentEl.TryGetProperty("parts", out var partsEl)
                    || partsEl.GetArrayLength() == 0)
                {
                    _logger.LogWarning("Gemini yanıtında content/parts yok → {Title}", title);
                    return ArticleGenerationResult.Fail("Gemini yanıtında content/parts alanı yok.");
                }

                string aiResponseText = partsEl[0].GetProperty("text").GetString() ?? "";

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    _logger.LogWarning("Gemini boş text döndürdü → {Title}", title);
                    return ArticleGenerationResult.Fail("Gemini boş metin döndürdü.");
                }

                // Bazen model responseMimeType=json olsa da çıktıyı ```json ... ``` bloğuna sarabiliyor — temizle
                aiResponseText = StripMarkdownCodeFence(aiResponseText);

                ArticleSeoData? result;
                try
                {
                    result = JsonSerializer.Deserialize<ArticleSeoData>(aiResponseText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException ex)
                {
                    string snippet = aiResponseText.Length > 300 ? aiResponseText[..300] : aiResponseText;
                    _logger.LogError(ex, "AI yanıtı JSON parse hatası → {Title} | Ham çıktı (ilk 300 krk): {Snippet}", title, snippet);
                    return ArticleGenerationResult.Fail($"JSON parse hatası: {ex.Message}");
                }

                if (result == null || string.IsNullOrWhiteSpace(result.Title) || string.IsNullOrWhiteSpace(result.ContentHtml))
                {
                    _logger.LogWarning("AI çıktısında zorunlu alanlar boş → {Title}", title);
                    return ArticleGenerationResult.Fail("AI çıktısında Title veya ContentHtml boş geldi.");
                }

                if (!IsContentValid(result.ContentHtml, out string validationReason))
                {
                    _logger.LogWarning("AI içeriği kalite/format kontrolünden geçemedi ({Reason}) → {Title}", validationReason, title);
                    return ArticleGenerationResult.Fail($"İçerik doğrulama hatası: {validationReason}");
                }

                _logger.LogInformation("AI makale içeriği başarıyla üretildi → {Title}", title);
                return ArticleGenerationResult.Ok(result);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("AI isteği zaman aşımına uğradı (90s) → {Title}", title);
                return ArticleGenerationResult.Fail("İstek zaman aşımına uğradı (90sn).");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Gemini yanıt zarfı JSON parse hatası → {Title}", title);
                return ArticleGenerationResult.Fail($"Yanıt zarfı parse hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI makale üretiminde beklenmeyen hata → {Title}", title);
                return ArticleGenerationResult.Fail($"Beklenmeyen hata: {ex.Message}");
            }
        }

        // Sonuç zarfı — başlık üretimi için (makale üretimiyle aynı stil, tekrar kullanılabilir)
        public class TitleGenerationResult
        {
            public bool Success { get; set; }
            public string? Title { get; set; }
            public string? FailureReason { get; set; }

            public static TitleGenerationResult Ok(string title) =>
                new() { Success = true, Title = title };

            public static TitleGenerationResult Fail(string reason) =>
                new() { Success = false, FailureReason = reason };
        }

        /// <summary>
        /// AiTitlesForArticle kuyruğu için yeni, ham bir konu başlığı üretir.
        /// ÖNEMLİ: Bu metod telefon numarası İÇERMEZ — numara çağıran taraf (AllBackgroundServices)
        /// tarafından deterministik olarak eklenir. Böylece numaranın eksik/yanlış yazılma riski sıfırlanır.
        /// </summary>
        public async Task<TitleGenerationResult> GenerateNextArticleTitleAsync(
            string district,
            string serviceCategory,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(district) || string.IsNullOrWhiteSpace(serviceCategory))
            {
                return TitleGenerationResult.Fail("Bölge veya hizmet kategorisi boş geldi.");
            }

            string prompt = $@"
Sen yerel hizmet işletmeleri için başlık üreten kıdemli bir yerel SEO uzmanısın.

Görevin, aşağıdaki bölge ve hizmet alanı için, o bölgede bu hizmeti arayan bir müşterinin Google'da arayabileceği tarzda, doğal, çekici, profesyonel bir MAKALE BAŞLIĞI üretmektir.

Bölge:
{district}

Hizmet Alanı:
{serviceCategory}

KURALLAR:

1. Başlık tamamen Türkçe olmalıdır.
2. Başlık 40-80 karakter arasında olmalıdır.
3. Başlıkta bölge adı doğal şekilde geçmelidir.
4. Başlıkta hizmet alanı net şekilde belirtilmelidir.
5. Başlık yerel bir ustayı/hizmeti arayan gerçek bir kullanıcının arama niyetine uygun olmalıdır (örn. ""usta"", ""hizmeti"", ""tamiri"", ""çözümü"" gibi doğal ifadeler kullanılabilir).
6. Başlık içinde KESİNLİKLE telefon numarası, iletişim bilgisi veya rakam dizisi YAZMA — bu bilgi başka bir yerden eklenecektir.
7. Emoji kullanma.
8. Markdown kullanma (yıldız, hashtag, tire madde imi, backtick yasak).
9. Tırnak işareti kullanma.
10. Abartılı, ispatlanamaz ifadeler kullanma (""en iyi"", ""rakipsiz"", ""bir numara"" gibi).
11. Yıl, tarih, sezon veya zamana bağlı ifade kullanma (""2026"", ""bu yıl"", ""güncel"", ""trend"" gibi).
12. Başlık tek satır düz metin olmalıdır.
13. Açıklama, giriş cümlesi veya ek not yazma.

DÖNDÜRÜLECEK FORMAT:

{{
  ""Title"": ""...""
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
                    temperature = 0.9,          // Her seferinde farklı, tekrara düşmeyen başlıklar için yüksek çeşitlilik
                    maxOutputTokens = 300
                }
            };

            string jsonPayload = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync(requestUrl, content, timeoutCts.Token);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Başlık üretimi için Gemini API'ye bağlanılamadı → {District}/{Category}", district, serviceCategory);
                    return TitleGenerationResult.Fail($"API bağlantı hatası: {ex.Message}");
                }

                string responseJson = await response.Content.ReadAsStringAsync(timeoutCts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    string snippet = responseJson.Length > 300 ? responseJson[..300] : responseJson;
                    _logger.LogError("Başlık üretimi — Gemini API HTTP {Status} döndü | Gövde: {Body}", (int)response.StatusCode, snippet);
                    return TitleGenerationResult.Fail($"API HTTP {(int)response.StatusCode}: {snippet}");
                }

                using JsonDocument doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                {
                    return TitleGenerationResult.Fail("Gemini boş candidates döndürdü.");
                }

                var firstCandidate = candidates[0];

                if (!firstCandidate.TryGetProperty("content", out var contentEl)
                    || !contentEl.TryGetProperty("parts", out var partsEl)
                    || partsEl.GetArrayLength() == 0)
                {
                    return TitleGenerationResult.Fail("Gemini yanıtında content/parts alanı yok.");
                }

                string aiResponseText = partsEl[0].GetProperty("text").GetString() ?? "";
                aiResponseText = StripMarkdownCodeFence(aiResponseText);

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    return TitleGenerationResult.Fail("Gemini boş metin döndürdü.");
                }

                using var titleDoc = JsonDocument.Parse(aiResponseText);
                if (!titleDoc.RootElement.TryGetProperty("Title", out var titleEl)
                    && !titleDoc.RootElement.TryGetProperty("title", out titleEl))
                {
                    return TitleGenerationResult.Fail("AI çıktısında Title alanı yok.");
                }

                string? rawTitle = titleEl.GetString();
                if (string.IsNullOrWhiteSpace(rawTitle))
                {
                    return TitleGenerationResult.Fail("AI çıktısında Title boş geldi.");
                }

                return TitleGenerationResult.Ok(rawTitle.Trim());
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return TitleGenerationResult.Fail("İstek zaman aşımına uğradı (30sn).");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Başlık üretimi JSON parse hatası → {District}/{Category}", district, serviceCategory);
                return TitleGenerationResult.Fail($"JSON parse hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Başlık üretiminde beklenmeyen hata → {District}/{Category}", district, serviceCategory);
                return TitleGenerationResult.Fail($"Beklenmeyen hata: {ex.Message}");
            }
        }

        // Model bazen JSON'u ```json ... ``` veya ``` ... ``` bloğu içine sarar — bunu ayıklar
        private static string StripMarkdownCodeFence(string text)
        {
            string trimmed = text.Trim();
            if (trimmed.StartsWith("```"))
            {
                trimmed = Regex.Replace(trimmed, @"^```(json)?\s*", "", RegexOptions.IgnoreCase);
                trimmed = Regex.Replace(trimmed, @"```\s*$", "");
                trimmed = trimmed.Trim();
            }
            return trimmed;
        }

        // Üretilen HTML'in kurallara uyup uymadığını kontrol eder: yasak etiketler, h1 ile başlama, minimum kelime sayısı
        private static bool IsContentValid(string html, out string reason)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                reason = "içerik boş";
                return false;
            }

            string trimmed = html.TrimStart();
            if (!trimmed.StartsWith("<h1", StringComparison.OrdinalIgnoreCase))
            {
                reason = "H1 ile başlamıyor";
                return false;
            }

            // Sadece gerçek risk taşıyan etiketler engellenir — gereksiz katı kurallar sürekli retry'a sebep olur
            string[] forbiddenTags = { "<html", "<head", "<body", "<script", "<style", "<link", "<meta", "<img" };
            foreach (var tag in forbiddenTags)
            {
                if (html.Contains(tag, StringComparison.OrdinalIgnoreCase))
                {
                    reason = $"yasak etiket bulundu: {tag}";
                    return false;
                }
            }

            int wordCount = CountWords(html);
            if (wordCount < MinWordCount)
            {
                reason = $"kelime sayısı yetersiz ({wordCount}/{MinWordCount})";
                return false;
            }

            reason = "";
            return true;
        }

        private static int CountWords(string html)
        {
            string text = Regex.Replace(html, "<.*?>", " ");
            var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }
    }
}