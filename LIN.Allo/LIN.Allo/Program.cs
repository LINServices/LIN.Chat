using LIN.Allo.Components;
using LIN.Access.Auth;
using LIN.Access.Communication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
   .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthenticationService();
builder.Services.AddCommunicationService();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

LIN.Access.Search.Build.Init();

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(LIN.Allo.Client.Pages.Login).Assembly)
   .AddAdditionalAssemblies(typeof(LIN.Allo.Shared._Imports).Assembly)
   .AddAdditionalAssemblies(typeof(LIN.Emma.UI.Emma).Assembly);

app.Run();