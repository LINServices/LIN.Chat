using LIN.Allo.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
   .AddInteractiveWebAssemblyComponents();



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

LIN.Access.Auth.Build.Init();
LIN.Access.Communication.Build.Init();
LIN.Access.Search.Build.Init();

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveWebAssemblyRenderMode()
   .AddInteractiveServerRenderMode()
   .AddAdditionalAssemblies(typeof(LIN.Allo.Client.Pages.Login).Assembly)
   .AddAdditionalAssemblies(typeof(LIN.Allo.Shared._Imports).Assembly)
   .AddAdditionalAssemblies(typeof(LIN.Emma.UI.Emma).Assembly);


app.Run();