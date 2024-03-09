using SILF.Script.Enums;
using SILF.Script.Interfaces;

namespace LIN.Allo.Client.Elements.Drawers;


public partial class Emma
{

    /// <summary>
    /// Id único del elemento.
    /// </summary>
    private string UniqueId { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Actual estado.
    /// </summary>
    private State ActualState { get; set; } = State.Witting;



    /// <summary>
    /// Lista de estados.
    /// </summary>
    private enum State
    {
        Witting,
        Responding
    }



    /// <summary>
    /// Entrada del usuario.
    /// </summary>
    private string Prompt { get; set; } = string.Empty;



    /// <summary>
    /// Respuesta de Emma.
    /// </summary>
    private string EmmaResponse { get; set; } = string.Empty;







    /// <summary>
    /// Abre el elemento.
    /// </summary>
    public async void Show()
    {
        await js.InvokeAsync<object>("ShowDrawer", $"drawerEmma-{UniqueId}", $"close-drawerEmma-{UniqueId}");
        StateHasChanged();
    }



    /// <summary>
    /// Enviar a Emma.
    /// </summary>
    private async void ToEmma()
    {

        // Cambia el estado.
        ActualState = State.Responding;
        StateHasChanged();

        // Respuesta.
        var response = await Access.Communication.Controllers.Messages.ToEmma(Prompt, Access.Communication.Session.Instance.Token);

        // Cambia el estado.
        ActualState = State.Witting;

        // Es un comando.
        if (response.Model.Content.StartsWith("#"))
        {
            var app = new SILF.Script.App(response.Model.Content.Remove(0, 1), new A());
            app.AddDefaultFunctions(Services.Scripts.Actions);
            app.Run();

            EmmaResponse = "Perfecto";
            StateHasChanged();
            return;
        }

        // Establece la respuesta de Emma.
        EmmaResponse = response.Model.Content;
        StateHasChanged();

    }



    /// <summary>
    /// Invocable.
    /// </summary>
    [JSInvokable("OnEmma")]
    public void OnEmma(string value)
    {
        Prompt = value;
        StateHasChanged();
        ToEmma();
    }



}

class A : IConsole
{
    

    public void InsertLine(string result, SILF.Script.Enums.LogLevel logLevel)
    {
       
    }
}