global using LIN.Types.Cloud.Identity.Enumerations;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Types.Communication.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Components;
global using Microsoft.JSInterop;
global using LIN.Allo.Client.Pages;
global using LIN.Types.Cloud.Identity.Abstracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LIN.Access.Auth;
using LIN.Access.Communication;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCommunicationService();
LIN.Allo.Shared.Services.Scripts.Build();
builder.Services.AddAuthenticationService();
LIN.Access.Search.Build.Init();

await builder.Build().RunAsync();