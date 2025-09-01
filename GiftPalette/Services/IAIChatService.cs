namespace GiftPalette.Services;

public interface IAIChatService
{
    Task<string> SendMessageAsync(string message, string threadId = "", string cartId = "");
    Task<string> CreateThreadAsync();
}