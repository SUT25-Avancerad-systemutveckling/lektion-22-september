using System.Collections.Generic;

namespace WontonUpAPI.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "wonton" | "dip" | "drink"
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<string>? Ingredients { get; set; }
    }
}
