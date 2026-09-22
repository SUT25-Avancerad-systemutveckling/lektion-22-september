namespace WontonUpAPI.DTOs
{
    public class ReceiptDto
    {
        public Guid Id { get; set; }
        public decimal OrderValue { get; set; }
        public string Timestamp { get; set; } = string.Empty;
        public int Eta { get; set; }
        public OrderedReceiptItemDto[] Items { get; set; } = Array.Empty<OrderedReceiptItemDto>();
    }

    public class OrderedReceiptItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Totalt pris för denna post (Quantity * Price)
    }
}
