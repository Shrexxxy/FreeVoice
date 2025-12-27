using FreeVooce.Front.Server.Components;
using FreeVooce.Front.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FreeVoice.Front.Client._Imports).Assembly);
app.MapFallbackToFile("App.razor");
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