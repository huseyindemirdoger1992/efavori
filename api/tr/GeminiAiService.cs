using System.Net.Http.Json;
using System.Text.Json;

public class GeminiAiService
{
    private readonly HttpClient _httpClient;
    // Görseldeki güncel ve tam API anahtarınız yerleştirildi
    private readonly string _apiKey = "AQ.Ab8RN6KwhxuhcjYnLCqkl-8s5vixV9D-bv2y7jx-LuQpEBKlQQ";

    public GeminiAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GenerateKeywordsAsync(string ProductName)
    {
        // endpoint ve api key parametresi hazırlandı
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        // Net ve kesin bir prompt yapısı
        var prompt = "GÖREV:\n" +
             $"Sana iletilen ürün kimliği/adı ({ProductName}) için, Trendyol, Hepsiburada ve Amazon pazaryerlerindeki kullanıcı arama alışkanlıklarını (Search Intent) analiz et. Bu ürün için aranma hacmi ve dönüşüm potansiyeli en yüksek 10 farklı SEO uyumlu alternatif başlık / arama terimi varyasyonu üret.\n\n" +
             "STRATEJİK ÇEŞİTLİLİK KURALLARI:\n" +
             "Üreteceğin 10 terim şu varyasyonları dengeli şekilde içermelidir:\n" +
             "- Jenerik Aramalar (Örn: 'Kablosuz Kulaklık')\n" +
             "- Spesifik Teknik Özellik Aramaları (Örn: 'Bluetooth 5.3 Kulak İçi Kulaklık')\n" +
             "- Amaca/Hedef Kitleye Yönelik Aramalar (Örn: 'Sporcu Bluetooth Kulaklık Siyah')\n" +
             "- Uzun Kuyruklu (Long-Tail) Niş Aramalar (Örn: 'Gürültü Engelleyici Kablosuz Kulaklık')\n\n" +
             "KATI FORMAT KURALLARI:\n" +
             "1. Çıktı KESİNLİKLE sadece geçerli ve parse edilebilir bir JSON string dizisi (Array) olmalıdır.\n" +
             "2. Örnek Format: [\"terim1\", \"terim2\", \"terim3\", \"terim4\", \"terim5\", \"terim6\", \"terim7\", \"terim8\", \"terim9\", \"terim10\"]\n" +
             "3. Asla '```json' veya '```' gibi markdown kod blokları KULLANMA.\n" +
             "4. Metnin başına veya sonuna hiçbir açıklama, giriş cümlesi, tırnak işareti veya ek karakter EKLEME.\n" +
             "5. Sadece ve sadece köşeli parantezle başlayıp biten, ham (raw) JSON dizisini döndür.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                responseSchema = new
                {
                    type = "ARRAY",
                    items = new { type = "STRING" }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            if (!response.IsSuccessStatusCode) return new List<string>();

            var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();

            // Gemini response ağacından text verisini güvenli bir şekilde çekiyoruz
            var rawText = jsonResult.GetProperty("candidates")[0]
                                    .GetProperty("content")
                                    .GetProperty("parts")[0]
                                    .GetProperty("text").GetString();

            if (string.IsNullOrWhiteSpace(rawText)) return new List<string>();

            return JsonSerializer.Deserialize<List<string>>(rawText.Trim()) ?? new List<string>();
        }
        catch
        {
            // Olası bir bağlantı koptu/kota doldu durumunda arka plan işçisinin (worker) kilitlenmemesi için boş liste dönüyoruz
            return new List<string>();
        }
    }

    public async Task<string> GenerateSingleSeoDescriptionAsync(string ProductName)
    {
        // 1. Giriş Doğrulama (Gereksiz API maliyetini önler)
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            // _logger.LogWarning("SEO Açıklaması üretilemedi: Ürün adı boş veya geçersiz.");
            return string.Empty;
        }

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        // 2. Dinamik ve Optimize Edilmiş Prompt Yapısı
        var advancedPrompt = "GÖREV:\n" +
                     $"Sana iletilen ham ürün adını ({ProductName}), Trendyol, Hepsiburada ve Amazon gibi pazaryerlerinde SEO performansını zirveye çıkaracak, yüksek dönüşüm oranlı (CTR) ideal bir 'Kısa Açıklama / Arama Meta Açıklaması' metnine dönüştür.\n\n" +
                     "KATI KURALLAR VE FORMAT:\n" +
                     "1. Karakter Sınırı: Çıktı, boşluklar dahil KESİNLİKLE 140 ile 160 karakter arasında olmalıdır. Ne eksik ne fazla.\n" +
                     "2. Yapısal Bileşenler: Metin; [Marka] [Model/Seri] + [En Önemli Teknik Özellik/Malzeme] + [Renk/Varyant] + [Temel Kullanım Amacı/Fayda] formülünü takip etmelidir.\n" +
                     "3. SEO & Doğallık: Arama hacmi yüksek anahtar kelimeleri doğal bir akışla metne yedir. Kelime yığını (keyword stuffing) yapma; akıcı, profesyonel, müşteri güvenini kazanan bir satış dili kullan.\n" +
                     "4. Platform Uyumluluğu: Hem Amazon'un ürün alt başlığı mantığına hem de Trendyol/Hepsiburada'nın arama algoritması gereksinimlerine hitap etmeli, tıklama teşvik eden bir tona sahip olmalı.\n" +
                     "5. Çıktı Kısıtlaması: Bana giriş cümlesi, açıklama, tırnak işareti, 'İşte metniniz:' gibi ek ifadeler veya analiz sunma. SADECE 140-160 karakterlik nihai açıklama metnini döndür.";

        // 3. API İstek Gövdesi ve Şema Tanımı
        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[] { new { text = advancedPrompt } }
            }
        },
            generationConfig = new
            {
                responseMimeType = "application/json",
                responseSchema = new
                {
                    type = "STRING" // API'nin yapısal JSON string döndürmesini zorunlu kılıyoruz
                }
            }
        };

        var localJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            // 4. HTTP POST İsteği
            using var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // _logger.LogError("Gemini API Hatası Döndürdü. Durum Kodu: {StatusCode}, Detay: {Error}", response.StatusCode, errorContent);
                return string.Empty;
            }

            // 5. JSON Yanıtını Güvenli Okuma
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>(localJsonOptions);

            if (jsonResult.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0 &&
                candidates[0].TryGetProperty("content", out var content) &&
                content.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0 &&
                parts[0].TryGetProperty("text", out var textElement))
            {
                var rawText = textElement.GetString();

                if (string.IsNullOrWhiteSpace(rawText)) return string.Empty;

                rawText = rawText.Trim();

                // SIFIR HATA STRATEJİSİ (Parser Koruma Katmanı):

                // Varyasyon A: API tam kurallara uyup çift tırnaklı JSON string dönerse (Örn: "Samsung Galaxy...")
                if (rawText.StartsWith("\"") && rawText.EndsWith("\""))
                {
                    return JsonSerializer.Deserialize<string>(rawText, localJsonOptions) ?? string.Empty;
                }

                // Varyasyon B: API kuralları esnetip Markdown JSON bloğu içinde dönerse
                if (rawText.StartsWith("```"))
                {
                    var cleanedJson = rawText.Replace("```json", "").Replace("```", "").Trim();

                    // Eğer temizlenen yapı yine tırnaklı bir string ise deserialize et, yoksa düz metindir
                    if (cleanedJson.StartsWith("\"") && cleanedJson.EndsWith("\""))
                    {
                        return JsonSerializer.Deserialize<string>(cleanedJson, localJsonOptions) ?? string.Empty;
                    }
                    return cleanedJson;
                }

                // Varyasyon C: API direkt olarak temiz, ham metin döndüyse (Çift Deserialize hatasını önler, harflerin ayrışmasını çözen ana yer)
                return rawText;
            }

            // _logger.LogWarning("Gemini API yanıt yapısı beklenen formatta gelmedi (Candidates veya Parts bulunamadı).");
            return string.Empty;
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "{ProductName} için SEO açıklaması üretilirken beklenmeyen bir hata oluştu.", productName);
            return string.Empty;
        }
    }
}