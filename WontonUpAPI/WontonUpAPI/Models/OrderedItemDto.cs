namespace WontonUpAPI.Models
{
    public class OrderedItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Totalt pris för denna post (Quantity * Price)
    }
}
