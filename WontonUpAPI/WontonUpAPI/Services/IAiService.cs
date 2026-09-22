namespace WontonUpAPI.Services
{
    public interface IAiService
    {
        Task<string> SendPrompt(string systemPrompt, string userPrompt);
    }
}
