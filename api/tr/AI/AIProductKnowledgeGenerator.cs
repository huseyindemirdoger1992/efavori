using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AIProductKnowledgeGenerator
{
    private readonly string _apiKey = "AQ.Ab8RN6Il8ODEGLl5GSn9vYaoGs1WRIQJYUKqxdSP8pI18udrJQ";
    private readonly HttpClient _httpClient;

    // ✅ AddHttpClient<AIProductKnowledgeGenerator>() ile uyumlu constructor
    // DI, IHttpClientFactory üzerinden HttpClient oluşturup buraya enjekte eder.
    public AIProductKnowledgeGenerator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // JSON Çıktısını Deserialization (Nesneye çevirme) işlemi için modelimiz
    public class ProductSeoData
    {
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string Keywords { get; set; }
    }

    public async Task<ProductSeoData> GenerateSeoContentAsync(string productName)
    {
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

        // Gemini 2.5 Flash REST API Endpoint'i
        string requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={_apiKey}";

        // API'nin beklediği istek şeması
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
                responseMimeType = "application/json", // Kesin JSON dönüşü sağlar
                temperature = 0.7
            }
        };

        // İsteği JSON string'e çevir
        string jsonPayload = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            // API'ye POST isteğini atıyoruz
            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            // Dönen cevabı okuyoruz
            string responseJson = await response.Content.ReadAsStringAsync();

            // Google Gemini cevabı "candidates" isimli bir array içinde döner. İçindeki asıl text'i çıkarıyoruz.
            using JsonDocument doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            string aiResponseText = root.GetProperty("candidates")[0]
                                        .GetProperty("content")
                                        .GetProperty("parts")[0]
                                        .GetProperty("text").GetString();

            // Çıkardığımız saf JSON metnini senin ProductSeoData C# nesnene dönüştürüyoruz
            var result = JsonSerializer.Deserialize<ProductSeoData>(aiResponseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Yapay zeka servisinde hata oluştu: {ex.Message}");
            return null;
        }
    }
}
