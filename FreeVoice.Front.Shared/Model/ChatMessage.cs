namespace FreeVoice.Front.Shared.Model;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsSystemMessage { get; set; }
}