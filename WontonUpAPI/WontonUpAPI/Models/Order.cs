using System.ComponentModel.DataAnnotations;

namespace WontonUpAPI.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public string ItemsJson { get; set; } = string.Empty;

        public decimal OrderValue { get; set; }

        public DateTime Eta { get; set; }

        public DateTime Timestamp { get; set; }

        public string State { get; set; } = "waiting";
    }
}
