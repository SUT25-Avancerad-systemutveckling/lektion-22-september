using System.Text.Json;
using WontonUpAPI.Models;

namespace WontonUpAPI.Services
{
    public class MenuService
    {
        private readonly List<MenuItem> _items;

        public MenuService()
        {
            string menuFile = File.ReadAllText("./Services/Menu.json");
            _items = JsonSerializer.Deserialize<List<MenuItem>>(menuFile) ?? new List<MenuItem>();
        }

        public IEnumerable<MenuItem> GetAll(string? type = null)
        {
            if (string.IsNullOrEmpty(type)) return _items;
            return _items.Where(i => i.Type == type);
        }

        public MenuItem? GetById(int id) => _items.FirstOrDefault(i => i.Id == id);
    }
}
