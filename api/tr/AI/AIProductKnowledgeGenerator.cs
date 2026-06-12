using Google.GenAI;
using Google.GenAI.Types;
using System.Net.Http;

public class AIProductKnowledgeGenerator
{
    private readonly Client _client;
    private const string MODEL = "gemini-2.0-flash";
    private const string API_KEY = "AQ.Ab8RN6Il8ODEGLl5GSn9vYaoGs1WRIQJYUKqxdSP8pI18udrJQ";

    // HttpClient, DI (Dependency Injection) üzerinden otomatik gelir
    public AIProductKnowledgeGenerator(HttpClient httpClient)
    {
        // Not: Google.GenAI kütüphanesi yapılandırmasına göre 
        // buraya httpClient'ı geçmeniz gerekebilir. 
        // Eğer kütüphane sadece API Key istiyorsa, httpClient'ı 
        // internal bir HTTP operasyonu için saklayabilirsiniz.

        _client = new Client(apiKey: API_KEY);
    }

    public async Task<string> GenerateProductContentAsync(string prompt)
    {
        try
        {
            var response = await _client.Models.GenerateContentAsync(
                model: MODEL,
                contents: prompt
            );

            return response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "EMPTY";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}