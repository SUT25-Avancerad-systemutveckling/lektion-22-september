namespace WontonUpAPI.Models
{
    public record OllamaResponse
    {
        public string Model { get; init; } = string.Empty;

        public DateTime CreatedAt { get; init; }

        public ChatMessage Message { get; init; } = new();

        public bool Done { get; init; }

        public string? DoneReason { get; init; }

        public long TotalDuration { get; init; }

        public int PromptEvalCount { get; init; }

        public long PromptEvalCachedCount { get; init; }

        public int EvalCount { get; init; }
    }

    public record ChatMessage
    {
        public string Role { get; init; } = string.Empty;

        public string Content { get; init; } = string.Empty;
    }
}
