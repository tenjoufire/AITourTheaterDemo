namespace GiftPalette.Services;

public class ChatStateService
{
    public event Action<bool>? ChatVisibilityChanged;
    
    private bool _isChatVisible = false;
    
    public bool IsChatVisible => _isChatVisible;
    
    public void ShowChat()
    {
        if (!_isChatVisible)
        {
            _isChatVisible = true;
            ChatVisibilityChanged?.Invoke(_isChatVisible);
        }
    }
    
    public void HideChat()
    {
        if (_isChatVisible)
        {
            _isChatVisible = false;
            ChatVisibilityChanged?.Invoke(_isChatVisible);
        }
    }
    
    public void ToggleChat()
    {
        if (_isChatVisible)
            HideChat();
        else
            ShowChat();
    }
}