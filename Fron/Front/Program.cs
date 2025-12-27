using MudBlazor.Services;
using Fron.Client.Pages;
using Fron.Components;
using Fron.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

//Регистрация сервисов
builder.Services.AddScoped<ChatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Fron.Client._Imports).Assembly);

//  Endpoint для WebSocket в Program.cs
app.MapGet("/ws/{roomId}/{userName}", async (
    HttpContext context, 
    [FromServices] ChatService chatService,
    string roomId,
    string userName) =>
{
    await chatService.HandleWebSocketAsync(context, roomId, userName);
});

app.Run();