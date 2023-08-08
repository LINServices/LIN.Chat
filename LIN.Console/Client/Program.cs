global using Microsoft.JSInterop;
using Blazored.LocalStorage;
using LIN.Console.Client;
using LIN.Console.Client.Online;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage(config =>
      config.JsonSerializerOptions.WriteIndented = true);


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


App.MyAddress = builder.HostEnvironment.BaseAddress;

Scripts.Build();

StaticHub.Key =  LIN.Shared.Tools.KeyGen.Generate(20, "dv.");

await builder.Build().RunAsync();
