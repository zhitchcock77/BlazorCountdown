using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorCountdown;
using BlazorCountdown.Services;
using BlazorCountdown.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ILetterProvider, LetterProvider>();
builder.Services.AddScoped<IWordValidator, WordValidator>();
builder.Services.AddScoped<IAudioService, AudioService>();

await builder.Build().RunAsync();
