using System.Net.Http.Headers;
using WontonUpAPI.Models;

namespace WontonUpAPI.Services
{
    public class OllamaService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public OllamaService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["OLLAMA_API_KEY"];
        }

        public async Task<string> SendPrompt(string systemPrompt, string userPrompt)
        {
            var payload = new
            {
                model = "gemma4:31b",
                messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
                Stream = false
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://ollama.com/api/chat");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request);
            //var json = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(json);
            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            Console.WriteLine(result);
            return result?.Message.Content ?? "";
        }
    }
}
