global using LIN.Types.Cloud.Identity.Enumerations;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Types.Communication.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Components;
global using Microsoft.JSInterop;
global using LIN.Allo.Client.Pages;
global using LIN.Types.Cloud.Identity.Abstracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

LIN.Allo.Shared.Services.Scripts.Build();
LIN.Access.Auth.Build.Init();
LIN.Access.Communication.Build.Init();
LIN.Access.Search.Build.Init();

await builder.Build().RunAsync();