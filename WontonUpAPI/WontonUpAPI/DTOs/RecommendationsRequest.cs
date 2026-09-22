using System.ComponentModel.DataAnnotations;

namespace WontonUpAPI.DTOs
{
    public class RecommendationsRequest
    {
        [Required]
        [MinLength(5)]
        public string prompt { get; set; } = String.Empty;
    }
}
