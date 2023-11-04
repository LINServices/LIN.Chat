using LIN.Allo.Client.Pages;

namespace LIN.Allo.Client.Sections;


public partial class ChatSection
{



    [Inject]
    private IJSRuntime JSRuntime { get; set; }



    /// <summary>
    /// Integrante del chat
    /// </summary>
    [Parameter]
    public MemberChatModel Iam { get; set; } = new();



    /// <summary>
    /// Accion a ejecutar cuando se presione sobre el back button
    /// </summary>
    [Parameter]
    public Action? OnBackPress { get; set; }



    /// <summary>
    /// Hub de conexión
    /// </summary>
    public static Access.Communication.Hubs.ChatHub? Hub { get; set; }



    /// <summary>
    /// Mensaje a enviar
    /// </summary>
    private string Message { get; set; } = string.Empty;





    /// <summary>
    /// Enviar un mensaje
    /// </summary>
    private void SendMessage()
    {

        var guid = Guid.NewGuid().ToString();
        Chat.OnReceiveMessage(new()
        {
            Contenido = Message,
            Conversacion = new()
            {
                ID = Iam.Conversation.ID
            },
            Remitente = Access.Communication.Session.Instance.Informacion,
            Time = DateTime.Now,
            Guid = guid,
            IsLocal = true
        });

        // Envía el mensaje al hub
        Hub?.SendMessage(Iam.Conversation.ID, Message, guid);

        // Reestablece el texto
        Message = "";

        // Actualiza la vista
        StateHasChanged();

        // Scroll al final
        _ = ScrollToBottom();

    }



    /// <summary>
    /// Ciclo de vida: Después de renderizar
    /// </summary>
    /// <param name="firstRender">Es el primer render</param>
    protected override void OnAfterRender(bool firstRender)
    {
        _ = ScrollToBottom();
        base.OnAfterRender(firstRender);
    }



    /// <summary>
    /// El estado ha cambiado
    /// </summary>
    public void Render()
    {
        StateHasChanged();
    }



    /// <summary>
    /// Scroll hasta el final
    /// </summary>
    public async Task ScrollToBottom()
    {
        await JSRuntime.InvokeVoidAsync("scrollToBottom", $"CM-{Iam.Conversation.ID}");
    }


}