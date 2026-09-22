using System.ComponentModel.DataAnnotations;

namespace WontonUpAPI.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        [MinLength(1)]
        public int[] Items { get; set; } = System.Array.Empty<int>();
    }
}
