namespace WontonUpAPI.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public OrderedReceiptItemDto[] Items { get; set; } = Array.Empty<OrderedReceiptItemDto>();
        public decimal OrderValue { get; set; }
        public string Eta { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
