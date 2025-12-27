using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using FreeVoice.Front.Shared.Model;

namespace FreeVooce.Front.Server.Services;

public class ChatService
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
    private readonly ConcurrentBag<ChatMessage> _messageHistory = new();
    
    public async Task HandleWebSocketAsync(
        HttpContext context, 
        string roomId, 
        string userName)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = $"{roomId}_{userName}_{Guid.NewGuid()}";
            
            _sockets.TryAdd(socketId, webSocket);
            
            await SendMessageHistoryAsync(webSocket);
            
            await BroadcastSystemMessageAsync($"{userName} присоединился к чату", roomId);
            
            try
            {
                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    var message = JsonSerializer.Deserialize<ChatMessage>(messageJson);
                    
                    if (message != null)
                    {
                        message.Timestamp = DateTime.Now;
                        _messageHistory.Add(message);
                        
                        await BroadcastMessageAsync(message, roomId);
                    }

                    receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(
                    receiveResult.CloseStatus.Value,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);
            }
            finally
            {
                _sockets.TryRemove(socketId, out _);
                await BroadcastSystemMessageAsync($"{userName} покинул чат", roomId);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    private async Task SendMessageHistoryAsync(WebSocket webSocket)
    {
        var history = _messageHistory.TakeLast(50).ToList();
        var historyJson = JsonSerializer.Serialize(history);
        var buffer = Encoding.UTF8.GetBytes(historyJson);
        
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
    
    private async Task BroadcastMessageAsync(ChatMessage message, string roomId)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var buffer = Encoding.UTF8.GetBytes(messageJson);
        
        var tasks = _sockets
            .Where(s => s.Key.StartsWith(roomId))
            .Select(s => s.Value.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None));
        
        await Task.WhenAll(tasks);
    }
    
    private async Task BroadcastSystemMessageAsync(string text, string roomId)
    {
        var systemMessage = new ChatMessage
        {
            UserName = "System",
            Message = text,
            IsSystemMessage = true,
            Timestamp = DateTime.Now
        };
        
        _messageHistory.Add(systemMessage);
        await BroadcastMessageAsync(systemMessage, roomId);
    }
}