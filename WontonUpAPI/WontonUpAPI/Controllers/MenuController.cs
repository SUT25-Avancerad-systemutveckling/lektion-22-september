using Microsoft.AspNetCore.Mvc;
using WontonUpAPI.Services;

namespace WontonUpAPI.Controllers
{
    [ApiController]
    [Route("menu")]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;

        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? type)
        {
            var items = _menuService.GetAll(type);
            return Ok(items);
        }
    }
}
