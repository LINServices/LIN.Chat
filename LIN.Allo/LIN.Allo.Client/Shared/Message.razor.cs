using LIN.Allo.Client.Pages;
using System.Text.RegularExpressions;

namespace LIN.Allo.Client.Shared;


public partial class Message
{

    /// <summary>
    /// Modelo del mensaje
    /// </summary>
    [Parameter]
    public MessageModel MessageModel { get; set; } = new();



    /// <summary>
    /// Obtiene si el mensaje no fue enviado por me
    /// </summary>
    public bool IsOther => MessageModel.Remitente.ID != Access.Communication.Session.Instance.Informacion.ID;



    /// <summary>
    /// Tipo del mensaje
    /// </summary>
    private int MessageType = 0;



    /// <summary>
    /// Cuando los parámetros se establecen
    /// </summary>
    protected override void OnParametersSet()
    {
        if (EsEnlaceGoogleMeet(MessageModel.Contenido))
            MessageType = 1;

        if (MessageModel.Contenido == "❤️")
         MessageType = 2;

        StateHasChanged();
        base.OnParametersSet();
    }


    public void UnLocal()
    {
        MessageModel.IsLocal = false;
        StateHasChanged();
    }


    protected override void OnInitialized()
    {
        Chat.MessageTasker.Add(this);
        base.OnInitialized();
    }


    /// <summary>
    /// Valida si es un enlace de google meet
    /// </summary>
    /// <param name="texto">Texto a validar</param>
    private static bool EsEnlaceGoogleMeet(string texto)
    {
        // Patrón de expresión regular para detectar enlaces de Google Meet
        var patron = @"https://meet\.google\.com/[a-zA-Z0-9\-]+";

        // Comprueba si el texto coincide con el patrón
        return Regex.IsMatch(texto, patron);
    }




    private List<string> SepararCadenas()
    {

        var input = MessageModel.Contenido;
        List<string> resultados = new();

        // Expresión regular para encontrar menciones que comienzan con @
        var patronMencion = @"@(\w+)";
        var menciones = Regex.Matches(input, patronMencion);

        // Separar el texto en partes
        var indiceInicio = 0;
        foreach (Match mencion in menciones)
        {
            if (mencion.Index > indiceInicio)
            {
                resultados.Add(input.Substring(indiceInicio, mencion.Index - indiceInicio)); // Agregar texto normal
            }
            resultados.Add(mencion.Value); // Agregar la mención
            indiceInicio = mencion.Index + mencion.Length;
        }

        // Agregar el texto restante, si lo hay
        if (indiceInicio < input.Length)
        {
            resultados.Add(input.Substring(indiceInicio));
        }

        return resultados;
    }

}