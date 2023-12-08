global using LIN.Allo.Client.Sections;
global using LIN.Types.Identity.Enumerations;
global using LIN.Types.Identity.Models;
global using LIN.Types.Communication.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Components;
global using Microsoft.JSInterop;
global using LIN.Allo.Client.Elements.Drawers;
global using LIN.Allo.Client.Shared;
global using LIN.Allo.Client.Pages;
global using LIN.Types.Identity.Abstracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();