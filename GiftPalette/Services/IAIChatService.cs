namespace GiftPalette.Services;

public interface IAIChatService
{
    Task<string> SendMessageAsync(string message, string threadId = "");
    Task<string> CreateThreadAsync();
}