using LIN.Types.Exp.Search.Models;
using SILF.Script.Elements.Functions;
using SILF.Script.Enums;
using SILF.Script.Interfaces;
using System.Reflection;
using static LIN.Allo.Client.Services.Scripts;

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
    private HeaderState HeaderActualState { get; set; } = HeaderState.Titles;




    ReadAllResponse<SearchResult> SearchModels { get; set; } = new();


    public ReadOneResponse<Weather> Modelo { get; set; }



    /// <summary>
    /// Lista de estados.
    /// </summary>
    private enum State
    {
        Witting,
        Responding
    }


    /// <summary>
    /// Lista de estados.
    /// </summary>
    private enum HeaderState
    {
        Titles,
        Weather,
        Search
    }





    /// <summary>
    /// Entrada del usuario.
    /// </summary>
    private string Prompt { get; set; } = string.Empty;



    /// <summary>
    /// Respuesta de Emma.
    /// </summary>
    private ReadOneResponse<Types.Emma.Models.ResponseIAModel>? EmmaResponse { get; set; } = null;



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
            app.AddDefaultFunctions(Load());


            EmmaResponse = new()
            {
                Response = Responses.Success,
                Model = new()
                {
                    Content = "Perfecto"
                }
            };

            app.Run();
            StateHasChanged();

            return;
        }

        // Establece la respuesta de Emma.
        EmmaResponse = response;
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




    IEnumerable<SILFFunction> Load()
    {


        // Acción.
        SILFFunction actionMessage =
        new((param) =>
        {

            // Propiedades.
            var content = param.Where(T => T.Name == "contenido").FirstOrDefault();

            EmmaResponse = new()
            {
                Message = content?.Objeto.Value.ToString(),
                Response = Responses.Success,
                Model = new()
                {
                    Content = content?.Objeto.Value.ToString(),
                    IsSuccess = true
                }
            };

            HeaderActualState = HeaderState.Titles;

            StateHasChanged();


        })
        {
            Name = "say",
            Parameters =
            [
                new Parameter("contenido", new("string"))
            ]
        };


        // Acción.
        SILFFunction weather =
        new(async (param) =>
        {

            Modelo = new()
            {
                Response = Responses.IsLoading
            };

            StateHasChanged();

            // Propiedades.
            var content = param.Where(T => T.Name == "contenido").FirstOrDefault();

            var city = await LIN.Access.Search.Controllers.Weather.Get(content.Objeto.Value.ToString());

            Modelo = city;

            HeaderActualState = HeaderState.Weather;

            StateHasChanged();


        })
        {
            Name = "weather",
            Parameters =
            [
                new Parameter("contenido", new("string"))
            ]
        };


        // Acción.
        SILFFunction search =
        new(async (param) =>
        {

            // Propiedades.
            var content = param.Where(T => T.Name == "contenido").FirstOrDefault();

            var city = await LIN.Access.Search.Controllers.Search.Get(content.Objeto.Value.ToString());

            SearchModels = city;

            HeaderActualState = HeaderState.Search;

            StateHasChanged();


        })
        {
            Name = "search",
            Parameters =
            [
                new Parameter("contenido", new("string"))
            ]
        };

        return [actionMessage, weather, search];

    }



}

class A : IConsole
{
    public void InsertLine(string error, string result, SILF.Script.Enums.LogLevel logLevel)
    {
        var s = "";
    }
}