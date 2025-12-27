namespace FreeVoice.Front.Client.ViewModels;


public record ChatMessageViewModel
{
    public string Text { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    
    public ChatMessageViewModel(string text)
    {
        Text = text;
    }
}