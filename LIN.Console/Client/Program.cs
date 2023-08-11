global using Microsoft.JSInterop;
global using LIN.Types.Responses;
global using LIN.Types.Developer;
global using LIN.Types.Developer.Models;
global using LIN.Types.Auth.Models;
global using LIN.Types.Enumerations;
global using LIN.Access;
global using LIN.Types.Auth.Enumerations;
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

StaticHub.Key =  LIN.Modules.KeyGen.Generate(20, "dv.");

await builder.Build().RunAsync();
