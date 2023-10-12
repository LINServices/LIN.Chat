global using LIN.Types.Auth.Enumerations;
global using LIN.Types.Auth.Models;
global using LIN.Types.Enumerations;
global using LIN.Types.Responses;
global using Microsoft.JSInterop;
global using LIN.Types.Communication.Models;
global using Microsoft.AspNetCore.Components;
global using LIN.Allo.Client;
global using LIN.Allo.Client.Sections;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LIN.Allo.Client.Online;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<LIN.Allo.Client.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage(config =>
      config.JsonSerializerOptions.WriteIndented = true);


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

Scripts.Build();

StaticHub.Key = LIN.Modules.KeyGen.Generate(20, "dv.");

await builder.Build().RunAsync();
