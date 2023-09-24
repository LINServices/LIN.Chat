using Microsoft.AspNetCore.Components;

namespace LIN.Chat.Client.Shared;


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
    public bool IsOther => MessageModel.Remitente.ID != LIN.Access.Communication.Session.Instance.Informacion.ID;



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

}