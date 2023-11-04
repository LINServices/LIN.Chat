global using LIN.Types.Auth.Enumerations;
global using LIN.Types.Auth.Models;
global using LIN.Types.Enumerations;
global using LIN.Types.Responses;
global using Microsoft.JSInterop;
global using LIN.Types.Communication.Models;
global using Microsoft.AspNetCore.Components;
global using LIN.Allo.Client;
global using LIN.Allo.Client.Sections;
global using LIN.Allo.Client.Elements.Drawers;
global using LIN.Allo.Client.Elements.Dropdowns;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();