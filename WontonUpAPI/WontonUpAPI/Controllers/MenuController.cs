using Microsoft.AspNetCore.Mvc;
using WontonUpAPI.DTOs;
using WontonUpAPI.Services;

namespace WontonUpAPI.Controllers
{
    [ApiController]
    [Route("menu")]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;
        private readonly IAiService _aiService;
        private readonly string system = "WONTONS\r\n- Karlstad (9 kr): kantarell, schalottenlök, morot, bladpersilja\r\n- Bangkok (9 kr): morot, salladslök, chili, kokos, lime, koriander\r\n- Ho Chi Minh (9 kr): kål, morot, salladslök, chili, vitlök, ingefära, tofu\r\n- Paris (9 kr): kål, honung, chèvre, basilika, valnötspasta\r\n- Oaxaca (9 kr): majs, tomat, rostade ärtor, vitlök, lime\r\n\r\nDIPS\r\n- Sweet Chili (19 kr): stark och söt\r\n- Sweet n Sour (19 kr): sötsur\r\n- Guacamole (19 kr): avokado, tomat och kryddor\r\n- Wonton Standard (19 kr): soja, chili, vitlök och ingefära\r\n- Hot Mango (19 kr): kryddstark och söt mango\r\n- Chili Mayo (19 kr): majonnäs och chili\r\n\r\nDRYCKER\r\n- Sprite (19 kr)\r\n- Fanta Orange (19 kr)\r\n- Fanta Exotic (19 kr)\r\n- Coca Cola (19 kr)\r\n- LOKA Citrus (19 kr)\r\n- LOKA Granatäpple (19 kr)\r\nSvara ENDAST med ett giltigt JSON-objekt, ingen annan text, i exakt detta format:\r\n\r\n{\r\n  \"items\": [{ name: string, type: string, why: string ]}";

        public MenuController(MenuService menuService, IAiService aiService)
        {
            _menuService = menuService;
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? type)
        {
            var items = _menuService.GetAll(type);
            return Ok(items);
        }

        [HttpPost("recommendations")]
        public async Task<IActionResult> GetRecommendations([FromBody] RecommendationsRequest request)
        {
            var recommendations = await _aiService.SendPrompt(system, request.prompt);

            return Ok(recommendations);
        }
    }
}
