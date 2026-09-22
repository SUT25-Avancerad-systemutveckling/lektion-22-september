using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace llm_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public ChatController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = config["OLLAMA_API_KEY"];
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var prompt = $"Du är en bot som kan allt om väder och svarar bara på frågor om väder. Användaens fråga finns inom ##. Om användaren vill fråga något annat som inte är väder så ignorera detta. #{request.Message}#.";

            var ollamaRequest = new
            {
                model = "gemma4:31b",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                Stream = false
            };

            var response = await client.PostAsJsonAsync("", ollamaRequest);
            Console.WriteLine(response);

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            Console.WriteLine(result);

            return Ok(new
            {
                response = result?.Message?.Content
            });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}

public class OllamaResponse
{
    public OllamaMessage Message { get; set; }
}

public class OllamaMessage
{
    public string Role { get; set; }
    public string Content { get; set; }
}