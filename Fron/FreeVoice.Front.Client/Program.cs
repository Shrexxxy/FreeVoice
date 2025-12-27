using System.Globalization;
using FreeVoice.Front.Client.Application;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, CustomAuthenticationStateProvider>();


builder.Services.AddMudServices();

await builder.Build().RunAsync();