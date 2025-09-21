using LIN.Access.Auth;
using LIN.Access.Communication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCommunicationService();
LIN.Allo.Shared.Services.Scripts.Build();
builder.Services.AddAuthenticationService();
LIN.Access.Search.Build.Init();

await builder.Build().RunAsync();