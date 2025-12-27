using Fron.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Shared.Model;

namespace Fron.Client.Pages;

public partial class Room : ComponentBase
{
    private List<ChatMessageViewModel> _messages = new();
    
    private EditForm _form = new();
    private ChatMessageEditFormModel _model = new();
    private MudTextField<string> _messageInput = new();
    
    /// <summary>
    /// Метод отправки сообщения.
    /// Пока что локальный.
    /// </summary>
    private async Task SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(_model.Message))
        {
            var msg = new ChatMessageViewModel(_model.Message);
            _messages.Add(msg);
            
            // Очищаем поле ввода
            _model = new();
            
            StateHasChanged();

            // Обязательно должен быть delay. Иначе фокус не установится
            await Task.Delay(1);
            
            await _messageInput.FocusAsync();
        }
    }
}


public class ChatMessageEditFormModel
{
    public string? Message = null!;
}