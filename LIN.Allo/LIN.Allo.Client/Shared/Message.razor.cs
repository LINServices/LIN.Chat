using Microsoft.AspNetCore.Components;
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

        StateHasChanged();
        base.OnParametersSet();
    }



    /// <summary>
    /// Valida si es un enlace de google meet
    /// </summary>
    /// <param name="texto">Texto a validar</param>
    static bool EsEnlaceGoogleMeet(string texto)
    {
        // Patrón de expresión regular para detectar enlaces de Google Meet
        string patron = @"https://meet\.google\.com/[a-zA-Z0-9\-]+";

        // Comprueba si el texto coincide con el patrón
        return System.Text.RegularExpressions.Regex.IsMatch(texto, patron);
    }




     List<string> SepararCadenas()
    {

        string input = MessageModel.Contenido;
        List<string> resultados = new List<string>();

        // Expresión regular para encontrar menciones que comienzan con @
        string patronMencion = @"@(\w+)";
        MatchCollection menciones = Regex.Matches(input, patronMencion);

        // Separar el texto en partes
        int indiceInicio = 0;
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